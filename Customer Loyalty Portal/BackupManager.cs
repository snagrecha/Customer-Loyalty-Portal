using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Customer_Loyalty_Portal
{
    public class DatabaseBackupTarget
    {
        public string Name { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public bool IsRemoteOnTPH { get; set; }
        public bool SupportsWithCompression { get; set; }
    }

    public static class BackupManager
    {
        private static readonly object _lock = new object();
        private static bool _isBackingUp = false;
        private static System.Threading.Timer _schedulerTimer = null;

        private static string RecordFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LastBackupSlot.txt");
        private static string LogFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupLog.txt");

        public static string GetCloudBackupDirectory()
        {
            // Check Google Drive G:\ drive first
            if (Directory.Exists(@"G:\My Drive"))
            {
                string gDrivePath = @"G:\My Drive\SQLBackups";
                if (!Directory.Exists(gDrivePath))
                {
                    try { Directory.CreateDirectory(gDrivePath); } catch { }
                }
                if (Directory.Exists(gDrivePath)) return gDrivePath;
            }

            // Fallback to D:\ or C:\
            string fallbackPath = Directory.Exists(@"D:\") ? @"D:\SQLBackups\GoogleDrive" : @"C:\SQLBackups\GoogleDrive";
            if (!Directory.Exists(fallbackPath))
            {
                try { Directory.CreateDirectory(fallbackPath); } catch { }
            }
            return fallbackPath;
        }

        private static string GetLocalStagingDirectory()
        {
            string staging = Directory.Exists(@"D:\") ? @"D:\SQLBackups\Staging" : @"C:\SQLBackups\Staging";
            if (!Directory.Exists(staging))
            {
                try { Directory.CreateDirectory(staging); } catch { }
            }
            return staging;
        }

        public static void InitializeAndStartWorker()
        {
            // 1. Initial catch-up check 10 seconds after app startup
            Task.Run(async () =>
            {
                await Task.Delay(10000);
                CheckAndRunDueBackups("Startup Catch-up");
            });

            // 2. Periodic background check every 60 seconds
            if (_schedulerTimer == null)
            {
                _schedulerTimer = new System.Threading.Timer((state) =>
                {
                    CheckAndRunDueBackups("Scheduled Timer");
                }, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
            }
        }

        public static void CheckAndRunDueBackups(string triggerSource)
        {
            if (_isBackingUp) return;

            DateTime now = DateTime.Now;
            string latestDueSlot = GetLatestDueSlot(now);
            string lastCompletedSlot = GetLastCompletedSlot();

            if (!string.Equals(latestDueSlot, lastCompletedSlot, StringComparison.OrdinalIgnoreCase))
            {
                Task.Run(() => RunFullBackup(latestDueSlot, triggerSource));
            }
        }

        private static string GetLatestDueSlot(DateTime now)
        {
            if (now.Hour >= 20)
            {
                return now.ToString("yyyy-MM-dd") + "_2000";
            }
            else if (now.Hour >= 12)
            {
                return now.ToString("yyyy-MM-dd") + "_1200";
            }
            else
            {
                // Previous evening slot
                return now.AddDays(-1).ToString("yyyy-MM-dd") + "_2000";
            }
        }

        private static string GetLastCompletedSlot()
        {
            try
            {
                if (File.Exists(RecordFilePath))
                {
                    string content = File.ReadAllText(RecordFilePath).Trim();
                    return content;
                }
            }
            catch (Exception ex)
            {
                Log("Error reading LastBackupSlot: " + ex.Message);
            }
            return "";
        }

        private static void RecordCompletedSlot(string slot)
        {
            try
            {
                File.WriteAllText(RecordFilePath, slot);
            }
            catch (Exception ex)
            {
                Log("Error saving LastBackupSlot: " + ex.Message);
            }
        }

        public static List<DatabaseBackupTarget> GetBackupTargets()
        {
            return new List<DatabaseBackupTarget>
            {
                new DatabaseBackupTarget
                {
                    Name = "CustomerLoyalty",
                    ServerName = Home.hostServerName,
                    DatabaseName = Home.hostDBName,
                    IsRemoteOnTPH = true,
                    SupportsWithCompression = true
                },
                new DatabaseBackupTarget
                {
                    Name = "TPH_GRExtreme",
                    ServerName = Home.tphServerName,
                    DatabaseName = Home.tphDBName,
                    IsRemoteOnTPH = true,
                    SupportsWithCompression = true
                },
                new DatabaseBackupTarget
                {
                    Name = "Junior_GRetail",
                    ServerName = Home.jrServerName,
                    DatabaseName = Home.jrDBName,
                    IsRemoteOnTPH = false,
                    SupportsWithCompression = false
                }
            };
        }

        public static bool RunFullBackup(string slotName = null, string triggerSource = "Manual")
        {
            lock (_lock)
            {
                if (_isBackingUp) return false;
                _isBackingUp = true;
            }

            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmm");
                string backupDir = GetCloudBackupDirectory();
                string stagingDir = GetLocalStagingDirectory();

                Log($"==================================================");
                Log($"[AutoBackup] Starting Full Database Backup (Trigger: {triggerSource}, Slot: {slotName ?? "Manual"})");
                Log($"[AutoBackup] Destination Directory: {backupDir}");

                var targets = GetBackupTargets();
                int successCount = 0;

                foreach (var target in targets)
                {
                    bool success = BackupSingleDatabase(target, backupDir, stagingDir, timestamp);
                    if (success) successCount++;
                }

                // Apply 15-Day Retention Cleanup
                ApplyRetentionCleanup(backupDir, 15);

                Log($"[AutoBackup] Backup completed: {successCount}/{targets.Count} databases backed up successfully.");
                Log($"==================================================");

                if (successCount == targets.Count && !string.IsNullOrEmpty(slotName))
                {
                    RecordCompletedSlot(slotName);
                }

                return successCount == targets.Count;
            }
            catch (Exception ex)
            {
                Log($"[AutoBackup] Fatal error during backup process: {ex.Message}");
                return false;
            }
            finally
            {
                lock (_lock)
                {
                    _isBackingUp = false;
                }
            }
        }

        private static bool BackupSingleDatabase(DatabaseBackupTarget target, string destinationDir, string localStagingDir, string timestamp)
        {
            string dbFolder = Path.Combine(destinationDir, target.Name);
            if (!Directory.Exists(dbFolder))
            {
                try { Directory.CreateDirectory(dbFolder); } catch { }
            }

            string zipFileName = $"{target.Name}_{timestamp}.zip";
            string finalZipPath = Path.Combine(dbFolder, zipFileName);

            string rawBakPathOnServer;
            string accessibleBakPathOnClient;

            if (target.IsRemoteOnTPH)
            {
                // SQL Server on TPH writes to E:\AutoBackup\
                string bakName = $"{target.Name}_{timestamp}.bak";
                rawBakPathOnServer = $@"E:\AutoBackup\{bakName}";
                accessibleBakPathOnClient = $@"Z:\AutoBackup\{bakName}";
            }
            else
            {
                // Local SQL Server on Junior writes to local staging directory
                string bakName = $"{target.Name}_{timestamp}.bak";
                rawBakPathOnServer = Path.Combine(localStagingDir, bakName);
                accessibleBakPathOnClient = rawBakPathOnServer;
            }

            try
            {
                Log($"[AutoBackup] [{target.Name}] Initiating SQL Server backup of '{target.DatabaseName}' on '{target.ServerName}'...");

                string connStr = $"Server={target.ServerName};Initial Catalog={target.DatabaseName};UID=cl_admin;PWD=Tph@2015;Connect Timeout=30;Pooling=False";
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 300; // 5 mins timeout
                        string compressionClause = target.SupportsWithCompression ? ", COMPRESSION" : "";
                        cmd.CommandText = $"BACKUP DATABASE [{target.DatabaseName}] TO DISK = '{rawBakPathOnServer}' WITH FORMAT, INIT{compressionClause};";
                        cmd.ExecuteNonQuery();
                    }
                }

                Log($"[AutoBackup] [{target.Name}] SQL backup created. Compressing to .zip archive...");

                // Wait up to 10 seconds for file to be ready/visible over network
                int waitCount = 0;
                while (!File.Exists(accessibleBakPathOnClient) && waitCount < 10)
                {
                    Thread.Sleep(1000);
                    waitCount++;
                }

                if (!File.Exists(accessibleBakPathOnClient))
                {
                    throw new FileNotFoundException($"Generated backup file not accessible at: {accessibleBakPathOnClient}");
                }

                // Compress .bak file to .zip
                if (File.Exists(finalZipPath)) File.Delete(finalZipPath);

                using (ZipArchive archive = ZipFile.Open(finalZipPath, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(accessibleBakPathOnClient, Path.GetFileName(accessibleBakPathOnClient), CompressionLevel.Optimal);
                }

                FileInfo zipInfo = new FileInfo(finalZipPath);
                Log($"[AutoBackup] [{target.Name}] SUCCESS -> Saved to {finalZipPath} ({zipInfo.Length / (1024 * 1024):N1} MB)");

                // Clean up staging .bak file
                try
                {
                    if (File.Exists(accessibleBakPathOnClient)) File.Delete(accessibleBakPathOnClient);
                }
                catch { }

                return true;
            }
            catch (Exception ex)
            {
                Log($"[AutoBackup] [{target.Name}] FAILED: {ex.Message}");
                return false;
            }
        }

        public static void ApplyRetentionCleanup(string baseBackupDir, int retentionDays = 15)
        {
            try
            {
                if (!Directory.Exists(baseBackupDir)) return;

                DateTime cutoff = DateTime.Now.AddDays(-retentionDays);
                string[] zipFiles = Directory.GetFiles(baseBackupDir, "*.zip", SearchOption.AllDirectories);

                int deletedCount = 0;
                foreach (string file in zipFiles)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.LastWriteTime < cutoff && fi.CreationTime < cutoff)
                        {
                            fi.Delete();
                            deletedCount++;
                            Log($"[AutoBackup] Retention Cleanup: Deleted old backup archive {fi.Name}");
                        }
                    }
                    catch (Exception fEx)
                    {
                        Log($"[AutoBackup] Retention file error ({Path.GetFileName(file)}): {fEx.Message}");
                    }
                }

                if (deletedCount > 0)
                {
                    Log($"[AutoBackup] Retention Cleanup completed: {deletedCount} old backup(s) pruned.");
                }
            }
            catch (Exception ex)
            {
                Log($"[AutoBackup] Retention Cleanup error: {ex.Message}");
            }
        }

        private static void Log(string message)
        {
            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Console.WriteLine(line);
            try
            {
                File.AppendAllText(LogFilePath, line + Environment.NewLine);
            }
            catch { }
        }
    }
}
