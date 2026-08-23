using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Customer_Loyalty_Portal
{
    public partial class IncentiveConfigDialog : Form
    {
        public IncentiveConfigDialog()
        {
            InitializeComponent();
            SetupGridStyle();
            LoadCurrentSettings();
        }

        private void SetupGridStyle()
        {
            slabsGrid.EnableHeadersVisualStyles = false;
            slabsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 60, 90);
            slabsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            slabsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            slabsGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            slabsGrid.ColumnHeadersHeight = 30;
            slabsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            slabsGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            slabsGrid.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            slabsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 229, 255);
            slabsGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            slabsGrid.RowTemplate.Height = 26;

            slabsGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 249, 252);
            slabsGrid.BackgroundColor = Color.White;
            slabsGrid.BorderStyle = BorderStyle.FixedSingle;
            slabsGrid.GridColor = Color.FromArgb(222, 226, 230);
            slabsGrid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            slabsGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            slabsGrid.ColumnCount = 2;
            slabsGrid.Columns[0].Name = "MinAmount";
            slabsGrid.Columns[0].HeaderText = "Min. Bill Amount (₹)";
            slabsGrid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            slabsGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            slabsGrid.Columns[0].DefaultCellStyle.Format = "N0";

            slabsGrid.Columns[1].Name = "IncentiveAmount";
            slabsGrid.Columns[1].HeaderText = "Incentive Amount (₹)";
            slabsGrid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            slabsGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            slabsGrid.Columns[1].DefaultCellStyle.Format = "N0";
        }

        private void LoadCurrentSettings()
        {
            chkEnableIncentive.Checked = IncentiveManager.Enabled;
            slabsGrid.Rows.Clear();

            foreach (var slab in IncentiveManager.Slabs)
            {
                slabsGrid.Rows.Add(slab.MinAmount.ToString("0"), slab.IncentiveAmount.ToString("0"));
            }
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            slabsGrid.Rows.Add("5000", "15");
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            if (slabsGrid.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in slabsGrid.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        slabsGrid.Rows.Remove(row);
                    }
                }
            }
            else if (slabsGrid.CurrentRow != null && !slabsGrid.CurrentRow.IsNewRow)
            {
                slabsGrid.Rows.Remove(slabsGrid.CurrentRow);
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "Delete Slab", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Reset incentive slabs to standard defaults (5000: ₹15, 7500: ₹20)?", "Reset Defaults", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                chkEnableIncentive.Checked = true;
                slabsGrid.Rows.Clear();
                slabsGrid.Rows.Add("5000", "15");
                slabsGrid.Rows.Add("7500", "20");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<IncentiveSlab> newSlabs = new List<IncentiveSlab>();

            foreach (DataGridViewRow row in slabsGrid.Rows)
            {
                if (row.IsNewRow) continue;

                object valMin = row.Cells[0].Value;
                object valInc = row.Cells[1].Value;

                if (valMin == null || valInc == null) continue;

                decimal minAmt, incAmt;
                if (decimal.TryParse(valMin.ToString().Trim(), out minAmt) && decimal.TryParse(valInc.ToString().Trim(), out incAmt))
                {
                    if (minAmt > 0 && incAmt > 0)
                    {
                        newSlabs.Add(new IncentiveSlab(minAmt, incAmt));
                    }
                }
            }

            IncentiveManager.SaveSettings(chkEnableIncentive.Checked, newSlabs);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
