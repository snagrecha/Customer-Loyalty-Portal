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
    public partial class UpdateBillDialog : Form
    {
        string machine = System.Environment.MachineName;

        public UpdateBillDialog()
        {
            InitializeComponent();
        }

        public string CreateClause()
        {
            String clause = "";
            String source = "";
            String billNo = billNoTextBox.Text;
            String billDate = dateTimePicker1.Value.Date.ToString("MM/dd/yyyy") ;

            if (phCheckBox.Checked == true && juniorCheckBox.Checked == true)
            {
                source = "";
            }

            else if (phCheckBox.Checked == true)
            {
                source = "PH";
            }
            else if (juniorCheckBox.Checked == true)
            {
                source = "Junior";
            }

            clause += "WHERE BillDate = '" + billDate + "'";

            if (source.Length > 0) clause += " AND Source = '" + source + "'";

            if (billNo.Length > 0) clause += "AND BillNo = '" + billNo + "'";

            clause += " ORDER BY BillNo DESC";

            return clause;
        }

        public void UpdateHomeGrid(DataTable dt)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 8;
            dataGridView1.Columns[0].Name = "Bill No";
            dataGridView1.Columns[1].Name = "Mobile";
            //dataGridView1.Columns[0].Name = "Name";
            dataGridView1.Columns[2].Name = "Bill Date";
            dataGridView1.Columns[3].Name = "Points";
            dataGridView1.Columns[4].Name = "Source";
            dataGridView1.Columns[5].Name = "Points Redeemed";
            dataGridView1.Columns[6].Name = "Year";
            dataGridView1.Columns[7].Name = "Daybook";
            //dataGridView1.Columns[6].Name = "Match";
            
            dataGridView1.Columns[6].Visible = false;
            dataGridView1.Columns[7].Visible = false;

            foreach (DataRow row in dt.Rows)
            {
                dataGridView1.Rows.Add(row["BillNo"].ToString(), row["Mobile"].ToString(), row["BillDate"].ToString(), row["Points"].ToString(), row["Source"], row["PointsRedeemed"], row["Year"], row["BookCode"]);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            String clause = CreateClause();
            DataTable dt = DBHandler.SelectQueryOnTable("BillDetails","*" , clause);

            UpdateHomeGrid(dt);
        }

        private void checkBoxCheckedChanged(object sender, EventArgs e)
        {
            String clause = CreateClause();
            DataTable dt = DBHandler.SelectQueryOnTable("BillDetails", "*", clause);

            UpdateHomeGrid(dt);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            String oldMobile = dataGridView1.Rows[rowIndex].Cells["Mobile"].Value.ToString();
            String billNo = dataGridView1.Rows[rowIndex].Cells["Bill No"].Value.ToString();
            oldNoTextBox.Text = oldMobile;
            billNoLabel.Text = billNo;
            nameLabel.Text = DBHandler.GetCustomerName(oldMobile);
        }

        public void VerifyBalance()
        {
            DataTable dt = DBHandler.GetBalanceDiscrepencies();

            foreach (DataRow row in dt.Rows)
            {
                String mobile = row["Mobile"].ToString();
                String actualBalance = row["ActualBalance"].ToString();
                String balance = row["Balance"].ToString();
                //MessageBox.Show(mobile + " " + actualBalance + " " + balance);

                DBHandler.UpdateBalance(mobile, actualBalance);
                DBHandler.AddTransaction("Overview", machine + "-App", "Corrected Balance", balance, actualBalance, mobile);
            }
        }

        private void changeNumberButton_Click(object sender, EventArgs e)
        {
            String newMobile = newNumberTextBox.Text;
            String oldMobile = oldNoTextBox.Text;
            int n = 0;
            bool isNumeric = int.TryParse(newMobile, out n);
            
            int rowIndex = dataGridView1.CurrentCell.RowIndex;
            String bookcode = dataGridView1.Rows[rowIndex].Cells["Daybook"].Value.ToString();
            String billNo = dataGridView1.Rows[rowIndex].Cells["Bill No"].Value.ToString();
            String source = dataGridView1.Rows[rowIndex].Cells["Source"].Value.ToString();
            String year = dataGridView1.Rows[rowIndex].Cells["Year"].Value.ToString();


            if (newMobile.Length == 10)
            {
                bool exists = DBHandler.CheckIfMemberExists(newMobile);
                LogWriter log = new LogWriter("Updating Mobile Number " + DateTime.Now.ToString());

                if (exists)
                {
                    DBHandler.UpdateMobile(oldMobile, newMobile, bookcode, billNo, source, year);
                    DBHandler.AddTransaction("BillDetails", machine + "-App", "Updated Mobile No", oldMobile + "- Bill No: " + billNo, newMobile + "- Bill No: " + billNo, newMobile);
                    log.LogWrite("Updated Mobile Number. Old Mobile: " + oldMobile + " New Mobile: " + newMobile + " Bill No: " + billNo + " Source: " + source + " Year: " + year);

                    int oldBal = DBHandler.GetBalance(oldMobile);
                    int newBal = DBHandler.GetBalance(newMobile);


                    //VerifyBalance();
                    log.LogWrite("Verified Balance");
                    MessageBox.Show("Number successfully updated for Bill No: " + billNo + ".\nNew Balance " + oldMobile + ": " + DBHandler.GetBalance(oldMobile) + "\nNew Balance " + newMobile + ": " + DBHandler.GetBalance(newMobile));
                }

                else
                {
                    String name = nameLabel.Text;
                    DialogResult dialogresult = MessageBox.Show("Member does not exist! Create new account?", "Add Account", MessageBoxButtons.YesNo);
                    if (dialogresult == DialogResult.Yes)
                    {
                        DBHandler.InsertIntoTable("Overview", "", "'" + newMobile + "', '" + name + "', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP");
                        DBHandler.UpdateMobile(oldMobile, newMobile, bookcode, billNo, source, year);
                        DBHandler.AddTransaction("BillDetails", machine + "-App", "Updated Mobile No", oldMobile + "- Bill No: " + billNo, newMobile + "- Bill No: " + billNo, newMobile);
                        log.LogWrite("Updated Mobile Number. Old Mobile: " + oldMobile + " New Mobile: " + newMobile + " Bill No: " + billNo + " Source: " + source + " Year: " + year);

                        VerifyBalance();
                        log.LogWrite("Verified Balance");
                        MessageBox.Show("Number successfully updated for Bill No: " + billNo + ".\nNew Balance " + oldMobile + ": " + DBHandler.GetBalance(oldMobile) + "\nNew Balance " + newMobile + ": " + DBHandler.GetBalance(newMobile));
                    }
                }
            }

            MessageBox.Show("Number successfully updated for Bill No: " + billNo + ".\nNew Balance " + oldMobile + ": " + DBHandler.GetBalance(oldMobile) + "\nNew Balance " + newMobile + ": " + DBHandler.GetBalance(newMobile));
        }

    }
}
