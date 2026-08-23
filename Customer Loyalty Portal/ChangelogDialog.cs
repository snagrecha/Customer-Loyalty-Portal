using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Customer_Loyalty_Portal
{
    public partial class ChangelogDialog : Form
    {
        public ChangelogDialog()
        {
            InitializeComponent();
            lblSubTitle.Text = $"Version History & Release Notes (Build: {AppVersion.BuildDate})";
            PopulateChangelog();
        }

        private void PopulateChangelog()
        {
            contentPanel.Controls.Clear();
            contentPanel.SuspendLayout();

            int currentY = 15;
            int cardWidth = contentPanel.ClientSize.Width - 45;

            var list = AppVersion.GetChangelog();
            foreach (var item in list)
            {
                Panel card = CreateReleaseCard(item, cardWidth);
                card.Location = new Point(15, currentY);
                contentPanel.Controls.Add(card);
                currentY += card.Height + 15;
            }

            contentPanel.ResumeLayout();
        }

        private Panel CreateReleaseCard(ChangelogEntry entry, int width)
        {
            Panel card = new Panel
            {
                Width = width,
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(14, 12, 14, 12)
            };

            // Version & Date Header
            Label headerLabel = new Label
            {
                Text = $"{entry.Version}  •  {entry.ReleaseDate}",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 110, 253),
                AutoSize = true,
                Location = new Point(12, 10)
            };
            card.Controls.Add(headerLabel);

            // Title
            Label titleLabel = new Label
            {
                Text = entry.Title,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = true,
                Location = new Point(12, 34)
            };
            card.Controls.Add(titleLabel);

            int bulletY = 60;
            foreach (var bullet in entry.Highlights)
            {
                Label bulletLabel = new Label
                {
                    Text = "• " + bullet,
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(73, 80, 87),
                    Location = new Point(16, bulletY),
                    Width = width - 35,
                    AutoSize = false
                };

                // Calculate required height for wrapped text
                using (Graphics g = bulletLabel.CreateGraphics())
                {
                    SizeF size = g.MeasureString(bulletLabel.Text, bulletLabel.Font, bulletLabel.Width);
                    bulletLabel.Height = (int)Math.Ceiling(size.Height) + 4;
                }

                card.Controls.Add(bulletLabel);
                bulletY += bulletLabel.Height + 4;
            }

            card.Height = bulletY + 10;
            return card;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
