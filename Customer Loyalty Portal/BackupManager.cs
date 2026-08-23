using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public bool IsEnabled { get; set; } = true;
    }

    public class BackupLogEntry
    {
        public string Timestamp { get; set; }
        public string Database { get; set; }
        public string Status { get; set; }
        public string Size { get; set; }
        public string Message { get; set; }
    }

    public class BackupSettings
    {
        private static string SettingsFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupSettings.txt");

        public bool Enabled { get; set; } = true;
        public string BackupDirectory { get; set; } = @"G:\My Drive\SQLBackups";
        public bool Slot1Enabled { get; set; } = true;
        public TimeSpan Slot1Time { get; set; } = new TimeSpan(12, 0, 0); // 12:00 PM
        public bool Slot2Enabled { get; set; } = true;
        public TimeSpan Slot2Time { get; set; } = new TimeSpan(20, 0, 0); // 08:00 PM
        public int RetentionDays { get; set; } = 15;
        public bool IncludeCustomerLoyalty { get; set; } = true;
        public bool IncludeTPH { get; set; } = true;
        public bool IncludeJunior { get; set; } = true;

        public static BackupSettings Current { get; set; } = new BackupSettings();

        static BackupSettings()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    if (!Directory.Exists(@"G:\My Drive"))
                    {
                        Current.BackupDirectory = Directory.Exists(@"D:\") ? @"D:\SQLBackups" : @"C:\SQLBackups";
                    }
                    Save();
                    return;
                }

                string[] lines = File.ReadAllLines(SettingsFilePath);
                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;

                    int idx = trimmed.IndexOf('=');
                    if (idx < 0) continue;

                    string key = trimmed.Substring(0, idx).Trim();
                    string val = trimmed.Substring(idx + 1).Trim();

                    switch (key)
                    {
                        case "Enabled":
                            bool en;
                            if (bool.TryParse(val, out en)) Current.Enabled = en;
                            break;
                        case "BackupDirectory":
                            if (!string.IsNullOrEmpty(val)) Current.BackupDirectory = val;
                            break;
                        case "Slot1Enabled":
                            bool s1en;
                            if (bool.TryParse(val, out s1en)) Current.Slot1Enabled = s1en;
                            break;
                        case "Slot1Time":
                            TimeSpan t1;
                            if (TimeSpan.TryParse(val, out t1)) Current.Slot1Time = t1;
                            break;
                        case "Slot2Enabled":
                            bool s2en;
                            if (bool.TryParse(val, out s2en)) Current.Slot2Enabled = s2en;
                            break;
                        case "Slot2Time":
                            TimeSpan t2;
                            if (TimeSpan.TryParse(val, out t2)) Current.Slot2Time = t2;
                            break;
                        case "RetentionDays":
                            int ret;
                            if (int.TryParse(val, out ret) && ret > 0) Current.RetentionDays = ret;
                            break;
                        case "IncludeCustomerLoyalty":
                            bool cl;
                            if (bool.TryParse(val, out cl)) Current.IncludeCustomerLoyalty = cl;
                            break;
                        case "IncludeTPH":
                            bool tph;
                            if (bool.TryParse(val, out tph)) Current.IncludeTPH = tph;
                            break;
                        case "IncludeJunior":
                            bool jr;
                            if (bool.TryParse(val, out jr)) Current.IncludeJunior = jr;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading BackupSettings: " + ex.Message);
            }
        }

        public static void Save()
        {
            try
            {
                List<string> lines = new List<string>();
                lines.Add("# Customer Loyalty Portal - Database Backup Configuration");
                lines.Add("Enabled=" + (Current.Enabled ? "true" : "false"));
                lines.Add("BackupDirectory=" + Current.BackupDirectory);
                lines.Add("Slot1Enabled=" + (Current.Slot1Enabled ? "true" : "false"));
                lines.Add("Slot1Time=" + Current.Slot1Time.ToString(@"hh\:mm"));
                lines.Add("Slot2Enabled=" + (Current.Slot2Enabled ? "true" : "false"));
                lines.Add("Slot2Time=" + Current.Slot2Time.ToString(@"hh\:mm"));
                lines.Add("RetentionDays=" + Current.RetentionDays);
                lines.Add("IncludeCustomerLoyalty=" + (Current.IncludeCustomerLoyalty ? "true" : "false"));
                lines.Add("IncludeTPH=" + (Current.IncludeTPH ? "true" : "false"));
                lines.Add("IncludeJunior=" + (Current.IncludeJunior ? "true" : "false"));

                File.WriteAllLines(SettingsFilePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving BackupSettings: " + ex.Message);
            }
        }
    }

    public static class BackupManager
    {
        private static readonly object _lock = new object();
        private static bool _isBackingUp = false;
        private static System.Threading.Timer _schedulerTimer = null;

        public static event Action<string> OnProgressMessage;
        public static event Action OnBackupCompleted;

        private static string RecordFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LastBackupSlot.txt");
        private static string AppLogFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupLog.txt");

        public static bool IsBackingUp => _isBackingUp;

        public static string GetCloudBackupDirectory()
        {
            string configuredDir = BackupSettings.Current.BackupDirectory;
            if (!string.IsNullOrWhiteSpace(configuredDir))
            {
                try
                {
                    if (!Directory.Exists(configuredDir)) Directory.CreateDirectory(configuredDir);
                    if (Directory.Exists(configuredDir)) return configuredDir;
                }
                catch { }
            }

            // Fallback to G:\My Drive\SQLBackups
            if (Directory.Exists(@"G:\My Drive"))
            {
                string gDrivePath = @"G:\My Drive\SQLBackups";
                try { if (!Directory.Exists(gDrivePath)) Directory.CreateDirectory(gDrivePath); } catch { }
                if (Directory.Exists(gDrivePath)) return gDrivePath;
            }

            // Fallback to D:\ or C:\
            string fallbackPath = Directory.Exists(@"D:\") ? @"D:\SQLBackups" : @"C:\SQLBackups";
            try { if (!Directory.Exists(fallbackPath)) Directory.CreateDirectory(fallbackPath); } catch { }
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
            if (_isBackingUp || !BackupSettings.Current.Enabled) return;

            DateTime now = DateTime.Now;
            string latestDueSlot = GetLatestDueSlot(now);
            if (string.IsNullOrEmpty(latestDueSlot)) return;

            string lastCompletedSlot = GetLastCompletedSlot();

            if (!string.Equals(latestDueSlot, lastCompletedSlot, StringComparison.OrdinalIgnoreCase))
            {
                Task.Run(() => RunFullBackup(latestDueSlot, triggerSource));
            }
        }

        public static string GetNextScheduledSlotString()
        {
            if (!BackupSettings.Current.Enabled) return "Disabled";

            DateTime now = DateTime.Now;
            DateTime todaySlot1 = now.Date.Add(BackupSettings.Current.Slot1Time);
            DateTime todaySlot2 = now.Date.Add(BackupSettings.Current.Slot2Time);

            if (BackupSettings.Current.Slot1Enabled && now < todaySlot1)
            {
                return $"Today at {todaySlot1:hh:mm tt}";
            }
            else if (BackupSettings.Current.Slot2Enabled && now < todaySlot2)
            {
                return $"Today at {todaySlot2:hh:mm tt}";
            }
            else
            {
                DateTime tomorrowSlot1 = now.Date.AddDays(1).Add(BackupSettings.Current.Slot1Time);
                return $"Tomorrow at {tomorrowSlot1:hh:mm tt}";
            }
        }

        private static string GetLatestDueSlot(DateTime now)
        {
            TimeSpan s1 = BackupSettings.Current.Slot1Time;
            TimeSpan s2 = BackupSettings.Current.Slot2Time;
            bool s1En = BackupSettings.Current.Slot1Enabled;
            bool s2En = BackupSettings.Current.Slot2Enabled;

            DateTime todayS1 = now.Date.Add(s1);
            DateTime todayS2 = now.Date.Add(s2);

            if (s2En && now >= todayS2)
            {
                return now.ToString("yyyy-MM-dd") + "_" + s2.ToString(@"hhmm");
            }
            else if (s1En && now >= todayS1)
            {
                return now.ToString("yyyy-MM-dd") + "_" + s1.ToString(@"hhmm");
            }
            else if (s2En)
            {
                return now.AddDays(-1).ToString("yyyy-MM-dd") + "_" + s2.ToString(@"hhmm");
            }
            else if (s1En)
            {
                return now.AddDays(-1).ToString("yyyy-MM-dd") + "_" + s1.ToString(@"hhmm");
            }
            return "";
        }

        public static string GetLastCompletedSlot()
        {
            try
            {
                if (File.Exists(RecordFilePath))
                {
                    return File.ReadAllText(RecordFilePath).Trim();
                }
            }
            catch (Exception ex)
            {
                Log("Error reading LastBackupSlot: " + ex.Message);
            }
            return "";
        }

        public static void RecordCompletedSlot(string slot)
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
            List<DatabaseBackupTarget> list = new List<DatabaseBackupTarget>();

            if (BackupSettings.Current.IncludeCustomerLoyalty)
            {
                list.Add(new DatabaseBackupTarget
                {
                    Name = "CustomerLoyalty",
                    ServerName = Home.hostServerName,
                    DatabaseName = Home.hostDBName,
                    IsRemoteOnTPH = true,
                    SupportsWithCompression = true
                });
            }

            if (BackupSettings.Current.IncludeTPH)
            {
                list.Add(new DatabaseBackupTarget
                {
                    Name = "TPH_GRExtreme",
                    ServerName = Home.tphServerName,
                    DatabaseName = Home.tphDBName,
                    IsRemoteOnTPH = true,
                    SupportsWithCompression = true
                });
            }

            if (BackupSettings.Current.IncludeJunior)
            {
                list.Add(new DatabaseBackupTarget
                {
                    Name = "Junior_GRetail",
                    ServerName = Home.jrServerName,
                    DatabaseName = Home.jrDBName,
                    IsRemoteOnTPH = false,
                    SupportsWithCompression = false
                });
            }

            return list;
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

                ReportProgress($"Starting backup ({triggerSource})...");
                Log($"==================================================");
                Log($"[AutoBackup] Starting Full Database Backup (Trigger: {triggerSource}, Slot: {slotName ?? "Manual"})");
                Log($"[AutoBackup] Destination Directory: {backupDir}");

                var targets = GetBackupTargets();
                if (targets.Count == 0)
                {
                    ReportProgress("No databases selected for backup.");
                    Log("[AutoBackup] No databases selected for backup in settings.");
                    return false;
                }

                int successCount = 0;

                foreach (var target in targets)
                {
                    ReportProgress($"Backing up {target.Name}...");
                    bool success = BackupSingleDatabase(target, backupDir, stagingDir, timestamp);
                    if (success) successCount++;
                }

                // Apply Retention Cleanup
                ReportProgress("Applying retention cleanup...");
                ApplyRetentionCleanup(backupDir, BackupSettings.Current.RetentionDays);

                Log($"[AutoBackup] Backup completed: {successCount}/{targets.Count} databases backed up successfully.");
                Log($"==================================================");

                if (successCount == targets.Count)
                {
                    ReportProgress($"✓ All {targets.Count} database(s) backed up successfully!");
                    if (!string.IsNullOrEmpty(slotName))
                    {
                        RecordCompletedSlot(slotName);
                    }
                }
                else
                {
                    ReportProgress($"⚠️ Partial backup: {successCount}/{targets.Count} database(s) succeeded.");
                }

                return successCount == targets.Count;
            }
            catch (Exception ex)
            {
                ReportProgress($"Error: {ex.Message}");
                Log($"[AutoBackup] Fatal error during backup process: {ex.Message}");
                return false;
            }
            finally
            {
                lock (_lock)
                {
                    _isBackingUp = false;
                }
                OnBackupCompleted?.Invoke();
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
                string bakName = $"{target.Name}_{timestamp}.bak";
                rawBakPathOnServer = $@"E:\AutoBackup\{bakName}";
                accessibleBakPathOnClient = $@"Z:\AutoBackup\{bakName}";
            }
            else
            {
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

                // Wait up to 10 seconds for file visibility
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

                // Compress to .zip
                if (File.Exists(finalZipPath)) File.Delete(finalZipPath);

                using (ZipArchive archive = ZipFile.Open(finalZipPath, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(accessibleBakPathOnClient, Path.GetFileName(accessibleBakPathOnClient), CompressionLevel.Optimal);
                }

                FileInfo zipInfo = new FileInfo(finalZipPath);
                string sizeMb = (zipInfo.Length / (1024.0 * 1024.0)).ToString("N1") + " MB";
                Log($"[AutoBackup] [{target.Name}] SUCCESS -> Saved to {finalZipPath} ({sizeMb})");

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

        public static void ApplyRetentionCleanup(string baseBackupDir, int retentionDays)
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

        public static List<BackupLogEntry> GetBackupHistory()
        {
            List<BackupLogEntry> entries = new List<BackupLogEntry>();
            try
            {
                string logFile = Path.Combine(GetCloudBackupDirectory(), "BackupLog.txt");
                if (!File.Exists(logFile) && File.Exists(AppLogFilePath)) logFile = AppLogFilePath;
                if (!File.Exists(logFile)) return entries;

                string[] lines = File.ReadAllLines(logFile);
                foreach (var line in lines.Reverse())
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("==")) continue;

                    if (trimmed.Contains("[AutoBackup]") && (trimmed.Contains("SUCCESS ->") || trimmed.Contains("FAILED:")))
                    {
                        try
                        {
                            int tsEnd = trimmed.IndexOf(']');
                            string timestamp = tsEnd > 1 ? trimmed.Substring(1, tsEnd - 1) : "";

                            string rest = trimmed.Substring(tsEnd + 1).Trim();
                            string dbName = "";
                            if (rest.Contains("[") && rest.Contains("]"))
                            {
                                int d1 = rest.IndexOf('[');
                                int d2 = rest.IndexOf(']', d1);
                                if (d2 > d1)
                                {
                                    dbName = rest.Substring(d1 + 1, d2 - d1 - 1).Replace("AutoBackup", "").Trim();
                                    if (string.IsNullOrEmpty(dbName))
                                    {
                                        int next1 = rest.IndexOf('[', d2);
                                        int next2 = rest.IndexOf(']', next1);
                                        if (next2 > next1) dbName = rest.Substring(next1 + 1, next2 - next1 - 1);
                                    }
                                }
                            }

                            string status = trimmed.Contains("SUCCESS") ? "SUCCESS" : "FAILED";
                            string size = "";
                            if (status == "SUCCESS" && trimmed.Contains("(") && trimmed.Contains(")"))
                            {
                                int p1 = trimmed.LastIndexOf('(');
                                int p2 = trimmed.LastIndexOf(')');
                                if (p2 > p1) size = trimmed.Substring(p1 + 1, p2 - p1 - 1);
                            }

                            string message = "";
                            if (status == "SUCCESS")
                            {
                                message = "Compressed archive created and saved to cloud.";
                            }
                            else
                            {
                                int fIdx = trimmed.IndexOf("FAILED:");
                                message = fIdx >= 0 ? trimmed.Substring(fIdx + 7).Trim() : "Backup failed.";
                            }

                            entries.Add(new BackupLogEntry
                            {
                                Timestamp = timestamp,
                                Database = dbName,
                                Status = status,
                                Size = size,
                                Message = message
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing backup history: " + ex.Message);
            }
            return entries;
        }

        private static void ReportProgress(string msg)
        {
            OnProgressMessage?.Invoke(msg);
        }

        private static void Log(string message)
        {
            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Console.WriteLine(line);

            // Log to local app directory
            try
            {
                File.AppendAllText(AppLogFilePath, line + Environment.NewLine);
            }
            catch { }

            // Also log to destination cloud folder
            try
            {
                string cloudLog = Path.Combine(GetCloudBackupDirectory(), "BackupLog.txt");
                File.AppendAllText(cloudLog, line + Environment.NewLine);
            }
            catch { }
        }
    }
}
