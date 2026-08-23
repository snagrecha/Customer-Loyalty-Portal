using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Customer_Loyalty_Portal
{
    public class PendingEmailItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string ToEmail { get; set; }
        public string Source { get; set; }
        public DateTime ReportDate { get; set; }
        public string AttachmentPath { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime EnqueuedAt { get; set; } = DateTime.Now;
        public DateTime? LastAttemptAt { get; set; }
        public int AttemptCount { get; set; } = 0;
        public string LastError { get; set; } = "";
    }

    public static class EmailQueueManager
    {
        private static readonly object _lock = new object();
        private static bool _isProcessing = false;
        private static System.Threading.Timer _retryTimer = null;

        private static string QueueFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PendingEmailQueue.txt");

        public static void InitializeAndStartWorker()
        {
            // 1. Initial background queue flush 5 seconds after startup
            Task.Run(async () =>
            {
                await Task.Delay(5000);
                await ProcessQueueAsync();
            });

            // 2. Periodic background worker every 5 minutes
            if (_retryTimer == null)
            {
                _retryTimer = new System.Threading.Timer(async (state) =>
                {
                    await ProcessQueueAsync();
                }, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
            }
        }

        public static void Enqueue(PendingEmailItem item)
        {
            if (item == null) return;
            lock (_lock)
            {
                List<PendingEmailItem> items = LoadQueueInternal();

                // Prevent duplicate enqueue for the exact same source and date if already pending
                bool exists = items.Any(i => i.Source == item.Source && i.ReportDate.Date == item.ReportDate.Date);
                if (!exists)
                {
                    items.Add(item);
                    SaveQueueInternal(items);
                    Console.WriteLine($"[EmailQueue] Queued email to {item.ToEmail} for {item.Source} ({item.ReportDate:dd-MM-yyyy})");
                }
            }
        }

        public static List<PendingEmailItem> GetPendingItems()
        {
            lock (_lock)
            {
                return LoadQueueInternal();
            }
        }

        public static async Task ProcessQueueAsync()
        {
            if (_isProcessing) return;
            _isProcessing = true;
            await Task.Yield();

            try
            {
                List<PendingEmailItem> items;
                lock (_lock)
                {
                    items = LoadQueueInternal();
                }

                if (items == null || items.Count == 0)
                {
                    return;
                }

                List<PendingEmailItem> remaining = new List<PendingEmailItem>();

                foreach (var item in items)
                {
                    string error = "";
                    bool sent = WriteToExcel.SendEmailDirect(item.ToEmail, item.Subject, item.Body, item.AttachmentPath, out error);

                    if (sent)
                    {
                        Console.WriteLine($"[EmailQueue] Successfully sent queued email to {item.ToEmail} for {item.Source} ({item.ReportDate:dd-MM-yyyy})");
                    }
                    else
                    {
                        item.AttemptCount++;
                        item.LastAttemptAt = DateTime.Now;
                        item.LastError = error;
                        remaining.Add(item);
                        Console.WriteLine($"[EmailQueue] Retry failed for {item.ToEmail} ({item.Source}): {error}");
                    }
                }

                lock (_lock)
                {
                    SaveQueueInternal(remaining);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[EmailQueue] ProcessQueue error: " + ex.Message);
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private static List<PendingEmailItem> LoadQueueInternal()
        {
            List<PendingEmailItem> list = new List<PendingEmailItem>();
            try
            {
                if (!File.Exists(QueueFilePath)) return list;

                string[] lines = File.ReadAllLines(QueueFilePath);
                PendingEmailItem current = null;

                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    if (trimmed == "===BEGIN_EMAIL===")
                    {
                        current = new PendingEmailItem();
                    }
                    else if (trimmed == "===END_EMAIL===" && current != null)
                    {
                        list.Add(current);
                        current = null;
                    }
                    else if (current != null && trimmed.Contains("="))
                    {
                        int idx = trimmed.IndexOf('=');
                        string key = trimmed.Substring(0, idx).Trim();
                        string val = trimmed.Substring(idx + 1).Trim();

                        switch (key)
                        {
                            case "Id":
                                current.Id = val;
                                break;
                            case "ToEmail":
                                current.ToEmail = val;
                                break;
                            case "Source":
                                current.Source = val;
                                break;
                            case "ReportDate":
                                DateTime rDate;
                                if (DateTime.TryParse(val, out rDate)) current.ReportDate = rDate;
                                break;
                            case "AttachmentPath":
                                current.AttachmentPath = val;
                                break;
                            case "Subject":
                                current.Subject = val;
                                break;
                            case "Body":
                                try
                                {
                                    byte[] data = Convert.FromBase64String(val);
                                    current.Body = Encoding.UTF8.GetString(data);
                                }
                                catch
                                {
                                    current.Body = val;
                                }
                                break;
                            case "EnqueuedAt":
                                DateTime eDate;
                                if (DateTime.TryParse(val, out eDate)) current.EnqueuedAt = eDate;
                                break;
                            case "LastAttemptAt":
                                DateTime lDate;
                                if (DateTime.TryParse(val, out lDate)) current.LastAttemptAt = lDate;
                                break;
                            case "AttemptCount":
                                int cnt;
                                if (int.TryParse(val, out cnt)) current.AttemptCount = cnt;
                                break;
                            case "LastError":
                                current.LastError = val;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading email queue file: " + ex.Message);
            }
            return list;
        }

        private static void SaveQueueInternal(List<PendingEmailItem> items)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (var item in items)
                {
                    lines.Add("===BEGIN_EMAIL===");
                    lines.Add("Id=" + item.Id);
                    lines.Add("ToEmail=" + item.ToEmail);
                    lines.Add("Source=" + item.Source);
                    lines.Add("ReportDate=" + item.ReportDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    lines.Add("AttachmentPath=" + item.AttachmentPath);
                    lines.Add("Subject=" + item.Subject);
                    string base64Body = Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Body ?? ""));
                    lines.Add("Body=" + base64Body);
                    lines.Add("EnqueuedAt=" + item.EnqueuedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    lines.Add("LastAttemptAt=" + (item.LastAttemptAt.HasValue ? item.LastAttemptAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""));
                    lines.Add("AttemptCount=" + item.AttemptCount);
                    lines.Add("LastError=" + (item.LastError ?? "").Replace('\r', ' ').Replace('\n', ' '));
                    lines.Add("===END_EMAIL===");
                }
                File.WriteAllLines(QueueFilePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing email queue file: " + ex.Message);
            }
        }
    }
}
