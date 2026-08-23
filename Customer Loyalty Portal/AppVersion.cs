using System;
using System.Collections.Generic;
using System.Reflection;

namespace Customer_Loyalty_Portal
{
    public class ChangelogEntry
    {
        public string Version { get; set; }
        public string ReleaseDate { get; set; }
        public string Title { get; set; }
        public List<string> Highlights { get; set; } = new List<string>();
    }

    public static class AppVersion
    {
        public const string Version = "v2.5.0";
        public const string BuildDate = "23-Aug-2026";
        public const string AppName = "Customer Loyalty Portal";

        public static string FullVersionTitle => $"{AppName}  •  {Version} ({BuildDate})";
        public static string VersionBadge => $"{Version} • What's New ℹ";

        public static List<ChangelogEntry> GetChangelog()
        {
            return new List<ChangelogEntry>
            {
                new ChangelogEntry
                {
                    Version = "v2.5.0",
                    ReleaseDate = "23-Aug-2026",
                    Title = "Resilient Email Outbox, Configurable Incentives & UI Modernization",
                    Highlights = new List<string>
                    {
                        "Offline Email Queue: Daily reports automatically save to persistent Outbox if internet is down and auto-retry every 5 minutes and on app launch.",
                        "Configurable Salesman Incentives: Added '⚙ Incentives' settings modal to configure tiered bill reward slabs.",
                        "Same-Day Customer Bill Aggregation: Multiple bills for the same customer on the same day are merged for incentive tier qualification.",
                        "Clean Incentive Hover Breakdown: Hovering over Commission / Total cells shows contributing bill details without clutter.",
                        "Print Report Refinement: Streamlined A5 report layout with taxable amount column removed and net payables clearly formatted.",
                        "Custom Title Bar: Modern dark navy title bar with custom window controls and smooth window dragging."
                    }
                },
                new ChangelogEntry
                {
                    Version = "v2.4.0",
                    ReleaseDate = "22-Aug-2026",
                    Title = "Detailed Bill View & Historical Daily Balance",
                    Highlights = new List<string>
                    {
                        "Detailed Bill View: Double-clicking any bill or clicking 'View' opens an itemized transaction dialog with item codes, names, MRP, rates, and taxes.",
                        "Historical Daily Balance: Date picker on the Daily Balance tab to inspect past cash, credit, debit, and transaction entries.",
                        "Visual Modernization: Upgraded UI styling, Segoe UI typography, styled DataGridViews, and status badges."
                    }
                },
                new ChangelogEntry
                {
                    Version = "v2.0.0",
                    ReleaseDate = "2024 - 2025",
                    Title = "Dual Store Integration & Daily Balance Automation",
                    Highlights = new List<string>
                    {
                        "Dual Store Integration: Cross-server data access between TPH and Junior stores.",
                        "Automated Excel Balance Sheets: One-click Excel generation, printing, and automated email dispatch.",
                        "Customer Loyalty Points: Automatic points calculation, tier bag rewards, and customer transfer support."
                    }
                }
            };
        }
    }
}
