using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Customer_Loyalty_Portal
{
    public partial class CustomerSearchDialog : Form
    {
        public string SelectedMobile { get; private set; }
        public string SelectedName { get; private set; }
        public string SelectedBalance { get; private set; }

        public CustomerSearchDialog(DataTable dt, string searchQuery = "")
        {
            InitializeComponent();
            SelectedMobile = "";
            SelectedName = "";
            SelectedBalance = "";

            if (!string.IsNullOrEmpty(searchQuery))
            {
                infoLabel.Text = "Found " + dt.Rows.Count + " customer(s) matching \"" + searchQuery + "\". Select one to proceed:";
            }
            else
            {
                infoLabel.Text = "Found " + dt.Rows.Count + " customer(s). Select one to proceed:";
            }

            PopulateGrid(dt);
        }

        private void PopulateGrid(DataTable dt)
        {
            customersGrid.Columns.Clear();
            customersGrid.AutoGenerateColumns = false;

            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.Name = "Name";
            nameCol.HeaderText = "Customer Name";
            nameCol.DataPropertyName = "Name";
            nameCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            nameCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            nameCol.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);

            DataGridViewTextBoxColumn mobileCol = new DataGridViewTextBoxColumn();
            mobileCol.Name = "Mobile";
            mobileCol.HeaderText = "Mobile Number";
            mobileCol.DataPropertyName = "Mobile";
            mobileCol.Width = 140;
            mobileCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewTextBoxColumn balanceCol = new DataGridViewTextBoxColumn();
            balanceCol.Name = "Balance";
            balanceCol.HeaderText = "Loyalty Points";
            balanceCol.DataPropertyName = "Balance";
            balanceCol.Width = 120;
            balanceCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            balanceCol.DefaultCellStyle.Padding = new Padding(0, 0, 8, 0);

            customersGrid.Columns.Add(nameCol);
            customersGrid.Columns.Add(mobileCol);
            customersGrid.Columns.Add(balanceCol);

            customersGrid.DataSource = dt;
            StyleGrid(customersGrid);

            if (customersGrid.Rows.Count > 0)
            {
                customersGrid.Rows[0].Selected = true;
            }
        }

        private void StyleGrid(DataGridView grid)
        {
            if (grid == null) return;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 60, 90);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 30;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 229, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.RowTemplate.Height = 26;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 249, 252);
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = Color.FromArgb(222, 226, 230);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }

        private void ConfirmSelection()
        {
            if (customersGrid.CurrentRow != null && customersGrid.CurrentRow.Index >= 0)
            {
                DataGridViewRow row = customersGrid.CurrentRow;
                SelectedName = row.Cells["Name"].Value != null ? row.Cells["Name"].Value.ToString() : "";
                SelectedMobile = row.Cells["Mobile"].Value != null ? row.Cells["Mobile"].Value.ToString() : "";
                SelectedBalance = row.Cells["Balance"].Value != null ? row.Cells["Balance"].Value.ToString() : "0";

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a customer from the list.", "No Customer Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            ConfirmSelection();
        }

        private void customersGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ConfirmSelection();
            }
        }

        private void customersGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                ConfirmSelection();
            }
        }
    }
}
