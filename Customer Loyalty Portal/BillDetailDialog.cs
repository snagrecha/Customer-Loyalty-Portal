using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Customer_Loyalty_Portal
{
    public partial class BillDetailDialog : Form
    {
        private string serverName;
        private string dbname;
        private string sourceName;
        private string salesID;
        private string voucherNo;
        private string customerName;
        private string mobile;
        private string dateStr;
        private string timeStr;

        public BillDetailDialog(string serverName, string dbname, string sourceName, string salesID, string voucherNo, string customerName, string mobile, string dateStr, string timeStr)
        {
            InitializeComponent();
            this.serverName = serverName;
            this.dbname = dbname;
            this.sourceName = sourceName;
            this.salesID = salesID;
            this.voucherNo = voucherNo;
            this.customerName = customerName;
            this.mobile = mobile;
            this.dateStr = dateStr;
            this.timeStr = timeStr;

            SetupGridStyle();
            LoadBillData();
        }

        private void SetupGridStyle()
        {
            itemsGrid.EnableHeadersVisualStyles = false;
            itemsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 60, 90);
            itemsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            itemsGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            itemsGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            itemsGrid.ColumnHeadersHeight = 32;
            itemsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            itemsGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            itemsGrid.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            itemsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 229, 255);
            itemsGrid.DefaultCellStyle.SelectionForeColor = Color.Black;
            itemsGrid.RowTemplate.Height = 24;

            itemsGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 249, 252);
            itemsGrid.BackgroundColor = Color.White;
            itemsGrid.BorderStyle = BorderStyle.FixedSingle;
            itemsGrid.GridColor = Color.FromArgb(222, 226, 230);
            itemsGrid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            itemsGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            itemsGrid.ColumnCount = 10;
            itemsGrid.Columns[0].Name = "Sr";
            itemsGrid.Columns[1].Name = "Barcode";
            itemsGrid.Columns[2].Name = "Item Description";
            itemsGrid.Columns[3].Name = "Size";
            itemsGrid.Columns[4].Name = "Qty";
            itemsGrid.Columns[5].Name = "MRP";
            itemsGrid.Columns[6].Name = "Disc %";
            itemsGrid.Columns[7].Name = "Disc Amt";
            itemsGrid.Columns[8].Name = "Net Amount";
            itemsGrid.Columns[9].Name = "Salesman";

            itemsGrid.Columns[0].Width = 45;
            itemsGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            itemsGrid.Columns[1].Width = 115;
            itemsGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            itemsGrid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            itemsGrid.Columns[3].Width = 60;
            itemsGrid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            itemsGrid.Columns[4].Width = 50;
            itemsGrid.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            itemsGrid.Columns[5].Width = 75;
            itemsGrid.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            itemsGrid.Columns[5].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
            itemsGrid.Columns[6].Width = 65;
            itemsGrid.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            itemsGrid.Columns[6].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
            itemsGrid.Columns[7].Width = 75;
            itemsGrid.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            itemsGrid.Columns[7].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
            itemsGrid.Columns[8].Width = 90;
            itemsGrid.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            itemsGrid.Columns[8].DefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            itemsGrid.Columns[8].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
            itemsGrid.Columns[9].Width = 100;
            itemsGrid.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void LoadBillData()
        {
            billTitleLabel.Text = sourceName + " - Bill #" + voucherNo;
            this.Text = sourceName + " Bill Details - #" + voucherNo;
            dateLabel.Text = "Date: " + dateStr + (string.IsNullOrEmpty(timeStr) ? "" : "  |  Time: " + timeStr);
            customerLabel.Text = "Customer: " + (string.IsNullOrEmpty(customerName) ? "Cash Sale" : customerName);
            mobileLabel.Text = "Mobile: " + (string.IsNullOrEmpty(mobile) ? "-" : mobile);

            if (string.IsNullOrEmpty(salesID))
            {
                return;
            }

            DataSet ds = DBHandler.GetDetailedBill(serverName, dbname, salesID);

            double totalMrp = 0;
            double totalDisc = 0;
            double totalNet = 0;
            double totalQty = 0;
            double cashAmt = 0;
            double cardAmt = 0;

            if (ds.Tables.Contains("Header") && ds.Tables["Header"].Rows.Count > 0)
            {
                DataRow headerRow = ds.Tables["Header"].Rows[0];
                if (headerRow["NetAmt"] != DBNull.Value) double.TryParse(headerRow["NetAmt"].ToString(), out totalNet);
                if (headerRow["CashAmt"] != DBNull.Value) double.TryParse(headerRow["CashAmt"].ToString(), out cashAmt);
                if (headerRow["CardAmt"] != DBNull.Value) double.TryParse(headerRow["CardAmt"].ToString(), out cardAmt);
                if (headerRow["TotalQty"] != DBNull.Value) double.TryParse(headerRow["TotalQty"].ToString(), out totalQty);
                if (headerRow["AccountName"] != DBNull.Value && !string.IsNullOrEmpty(headerRow["AccountName"].ToString()))
                {
                    customerLabel.Text = "Customer: " + headerRow["AccountName"].ToString();
                }
                if (headerRow["MobileNo"] != DBNull.Value && !string.IsNullOrEmpty(headerRow["MobileNo"].ToString()))
                {
                    mobileLabel.Text = "Mobile: " + headerRow["MobileNo"].ToString();
                }
            }

            itemsGrid.Rows.Clear();

            if (ds.Tables.Contains("Items"))
            {
                double calculatedQty = 0;
                foreach (DataRow row in ds.Tables["Items"].Rows)
                {
                    double mrp = 0;
                    double discPrc = 0;
                    double discAmt = 0;
                    double netAmt = 0;
                    double qty = 1;

                    if (row["MRP"] != DBNull.Value) double.TryParse(row["MRP"].ToString(), out mrp);
                    if (row["DiscPrc"] != DBNull.Value) double.TryParse(row["DiscPrc"].ToString(), out discPrc);
                    if (row["DiscAmt"] != DBNull.Value) double.TryParse(row["DiscAmt"].ToString(), out discAmt);
                    if (row["NetAmt"] != DBNull.Value) double.TryParse(row["NetAmt"].ToString(), out netAmt);
                    if (row["Qty"] != DBNull.Value) double.TryParse(row["Qty"].ToString(), out qty);

                    totalMrp += (mrp * qty);
                    totalDisc += discAmt;
                    calculatedQty += qty;

                    string discPrcStr = discPrc > 0 ? discPrc.ToString("0.#") + "%" : "-";
                    string discAmtStr = discAmt > 0 ? "₹" + discAmt.ToString("N0") : "-";

                    itemsGrid.Rows.Add(
                        row["SrNo"],
                        row["Barcode"],
                        row["ItemName"],
                        row["Size"],
                        qty.ToString("0.#"),
                        "₹" + mrp.ToString("N0"),
                        discPrcStr,
                        discAmtStr,
                        "₹" + netAmt.ToString("N0"),
                        row["Salesman"]
                    );
                }

                if (totalQty == 0) totalQty = calculatedQty;
                if (totalNet == 0 && ds.Tables["Items"].Rows.Count > 0)
                {
                    foreach (DataGridViewRow r in itemsGrid.Rows)
                    {
                        // Fallback net calculation if needed
                    }
                }
            }

            totalQtyLabel.Text = "Total Items / Qty: " + totalQty.ToString("0.#");
            totalMrpLabel.Text = "Total MRP: ₹" + totalMrp.ToString("N0");
            totalDiscLabel.Text = "Total Disc: ₹" + totalDisc.ToString("N0");
            netAmountLabel.Text = "Net Amount: ₹" + totalNet.ToString("N0");
            paymentModeLabel.Text = "Cash: ₹" + cashAmt.ToString("N0") + "  |  Card: ₹" + cardAmt.ToString("N0");
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BillDetailDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
