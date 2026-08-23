//using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Printing;

namespace Customer_Loyalty_Portal
{
    public partial class Home : Form
    {
        //Server Name of CustomerLoyaltyPortal App (HP-PC\SQL2008, LENOVO-PC\SQLEXPRESS etc.)
        //public static string hostServerName = "SNEH-CUSTOM";
        public static string hostServerName = ConfigurationManager.AppSettings["hostServerName"];

        //public static string tphServerName = "SNEH-CUSTOM";
        public static string tphServerName = ConfigurationManager.AppSettings["tphServerName"];

        //public static string jrServerName = "HP-PC\\SQL2008";
        public static string jrServerName = ConfigurationManager.AppSettings["jrServerName"];

        public static string jrSourceName = ConfigurationManager.AppSettings["jrSourceName"];

        public static string phSourceName = ConfigurationManager.AppSettings["phSourceName"];

        //Database Name of CustomerLoyaltyPortal App
        //public static string hostDBName = "CustomerLoyalty";
        public static string hostDBName = ConfigurationManager.AppSettings["hostDBName"];

        //public static string tphDBName = "GRExtreme_PantHouseJ";
        public static string tphDBName = ConfigurationManager.AppSettings["tphDBName"];

        //public static string jrDBName = "G_PANTHOUSE";
        public static string jrDBName = ConfigurationManager.AppSettings["jrDBName"];

        //public static string tphMachineName = "SNEH-CUSTOM";
        public static string tphMachineName = ConfigurationManager.AppSettings["tphMachineName"];

        //public static string jrMachineName = "HP-PC";
        public static string jrMachineName = ConfigurationManager.AppSettings["jrMachineName"];

        //public static string year = "2324";
        public static string year = ConfigurationManager.AppSettings["year"];

        //public static string phFinYearID = "8";
        public static string phFinYearID = ConfigurationManager.AppSettings["phFinYearID"];

        //public static string jrFinYearID = "6";
        public static string jrFinYearID = ConfigurationManager.AppSettings["jrFinYearID"];

        string machine = System.Environment.MachineName;
        //string machine = "SNEH-CUSTOM";

        string version = Application.ProductVersion;
        Dictionary<String, int> itemValue = new Dictionary<string, int>();
        //Dictionary<String, int> itemValue2 = new Dictionary<string, int>();
        Dictionary<String, String> dbnameDict = new Dictionary<string, string>();
        Dictionary<String, String> dbnameReverseDict = new Dictionary<string, string>();
        Dictionary<String, String> servernameDict = new Dictionary<string, string>();
        Dictionary<String, String> machineDbNameDict = new Dictionary<string, string>();
        Dictionary<String, String> machineServerNameDict = new Dictionary<string, string>();

        List<String> lastUpdatedList = new List<string>();
        Dictionary<string, SalesmanCommissionData> currentCommissionStats = new Dictionary<string, SalesmanCommissionData>();
        DataTable phNewBills = new DataTable();
        DataTable jrNewBills = new DataTable();
        List<List<String>> updationList= new List<List<String>>();
        List<List<String>> modifiedList = new List<List<string>>();



        public string paytmId = "";
        public string bajajId = "";
        public string cardId = "";

        float balance = 0;

        public void UpdateItemValueDictionary()
        {
            itemValue.Add("2450", 3000);
            itemValue.Add("2451", 6000);
            itemValue.Add("2811", 10000);
            itemValue.Add("2837", 5000);
            itemValue.Add("2838", 8000);
            itemValue.Add("2839", 15000);
            itemValue.Add("1282", 3000);
            itemValue.Add("1284", 6000);
            itemValue.Add("1311", 10000);
            itemValue.Add("1283", 5000);
            itemValue.Add("1414", 8000);
            itemValue.Add("1415", 15000);
            itemValue.Add("1126", 3000);
            itemValue.Add("1127", 3000);

        }

        public void EnableRemoveButtons(List<Tuple<String, String>> bagsGivenList)
        {
            foreach (Tuple<String, String> bag in bagsGivenList)
            {
                if (bag.Item2 == "3000") remove3.Enabled = true;
                if (bag.Item2 == "5000") remove5.Enabled = true;
                if (bag.Item2 == "8000") remove8.Enabled = true;
                if (bag.Item2 == "10000") remove10.Enabled = true;
                if (bag.Item2 == "15000") remove15.Enabled = true;
            }
        }

        public static List<Tuple<String, String>> ParseBagsGiven(String bagsGiven)
        {
            List<Tuple<String, String>> bags = new List<Tuple<String, String>>();

            String[] bagsSplit = bagsGiven.Split('x');

            for (int x = 0; x < bagsSplit.Length - 1; x++ )
            {
                String qty = bagsSplit[x].Substring(bagsSplit[x].Length-1, 1);
                String points = "";
                
                if (x == bagsSplit.Length - 2) points = bagsSplit[x + 1].Substring(0, bagsSplit[x + 1].Length);
                else points = bagsSplit[x + 1].Substring(0, bagsSplit[x + 1].Length-1);

                bags.Add(Tuple.Create(qty, points));
                //Console.WriteLine("###" + bagsSplit[x] + "***");
            }

            return bags;
        }

        public void UpdateHomeGrid(String mobile)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.ColumnCount = 5;
            dataGridView1.ReadOnly = false;
            dataGridView1.Columns[0].Name = "Bill No";
            dataGridView1.Columns[1].Name = "Bill Date";
            dataGridView1.Columns[2].Name = "Points";
            dataGridView1.Columns[3].Name = "Source";
            dataGridView1.Columns[4].Name = "Points Redeemed";

            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;

            dataGridView1.Columns[0].Width = 85;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Width = 95;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            dataGridView1.Columns[3].Width = 75;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[4].DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);

            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            chk.Name = "Update";
            chk.Width = 55;
            chk.ReadOnly = false;         
            dataGridView1.Columns.Add(chk);

            StyleGridCommon(dataGridView1);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            if (mobile.Length == 10)
            {
                DataTable dt = DBHandler.SelectQueryOnTable("BillDetails", "*", "WHERE Mobile='" + mobile + "' ORDER BY BillDate");
                DateTime date;
                float totalPoints = 0;
                float redeemedPoints = 0;

                foreach (DataRow row in dt.Rows)
                {
                    date = (DateTime)row["BillDate"];
                    //itemNoDictionary.Add(row["Item"].ToString(),  int.Parse(row["ItemNo"].ToString()));
                    totalPoints += float.Parse(row["Points"].ToString());
                    dataGridView1.Rows.Add(row["BillNo"].ToString(), date.Date.ToString("dd/MM/yyyy"), row["Points"], row["Source"], row["BagsGiven"]);
                }

                redeemedPoints = totalPoints - balance;
                dataGridView1.Rows.Add("", "Total Points:", totalPoints.ToString(), "Total Redeemed:", redeemedPoints.ToString());
            }
        }

        public void RefreshLastUpdated()
        {
            lastUpdatedList.Clear();

            DataTable dt = DBHandler.SelectQueryOnTable("LastUpdate");

            foreach (DataRow row in dt.Rows)
            {
                lastUpdatedList.Add(row["LastUpdated"].ToString());
                lastUpdatedList.Add(row[phSourceName].ToString());
                lastUpdatedList.Add(row[jrSourceName].ToString());
                //MessageBox.Show(row["PH"].ToString() + row["Junior"].ToString());
            }

            salesDate.Text = lastUpdatedList[0];
            phSales.Text = lastUpdatedList[1];
            juniorSales.Text = lastUpdatedList[2];

            exchDate.Text = lastUpdatedList[3];
            phExch.Text = lastUpdatedList[4];
            juniorExchange.Text = lastUpdatedList[5];
        }

        public int UpdateBillDataGrid(DataTable dt, DataTable dt2)
        {
            dataGridViewPH.Rows.Clear();
            dataGridViewPH.Columns.Clear();

            dataGridViewPH.ColumnCount = 6;
            dataGridViewPH.Columns[0].Name = "Date";
            dataGridViewPH.Columns[1].Name = "Time";
            dataGridViewPH.Columns[2].Name = "Bill No";
            dataGridViewPH.Columns[3].Name = "Mobile";
            dataGridViewPH.Columns[4].Name = "Name";
            dataGridViewPH.Columns[5].Name = "Amount";

            DataGridViewLinkColumn phLinkCol = new DataGridViewLinkColumn();
            phLinkCol.Name = "Action";
            phLinkCol.HeaderText = "Bill";
            phLinkCol.Text = "View";
            phLinkCol.UseColumnTextForLinkValue = true;
            phLinkCol.LinkColor = Color.FromArgb(13, 110, 253);
            phLinkCol.ActiveLinkColor = Color.FromArgb(11, 94, 215);
            phLinkCol.VisitedLinkColor = Color.FromArgb(13, 110, 253);
            phLinkCol.LinkBehavior = LinkBehavior.HoverUnderline;
            phLinkCol.TrackVisitedState = false;
            phLinkCol.Width = 48;
            phLinkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            phLinkCol.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewPH.Columns.Add(phLinkCol);

            DataGridViewTextBoxColumn phSalesIdCol = new DataGridViewTextBoxColumn();
            phSalesIdCol.Name = "SalesID";
            phSalesIdCol.Visible = false;
            dataGridViewPH.Columns.Add(phSalesIdCol);

            dataGridViewPH.Columns[0].Width = 75;
            dataGridViewPH.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewPH.Columns[1].Width = 68;
            dataGridViewPH.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewPH.Columns[2].Width = 65;
            dataGridViewPH.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewPH.Columns[3].Width = 88;
            dataGridViewPH.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewPH.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewPH.Columns[5].Width = 75;
            dataGridViewPH.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewPH.Columns[5].DefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            dataGridViewPH.Columns[5].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);

            StyleGridCommon(dataGridViewPH);
            dataGridViewPH.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewPH.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            foreach(DataRow row in dt.Rows)
            {
                string timeStr = "";
                if (row.Table.Columns.Contains("AddDate") && row["AddDate"] != DBNull.Value && row["AddDate"] != null)
                {
                    DateTime dtVal = Convert.ToDateTime(row["AddDate"]);
                    timeStr = dtVal.ToString("hh:mm tt");
                }
                else if (row["VoucherDate"] != DBNull.Value && row["VoucherDate"] != null)
                {
                    DateTime dtVal = Convert.ToDateTime(row["VoucherDate"]);
                    if (dtVal.TimeOfDay.TotalSeconds > 0)
                    {
                        timeStr = dtVal.ToString("hh:mm tt");
                    }
                }
                string dateStr = ((DateTime)row["VoucherDate"]).ToString("dd-MM-yyyy");
                string salesID = row.Table.Columns.Contains("SalesID") ? row["SalesID"].ToString() : "";

                dataGridViewPH.Rows.Add(dateStr, timeStr, row["VoucherNo"], row["MobileNo"], row["AccountName"], row["NetAmt"], "View", salesID);
            }

            dataGridViewJunior.Rows.Clear();
            dataGridViewJunior.Columns.Clear();

            dataGridViewJunior.ColumnCount = 6;
            dataGridViewJunior.Columns[0].Name = "Date";
            dataGridViewJunior.Columns[1].Name = "Time";
            dataGridViewJunior.Columns[2].Name = "Bill No";
            dataGridViewJunior.Columns[3].Name = "Mobile";
            dataGridViewJunior.Columns[4].Name = "Name";
            dataGridViewJunior.Columns[5].Name = "Amount";

            DataGridViewLinkColumn jrLinkCol = new DataGridViewLinkColumn();
            jrLinkCol.Name = "Action";
            jrLinkCol.HeaderText = "Bill";
            jrLinkCol.Text = "View";
            jrLinkCol.UseColumnTextForLinkValue = true;
            jrLinkCol.LinkColor = Color.FromArgb(13, 110, 253);
            jrLinkCol.ActiveLinkColor = Color.FromArgb(11, 94, 215);
            jrLinkCol.VisitedLinkColor = Color.FromArgb(13, 110, 253);
            jrLinkCol.LinkBehavior = LinkBehavior.HoverUnderline;
            jrLinkCol.TrackVisitedState = false;
            jrLinkCol.Width = 48;
            jrLinkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            jrLinkCol.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewJunior.Columns.Add(jrLinkCol);

            DataGridViewTextBoxColumn jrSalesIdCol = new DataGridViewTextBoxColumn();
            jrSalesIdCol.Name = "SalesID";
            jrSalesIdCol.Visible = false;
            dataGridViewJunior.Columns.Add(jrSalesIdCol);

            dataGridViewJunior.Columns[0].Width = 75;
            dataGridViewJunior.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewJunior.Columns[1].Width = 68;
            dataGridViewJunior.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewJunior.Columns[2].Width = 65;
            dataGridViewJunior.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewJunior.Columns[3].Width = 88;
            dataGridViewJunior.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewJunior.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewJunior.Columns[5].Width = 75;
            dataGridViewJunior.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewJunior.Columns[5].DefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            dataGridViewJunior.Columns[5].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);

            StyleGridCommon(dataGridViewJunior);
            dataGridViewJunior.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridViewJunior.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            foreach (DataRow row in dt2.Rows)
            {
                string timeStr2 = "";
                if (row.Table.Columns.Contains("AddDate") && row["AddDate"] != DBNull.Value && row["AddDate"] != null)
                {
                    DateTime dtVal = Convert.ToDateTime(row["AddDate"]);
                    timeStr2 = dtVal.ToString("hh:mm tt");
                }
                else if (row["VoucherDate"] != DBNull.Value && row["VoucherDate"] != null)
                {
                    DateTime dtVal = Convert.ToDateTime(row["VoucherDate"]);
                    if (dtVal.TimeOfDay.TotalSeconds > 0)
                    {
                        timeStr2 = dtVal.ToString("hh:mm tt");
                    }
                }
                string dateStr2 = ((DateTime)row["VoucherDate"]).ToString("dd-MM-yyyy");
                string salesID2 = row.Table.Columns.Contains("SalesID") ? row["SalesID"].ToString() : "";

                dataGridViewJunior.Rows.Add(dateStr2, timeStr2, row["VoucherNo"], row["MobileNo"], row["AccountName"], row["NetAmt"], "View", salesID2);
            }
            return 1;
        }

        public void UpdateListOfBills(String phone, String dbCode, String billNo, String source, String points, String recon, String year, String bagsGiven, String date, String name, String pointsRedeemed, String co_year, String salesID)
        {
            List<String> temp = new List<string>();

            temp.Add(phone);           //[0]
            temp.Add(dbCode);          //[1] 
            temp.Add(billNo);          //[2]
            temp.Add(source);          //[3]
            temp.Add(points);          //[4] 
            temp.Add(recon);           //[5] 
            temp.Add(year);            //[6]
            temp.Add(bagsGiven);       //[7]
            temp.Add(date);            //[8]
            temp.Add(name);            //[9] 
            temp.Add(pointsRedeemed);  //[10]
            temp.Add(co_year);         //[11]
            temp.Add(salesID);         //[12]

            updationList.Add(temp);
        }

        public void GetNewBills()
        {
            int oldListSize = 0;
            //phNewBills = DBHandler.GetPendingBills(lastUpdatedList[1], "GRExtreme_PantHouseJ", "401", "2019-2020", "LENOVO-PC\\SQL2008");
            phNewBills = DBHandler.GetPendingBills(lastUpdatedList[1], tphDBName, "401", "2019-2020", tphServerName, phFinYearID);

            if (phNewBills.Rows.Count > 0)
            {
                oldListSize = phNewBills.Rows.Count;
                lastUpdatedList[1] = phNewBills.Rows[phNewBills.Rows.Count - 1]["VoucherNo"].ToString();
            }

            foreach (DataRow row in phNewBills.Rows)
            {
                DateTime date = (DateTime)row["VoucherDate"];
                String voucherDate = date.ToString("MM-dd-yyyy");
                UpdateListOfBills(row["MobileNo"].ToString(), row["DayBookID"].ToString(), row["VoucherNo"].ToString(), phSourceName, row["NetAmt"].ToString(), "1", year, "", voucherDate, row["AccountName"].ToString(), "", row["FinYearID"].ToString(), row["SalesID"].ToString());
             }

            jrNewBills = DBHandler.GetPendingBills(lastUpdatedList[2], jrDBName, "401", "2019-2020", jrServerName, jrFinYearID);
            if (jrNewBills.Rows.Count > 0)
            {
                oldListSize = jrNewBills.Rows.Count;
                lastUpdatedList[2] = jrNewBills.Rows[jrNewBills.Rows.Count - 1]["VoucherNo"].ToString();
            }

            foreach (DataRow row in jrNewBills.Rows)
            {
                DateTime date = (DateTime)row["VoucherDate"];
                String voucherDate = date.ToString("MM-dd-yyyy");
                UpdateListOfBills(row["MobileNo"].ToString(), row["DayBookID"].ToString(), row["VoucherNo"].ToString(), jrSourceName, row["NetAmt"].ToString(), "1", year, "", voucherDate, row["AccountName"].ToString(), "", row["FinYearID"].ToString(), row["SalesID"].ToString());
            }
        }

        public void GetBagsGiven()
        {
            foreach (List<String> bill in updationList)
            {
                String bag = "";
                int pointsRedeemed = 0;
                DataTable dt = DBHandler.GetBagsGiven(bill[12], dbnameDict[bill[3]], bill[1], bill[11], servernameDict[bill[3]]);
                foreach (DataRow row in dt.Rows)
                {
                    int qty = int.Parse(row["Qty"].ToString());
                    int value = itemValue[row["ItemID"].ToString()];
                    bag += qty.ToString() + "x" + value.ToString();
                    pointsRedeemed += qty * value;
                }

                bill[7] = bag;
                bill[10] = pointsRedeemed.ToString();
                //if(bag.Length>0) MessageBox.Show(bill[2] + " " + bag);
            }
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

            //Remove balance from old number in case of mobile number updation
            DBHandler.CorrectBalanceForUpdatedNumber();
        }

        public void UpdateBalance()
        {
            foreach (List<String> bill in updationList)
            {
                //MessageBox.Show("Inside Update Balance: " + bill[0]);
                int old_bal = 0;
                int new_bal = 0;

                bool customerExists = DBHandler.CheckIfMemberExists(bill[0]);
                bool billExists = DBHandler.CheckIfBillExists(bill[1], bill[2], bill[3], bill[6]);
                
                //IF BILL DOESN'T ALREADY EXIST INSERT THE BILL IN BillDetails
                if (bill[0].Length > 0 && !billExists)
                {
                    DBHandler.InsertIntoTable("BillDetails", "", "'" + bill[0] + "','" + bill[1] + "','" + bill[2] + "','" + bill[3] + "'," + bill[4] + "," + bill[5] + "," + bill[6] + ",'" + bill[7] + "','" + bill[8] + "', '" + bill[11] + "', " + bill[10] + ",CURRENT_TIMESTAMP, CURRENT_TIMESTAMP");
                    DBHandler.AddTransaction("BillDetails", machine + "-Bill", "Added new bill", "", bill[0] + "," + bill[1] + "," + bill[2] + "," + bill[3] + "," + bill[4] + "," + bill[5] + "," + bill[6] + "," + bill[7] + "," + bill[8] + "," + bill[11] + "," + bill[10], bill[0]);
                }

                //IF BILL ALREADY EXISTS, UPDATE THE BILL
                else
                {
                    DBHandler.UpdatePoints(bill[0], bill[4], bill[8], bill[2], bill[3]);
                    DBHandler.AddTransaction("BillDetails", machine + "-Bill", "Updated Existing Bill", bill[0] + "," + bill[1] + "," + bill[2] + "," + bill[3] + "," + bill[4] + "," + bill[5] + "," + bill[6] + "," + bill[7] + "," + bill[8] + "," + bill[11] + "," + bill[10], bill[0] + "," + bill[1] + "," + bill[2] + "," + bill[3] + "," + bill[4] + "," + bill[5] + "," + bill[6] + "," + bill[7] + "," + bill[8] + "," + bill[11] + "," + bill[10], bill[0]);
                }

                //IF CUSTOMER EXISTS, GET OLD BALANCE
                if (customerExists)
                {
                    //DataTable dt = DBHandler.SelectQueryOnTable("Overview", "*", "WHERE Mobile = '" + bill[0] + "'");
                    DataTable dt = DBHandler.SelectQueryOnTable("BillDetails", "SUM(CONVERT(int,Points)) - SUM(CONVERT(int,PointsRedeemed)) as ActualBalance", "WHERE Mobile = '" + bill[0] + "'");
                    foreach (DataRow row in dt.Rows)
                    {
                        //TBD***************************
                        int.TryParse(row["ActualBalance"].ToString(), out old_bal);
                        break;
                    }

                }

                //IF CUSTOMER DOESN'T EXIST ADD NEW CUSTOMER CUSTOMER  
                else
                {
                    if (bill[0].Length == 10)
                    {
                        DBHandler.InsertIntoTable("Overview", "", "'" + bill[0] + "', '" + bill[9] + "', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP");
                        DBHandler.AddTransaction("Overview", machine + "-Bill", "Added new Customer. Mobile: " + bill[0] + " Name: " + bill[9], "", bill[0] + " " + bill[9] + " 0", bill[0]);
                    }
                }

                //UPDATE CUSTOMER TOTAL POINTS
                if (!billExists) new_bal = old_bal + int.Parse(bill[4]) - int.Parse(bill[10]);
                else new_bal = old_bal; //SINCE OLD BAL WILL HAVE THE UPDATED BILL DETAILS
                DBHandler.UpdateBalance(bill[0], new_bal.ToString());
                DBHandler.AddTransaction("Overview", machine + "-Bill", "Updated Balance for " + bill[0], old_bal.ToString(), new_bal.ToString(), bill[0]);
                //MessageBox.Show(bill[1] + " " + bill[2] + " " + bill[3] + " " + bill[7]);
            }
            updationList.Clear();
        }

        public int UpdateLastUpdated()
        {
            
            String phSale = DBHandler.GetLastBill(phSourceName, "401", hostServerName, hostDBName, year);
            String phExch = DBHandler.GetLastBill(phSourceName, "403", hostServerName, hostDBName, year);
            String juniorSale = DBHandler.GetLastBill(jrSourceName, "401", hostServerName, hostDBName, year);
            String juniorExch = DBHandler.GetLastBill(jrSourceName, "403", hostServerName, hostDBName, year);

            DBHandler.AddTransaction("LastUpdate", machine + "-App", "Updated LastUpdated", "PHSale = " + lastUpdatedList[1] + ", JuniorSale = " + lastUpdatedList[2] + ", PHExch = " + lastUpdatedList[4] + ", JuniorExch = " + lastUpdatedList[5], "PHSale = " + phSale + ", JuniorSale = " + juniorSale + ", PHExch = " + phExch + ", JuniorExch = " + juniorExch, "");

            lastUpdatedList[1] = phSale;
            lastUpdatedList[2] = juniorSale;
            lastUpdatedList[4] = phExch;
            lastUpdatedList[5] = juniorExch;

            DBHandler.UpdateLastUpdated(lastUpdatedList, phSourceName, jrSourceName);

            return 1;
        }

        //public Home(SplashScreen splashScreen)
        public void UpdateCashGrid()
        {
            cashGridView.Rows.Clear();
            int x2000 = 0, x500 = 0, x200 = 0, x100 = 0, x50 = 0, x20 = 0, x10 = 0, x5 = 0;

            DataTable dt = DBHandler.SelectQueryOnTable("DailyCash", "*", "WHERE CAST(Date AS DATE) = '" + dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd") + "' AND Machine = '" + machine + "'");

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[dt.Rows.Count - 1];
                int.TryParse(row["x2000"].ToString(), out x2000);
                int.TryParse(row["x500"].ToString(), out x500);
                int.TryParse(row["x200"].ToString(), out x200);
                int.TryParse(row["x100"].ToString(), out x100);
                int.TryParse(row["x50"].ToString(), out x50);
                int.TryParse(row["x20"].ToString(), out x20);
                int.TryParse(row["x10"].ToString(), out x10);
                int.TryParse(row["x5"].ToString(), out x5);
            }

            cashGridView.Rows.Add("2000", x2000.ToString(), (2000 * x2000).ToString());
            cashGridView.Rows.Add("500", x500.ToString(), (500 * x500).ToString());
            cashGridView.Rows.Add("200", x200.ToString(), (200 * x200).ToString());
            cashGridView.Rows.Add("100", x100.ToString(), (100 * x100).ToString());
            cashGridView.Rows.Add("50", x50.ToString(), (50 * x50).ToString());
            cashGridView.Rows.Add("20", x20.ToString(), (20 * x20).ToString());
            cashGridView.Rows.Add("10", x10.ToString(), (10 * x10).ToString());
            cashGridView.Rows.Add("5", x5.ToString(), (5 * x5).ToString());
        }

        public void UpdateCreditGrid()
        {
            creditGridView.Rows.Clear();

            DataTable dt = DBHandler.SelectQueryOnTable("DailyTransactions", "*", "WHERE Machine = '" + machine + "' AND Type = 'CR' AND CAST(TransactionDate AS DATE) = '" + dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd") + "'");

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    creditGridView.Rows.Add(row["Particular"].ToString(), row["Amount"].ToString());
                }
            }
        }

        public void UpdateDebitGrid()
        {
            debitGridView.Rows.Clear();

            DataTable dt = DBHandler.SelectQueryOnTable("DailyTransactions", "*", "WHERE Machine = '" + machine + "' AND Type = 'DR' AND CAST(TransactionDate AS DATE) = '" + dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd") + "'");

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    debitGridView.Rows.Add(row["Particular"].ToString(), row["Amount"].ToString());
                }
            }
        }

        public int UpdateTotalCredit()
        {
            int total = 0;
            for (int i = 0; i < creditGridView.Rows.Count; i++)
            {
                if (creditGridView.Rows[i].Cells[1].Value != null)
                {
                    int value = 0;
                    int.TryParse(creditGridView.Rows[i].Cells[1].Value.ToString(), out value);
                    total += value;
                }
            }
            totalCreditLabel.Text = total.ToString();
            return total;
        }
        
        public int UpdateTotalDebit()
        {
            int total = 0;
            for (int i = 0; i < debitGridView.Rows.Count; i++)
            {
                if (debitGridView.Rows[i].Cells[1].Value != null)
                {
                    int value = 0;
                    int.TryParse(debitGridView.Rows[i].Cells[1].Value.ToString(), out value);
                    total += value;
                }
            }
            totalDebitLabel.Text = total.ToString();
            return total;
        }
        
        public int UpdateTotalCash()
        {
            int total = 0;
            if (cashGridView.Rows.Count > 0 && cashGridView.Rows[0] != null)
            {
                foreach (DataGridViewRow row in cashGridView.Rows)
                {
                    int value = 0;
                    int.TryParse(row.Cells[2].Value.ToString(), out value);
                    total += value;
                }
            }
            totalCashLabel.Text = total.ToString();
            return total;
        }

        public int UpdateCalculatedBalance()
        {
            int calculatedTotal =0, cash =0, debit =0, credit = 0, totalSale = 0, card = 0, debitSale = 0, balReceived_cash = 0, balReceived_card = 0, paytm = 0, bajaj = 0, crntIssued = 0, crntRecv = 0;
            int.TryParse(cashSaleTextBox.Text.ToString(), out cash);
            int.TryParse(totalCreditLabel.Text.ToString(), out credit);
            int.TryParse(totalDebitLabel.Text.ToString(), out debit);
            int.TryParse(totalSaleTextBox.Text.ToString(), out totalSale);
            int.TryParse(cardSaleTextBox.Text.ToString(), out card);
            int.TryParse(debitSaleTextBox.Text.ToString(), out debitSale);
            int.TryParse(balReceivedCashTextBox.Text.ToString(), out balReceived_cash);
            int.TryParse(balReceivedCardTextBox.Text.ToString(), out balReceived_card);
            int.TryParse(paytmTextBox.Text.ToString(), out paytm);
            int.TryParse(bajajTextBox.Text.ToString(), out bajaj);
            int.TryParse(crntIssuedTextBox.Text.ToString(), out crntIssued);
            int.TryParse(crntRecvTextBox.Text.ToString(), out crntRecv);

            //total = cash + credit - debit;
            calculatedTotal = totalSale + credit + balReceived_cash + crntIssued - card - debit - debitSale - paytm - bajaj - crntRecv;
            calculatedBalanceLabel.Text = calculatedTotal.ToString();

            return calculatedTotal;
        }

        public void CheckBalanceMatch()
        {
            int cashSale = 0; 
            int.TryParse(cashSaleTextBox.Text.ToString(), out cashSale);
            int totalCredit = UpdateTotalCredit();
            int totalDebit = UpdateTotalDebit();
            int totalCash = UpdateTotalCash();
            int calculatedBalance = UpdateCalculatedBalance();

            int difference = totalCash - calculatedBalance;

            if (difference == 0)
            {
                balanceMatchLabel.Text = "MATCHED ✓";
                balanceMatchLabel.ForeColor = Color.FromArgb(25, 135, 84);
            }
            else
            {
                balanceMatchLabel.Text = "Diff: " + difference.ToString("+#,##0;-#,##0;0");
                balanceMatchLabel.ForeColor = Color.FromArgb(220, 53, 69);
            }
        }

        public void InitializeDay()
        {
            //Replace HP-PC with machine****************************
            //DataTable dt = DBHandler.SelectQueryOnTable("DailyCash", "*", "WHERE Date = '" + DateTime.Now.Date.ToString("yyyy-MM-dd").ToString() + "' AND Machine = 'HP-PC' AND Initialised = '1'");
            DataTable dt = DBHandler.SelectQueryOnTable("DailyCash", "*", "WHERE Date = '" + DateTime.Now.Date.ToString("yyyy-MM-dd").ToString() + "' AND Machine = '" + machine + "' AND Initialised = '1'");

            if (dt.Rows.Count > 0)
            {

            }

            else
            {
                MessageBox.Show("Day Not Initialized");

                /*cashGridView.Rows.Add("2000", "0", "0");
                cashGridView.Rows.Add("500", "0", "0");
                cashGridView.Rows.Add("200", "0", "0");
                cashGridView.Rows.Add("100", "0", "0");
                cashGridView.Rows.Add("50", "0", "0");
                cashGridView.Rows.Add("20", "0", "0");
                cashGridView.Rows.Add("10", "0", "0");
                cashGridView.Rows.Add("5", "0", "0");
                */

                //Replace HP-PC with machine****************************
                //DBHandler.InitializeDailyCash(DateTime.Now.Date.ToString("yyyy-MM-dd").ToString(), "HP-PC", "0");
                //DBHandler.ClearDailyTransactions("HP-PC");
                DBHandler.InitializeDailyCash(DateTime.Now.Date.ToString("yyyy-MM-dd").ToString(), machine, "0");
                // DBHandler.ClearDailyTransactions(machine);

                OpeningCashDialog cashDialog = new OpeningCashDialog();
                
                if (cashDialog.ShowDialog(this) == DialogResult.OK)
                {
                    String openingCash = cashDialog.opBalDialogText.Text.ToString();
                    creditGridView.Rows.Add("Opening Balance", cashDialog.opBalDialogText.Text.ToString());
                    creditGridView.Rows[0].ReadOnly = true;

                    DBHandler.InitializeDailyCash(DateTime.Now.Date.ToString("yyyy-MM-dd").ToString(), machine, "1");
                    DBHandler.AddDailyTransactions(machine, "CR", "Opening Balance", openingCash, DateTime.Now.ToString("yyyy-MM-dd")); // FIXME CHANGE TO CURRENT DATE
                }
            }
        }

        public Home()
        
        {
            InitializeComponent();

            WriteToExcel.workingDirectory = Directory.GetCurrentDirectory() + "\\DailyBalanceExcel\\";
            //if (machine.Equals("SNEH-PC")) machine = jrMachineName;
            //if (machine.Equals("ACER-Laptop")) machine = jrMachineName;
            if (!(machine.Equals(jrMachineName) || machine.Equals(tphMachineName))) machine = jrMachineName;
            
            //if (machine.Equals("HP-PC"))
            if (machine.Equals(jrMachineName))
            {
                paytmId = "149545";
                bajajId = "149546";
                cardId = "165";
            }

            //else if (machine.Equals("LENOVO-PC"))
            else if (machine.Equals(tphMachineName))
            {
                paytmId = "219587";
                bajajId = "219588";
                cardId = "128";
            }

            LogWriter log = new LogWriter("Initializing App! " + DateTime.Now.ToString());

            //dbnameDict.Add("PH", "GRExtreme_PantHouseJ");
            dbnameDict.Add(phSourceName, tphDBName);

            //dbnameDict.Add("Junior", "G_PANTHOUSE");
            dbnameDict.Add(jrSourceName, jrDBName);

            //machineDbNameDict.Add("LENOVO-PC", "GRExtreme_PantHouseJ");
            machineDbNameDict.Add(tphMachineName, tphDBName);
            //machineDbNameDict.Add("HP-PC", jrDBName);
            machineDbNameDict.Add(jrMachineName, jrDBName);

            machineServerNameDict.Add(tphMachineName, tphServerName);
            machineServerNameDict.Add(jrMachineName, jrServerName);

            //dbnameReverseDict.Add("GRExtreme_PantHouseJ", "PH");
            dbnameReverseDict.Add(tphMachineName, phSourceName);
            //dbnameReverseDict.Add("G_PANTHOUSE", "Junior");
            dbnameReverseDict.Add(jrMachineName, jrSourceName);

            //servernameDict.Add("PH", "LENOVO-PC\\SQL2008");
            servernameDict.Add(phSourceName, tphServerName);
            //servernameDict.Add("Junior", "HP-PC\\SQL2008");
            servernameDict.Add(jrSourceName, jrServerName);

            UpdateDateLabel();
            StyleDailyBalanceGrids();
            SetupBagButtonStates();

            UpdateItemValueDictionary();
            log.LogWrite("Updating ItemValueDictionary");

            UpdateHomeGrid("");
            log.LogWrite("Updating Home Grid");

            InitializeDay();
            log.LogWrite("Initialising day for: " + machine);

            UpdateCashGrid();
            UpdateTotalCash();
            log.LogWrite("Updating Cash Grid");

            UpdateCreditGrid();
            UpdateTotalCredit();
            log.LogWrite("Updating Credit Grid");

            UpdateDebitGrid();
            UpdateTotalDebit();
            log.LogWrite("Updating Debit Grid");

            CheckBalanceMatch();
            log.LogWrite("Checking if balance Matches");

            UpdateCalculatedBalance();
            log.LogWrite("Updated Calculated Balance");
        
            RefreshLastUpdated();
            GetNewBills();
            log.LogWrite("Got New Bills!");
            GetBagsGiven();
            log.LogWrite("Got Bags Given!");
            UpdateBalance();
            log.LogWrite("Updated Balance!");

            VerifyBalance();
            log.LogWrite("Verified Balance");

            UpdateLastUpdated();
            log.LogWrite("Updated Last Updated!");

            RefreshLastUpdated();
            //GetNewBags();
            //UpdateNewBags();

            //DataTable dt = DBHandler.GetLastBills("LENOVO-PC\\SQL2008", "GRExtreme_PantHouseJ", "10", finYearID);
            DataTable dt = DBHandler.GetLastBills(tphServerName, tphDBName, "10", phFinYearID);
            //DataTable dt2 = DBHandler.GetLastBills("HP-PC\\SQL2008", "G_PANTHOUSE", "10", finYearID);
            DataTable dt2 = DBHandler.GetLastBills(jrServerName, jrDBName, "10", jrFinYearID);
            //label12.Text = "V " + version;
            UpdateBillDataGrid(dt, dt2);

            //splashScreen.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            String mobile = textBox1.Text.ToString();
            //float balance = 0;
            String name = "";
            remove3.Enabled = false;
            remove5.Enabled = false;
            remove8.Enabled = false;
            remove10.Enabled = false;
            remove15.Enabled = false;

            if (mobile.Length == 10)
            {
                label1.Show();
                label2.Show();
                nameLabel.Show();
                balanceLabel.Show();
                button3.Enabled = true;
                button5.Enabled = true;
                button8.Enabled = true;
                button10.Enabled = true;
                button15.Enabled = true;

                DataTable dt = DBHandler.SelectQueryOnTable("Overview", "*", "WHERE Mobile = '" + mobile + "'");

                foreach (DataRow row in dt.Rows)
                {
                    balance = float.Parse(row["Balance"].ToString());
                    name = row["Name"].ToString();
                }

                if (name.Length > 0)
                {
                    UpdateHomeGrid(mobile);
                    nameLabel.Text = name;
                    balanceLabel.Text = balance.ToString();
                }

                else
                {
                    dataGridView1.Rows.Clear();
                    nameLabel.Text = "Customer not added!";
                    balanceLabel.Text = "0";
                }
                //Microsoft.Scripting..ScriptEngine py = Python.CreateEngine();
            }


            else
            {
                dataGridView1.Rows.Clear();
                label1.Hide();
                label2.Hide();
                nameLabel.Hide();
                balanceLabel.Hide();
                button3.Enabled = false;
                button5.Enabled = false;
                button8.Enabled = false;
                button10.Enabled = false;
                button15.Enabled = false;
            }

        }

        private void label3_MouseHover(object sender, EventArgs e)
        {
            //label3.Text = DBHandler.GetSales("LENOVO-PC\\SQL2008", "GRetailExtreme_THEPANTHOUSE", DateTime.Now.Date.ToString("yyyy-MM-dd")).ToString();
            //toolTip1.SetToolTip(label3, DBHandler.GetSales("LENOVO-PC\\SQL2008", "GRExtreme_PantHouseJ", DateTime.Now.Date.ToString("yyyy-MM-dd")).ToString());
            toolTip1.SetToolTip(label3, DBHandler.GetSales(tphServerName, tphDBName, DateTime.Now.Date.ToString("yyyy-MM-dd")).ToString());
        }

        private void label4_MouseHover(object sender, EventArgs e)
        {
            //toolTip1.SetToolTip(label4, DBHandler.GetSales("HP-PC\\SQL2008", "G_PANTHOUSE", DateTime.Now.Date.ToString("yyyy-MM-dd")).ToString());
            toolTip1.SetToolTip(label4, DBHandler.GetSales(jrServerName, jrDBName, DateTime.Now.Date.ToString("yyyy-MM-dd")).ToString());
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewPH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex >= 0 && dataGridViewPH.Columns[e.ColumnIndex].Name == "Action")
            {
                ShowBillDetail(dataGridViewPH, tphServerName, tphDBName, "TPH", e.RowIndex);
                return;
            }

            if (dataGridViewPH.Rows[e.RowIndex].Cells["Mobile"].Value != null)
            {
                String mobile = dataGridViewPH.Rows[e.RowIndex].Cells["Mobile"].Value.ToString();
                textBox1.Text = mobile;
            }
        }

        private void dataGridViewJunior_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex >= 0 && dataGridViewJunior.Columns[e.ColumnIndex].Name == "Action")
            {
                ShowBillDetail(dataGridViewJunior, jrServerName, jrDBName, "Junior", e.RowIndex);
                return;
            }

            if (dataGridViewJunior.Rows[e.RowIndex].Cells["Mobile"].Value != null)
            {
                String mobile = dataGridViewJunior.Rows[e.RowIndex].Cells["Mobile"].Value.ToString();
                textBox1.Text = mobile;
            }
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            String mobile = textBox1.Text;
            String name = nameLabel.Text;
            int pointsRedeemed = int.Parse(button.Text.Substring(0, button.Text.Length - 2));

            DialogResult dialogResult = MessageBox.Show("Redeem " + pointsRedeemed.ToString() + " points for " + mobile + " - " + name + "?", "Redeem Gift", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                
                DataTable bill = DBHandler.SelectQueryOnTable("BillDetails", " TOP 1 * ", "WHERE Mobile = '" + mobile + "' ORDER BY BillDate DESC");
                String oldBags = bill.Rows[0]["BagsGiven"].ToString();
                int oldPointsRedeemed = int.Parse(bill.Rows[0]["PointsRedeemed"].ToString());

                int oldBal = DBHandler.GetBalance(mobile);
                int newBal = oldBal - pointsRedeemed;

                String newBags = oldBags + "1x" + pointsRedeemed.ToString();
                int newPointsRedeemed = oldPointsRedeemed + pointsRedeemed;
                //MessageBox.Show("Old Bags: " + oldBags +"\nNew Bags: " + newBags + "\nOld Bal: " + oldBal + "\nNew Bal: " + newBal);

                DBHandler.UpdateBagsGiven(mobile, bill.Rows[0]["BookCode"].ToString(), bill.Rows[0]["BillNo"].ToString(), bill.Rows[0]["Source"].ToString(), bill.Rows[0]["Year"].ToString(), newBags.ToString(), newPointsRedeemed.ToString());
                DBHandler.UpdateBalance(mobile, newBal.ToString());

                DBHandler.AddTransaction("BillDetails", machine + "-App", "Added Bags to " + mobile + " " + name, "BagsGiven: " + oldBags + ", PointsRedeemed: " + oldPointsRedeemed, "BagsGiven: " + newBags + ", PointsRedeemed: " + newPointsRedeemed, mobile);
                DBHandler.AddTransaction("Overview", machine + "-App", "Reduced Balance of " + mobile + " " + name + " by " + pointsRedeemed, "Balance: " + oldBal, "Balance: " + newBal, mobile);
                
                MessageBox.Show(pointsRedeemed + " points redeemed for " + mobile + " - " + name + ". New Balance: " + newBal);
                textBox1.Text = "";
                textBox1.Text = mobile;
                //ParseBagsGiven(newBags);
                
            }
            else if (dialogResult == DialogResult.No)
            {
                //MessageBox.Show("No!!!!!");
            }
        }

        private void RemoveButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;

            int rowIndex = dataGridView1.CurrentCell.RowIndex;

            String mobile = textBox1.Text;
            String name = nameLabel.Text;
            String billNo = dataGridView1.Rows[rowIndex].Cells["Bill No"].Value.ToString();
            String billDate = dataGridView1.Rows[rowIndex].Cells["Bill Date"].Value.ToString();
            String source = dataGridView1.Rows[rowIndex].Cells["Source"].Value.ToString();
            String date = DateTime.Parse(billDate).ToString("MM/dd/yyyy");    

            int pointsRemove = int.Parse(button.Text.Substring(0, button.Text.Length - 2));

            DialogResult dialogResult = MessageBox.Show("Delete bag worth " + pointsRemove.ToString() + " for " + mobile + " - " + name + "?", "Remove Bag", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                DataTable bill = DBHandler.SelectQueryOnTable("BillDetails", " TOP 1 * ", "WHERE Mobile = '" + mobile + "' AND BillNo = '" + billNo + "' AND Source = '" + source + "' AND BillDate = '" + date + "'");

                String oldBags = bill.Rows[0]["BagsGiven"].ToString();
                int oldPointsRedeemed = int.Parse(bill.Rows[0]["PointsRedeemed"].ToString());
                int oldBal = DBHandler.GetBalance(mobile);

                int newBal = oldBal + pointsRemove;
                int newPointsRedeemed = oldPointsRedeemed - pointsRemove;
                String newBags = "";

                List<Tuple<String, String>> bagsGivenList = ParseBagsGiven(oldBags);

                for (int x = 0; x < bagsGivenList.Count; x++ )
                {
                    if (bagsGivenList[x].Item2 == pointsRemove.ToString())
                    {
                        int qty = int.Parse(bagsGivenList[x].Item1);

                        if (qty == 1) bagsGivenList.RemoveAt(x);
                        else
                        {
                            bagsGivenList[x] = Tuple.Create((qty-1).ToString(), bagsGivenList[x].Item2);
                        }
                        break;
                    }
                }

                foreach (Tuple<String, String> bag in bagsGivenList)
                {
                    newBags += bag.Item1 + "x" + bag.Item2;
                }
                //MessageBox.Show("OldBags: " + oldBags + "\nNew Bag List: " + newBags + "OldBal: " + oldBal + "\nNewBalance: " + newBal);

                DBHandler.UpdateBagsGiven(mobile, bill.Rows[0]["BookCode"].ToString(), bill.Rows[0]["BillNo"].ToString(), bill.Rows[0]["Source"].ToString(), bill.Rows[0]["Year"].ToString(), newBags.ToString(), newPointsRedeemed.ToString());
                DBHandler.UpdateBalance(mobile, newBal.ToString());

                DBHandler.AddTransaction("BillDetails", machine + "-App", "Removed Bags from " + mobile + " " + name, "BagsGiven: " + oldBags + ", PointsRedeemed: " + oldPointsRedeemed, "BagsGiven: " + newBags + ", PointsRedeemed: " + newPointsRedeemed, mobile);
                DBHandler.AddTransaction("Overview", machine + "-App", "Increased Balance of " + mobile + " " + name + " by " + pointsRemove, "Balance: " + oldBal, "Balance: " + newBal, mobile);
                
                MessageBox.Show("Bags worth " + pointsRemove + " deleted for " + mobile +" - " + name + ". New Balance: " + newBal);
                textBox1.Text = "";
                textBox1.Text = mobile;
            }

            else if (dialogResult == DialogResult.No)
            {
                //MessageBox.Show("No!!!!!");
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            remove3.Enabled = false;
            remove5.Enabled = false;
            remove8.Enabled = false;
            remove10.Enabled = false;
            remove15.Enabled = false;

            int rowCount = dataGridView1.RowCount;
            int rowIndex = e.RowIndex;

            if (rowIndex != rowCount - 1)
            {
                String bagsGiven = dataGridView1.Rows[e.RowIndex].Cells["Points Redeemed"].Value.ToString();

                List<Tuple<String, String>> bagsGivenList = ParseBagsGiven(bagsGiven);

                EnableRemoveButtons(bagsGivenList);

                //MessageBox.Show(bagsGiven);
            }
        }
        
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void rectangleShape3_MouseDown(object sender, MouseEventArgs e)
        {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            
            LogWriter log = new LogWriter("REFRESHING APP! " + DateTime.Now.ToString());

            RefreshLastUpdated();
            GetNewBills();
            log.LogWrite("Got New Bills!");
            GetBagsGiven();
            log.LogWrite("Got Bags Given!");
            UpdateBalance();
            log.LogWrite("Updated Balance!");

            VerifyBalance();
            log.LogWrite("Verified Balance");

            UpdateLastUpdated();
            log.LogWrite("Updated Last Updated!");

            RefreshLastUpdated();
            //GetNewBags();
            //UpdateNewBags();

            //DataTable dt = DBHandler.GetLastBills("LENOVO-PC\\SQL2008", "GRExtreme_PantHouseJ", "10", finYearID);
            DataTable dt = DBHandler.GetLastBills(tphServerName, tphDBName, "10", phFinYearID);
            //DataTable dt2 = DBHandler.GetLastBills("HP-PC\\SQL2008", "G_PANTHOUSE", "10", finYearID);
            DataTable dt2 = DBHandler.GetLastBills(jrServerName, jrDBName, "10", jrFinYearID);

            UpdateBillDataGrid(dt, dt2);


        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            UpdateBillDialog updateBill = new UpdateBillDialog();
            updateBill.Show();
        }

        private void populateDailyBalanceTab()
        {
            //Replace HP-PC with machine****************************
            //SaleParameters sale = DBHandler.getDailySales("HP-PC" + "\\SQL2008", machineDbNameDict["HP-PC"], DateTime.Now.Date.ToString("yyyy-MM-dd"));
            //SaleParameters sale = DBHandler.getDailySales(machine + "\\SQL2008", machineDbNameDict[machine], DateTime.Now.Date.ToString("yyyy-MM-dd"), paytmId, bajajId);
            SaleParameters sale = DBHandler.getDailySales(machineServerNameDict[machine], machineDbNameDict[machine], DateTime.Now.Date.ToString("yyyy-MM-dd"), paytmId, bajajId);  // FIXME

            totalSaleTextBox.Text = sale.getTotSale().ToString();
            cashSaleTextBox.Text = sale.getCashSale().ToString();
            cardSaleTextBox.Text = sale.getCardSale().ToString();
            debitSaleTextBox.Text = sale.getDebitSale().ToString();
            balReceivedCashTextBox.Text = sale.getBalReceivedCash().ToString();
            balReceivedCardTextBox.Text = sale.getBalReceivedCard().ToString();
            paytmTextBox.Text = sale.getPaytm().ToString();
            bajajTextBox.Text = sale.getBajaj().ToString();
            crntIssuedTextBox.Text = sale.getCrntIssued().ToString();
            crntRecvTextBox.Text = sale.getCrntRecv().ToString();
            startBillTextBox.Text = sale.getStartBill();
            endBillTextBox.Text = sale.getEndBill();

            CheckBalanceMatch();
            UpdateCalculatedBalance();
            //log.LogWrite("Updated Calculated Balance");
        }

        private void dailyBalanceTab_Click(object sender, EventArgs e)
        {
            //Replace HP-PC with machine****************************
            //SaleParameters sale = DBHandler.getDailySales("HP-PC" + "\\SQL2008", machineDbNameDict["HP-PC"], DateTime.Now.Date.ToString("yyyy-MM-dd"));
            try
            {
                //SaleParameters sale = DBHandler.getDailySales(machine + "\\SQL2008", machineDbNameDict[machine], DateTime.Now.Date.ToString("yyyy-MM-dd"), paytmId, bajajId);
                // SaleParameters sale = DBHandler.getDailySales(machineServerNameDict[machine], machineDbNameDict[machine], DateTime.Now.Date.ToString("yyyy-MM-dd"), paytmId, bajajId); // FIXME DateTime.Now to currentBillDate
                SaleParameters sale = DBHandler.getDailySales(machineServerNameDict[machine], machineDbNameDict[machine], dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd"), paytmId, bajajId); // FIXME DateTime.Now to currentBillDate

                totalSaleTextBox.Text = sale.getTotSale().ToString();
                cashSaleTextBox.Text = sale.getCashSale().ToString();
                cardSaleTextBox.Text = sale.getCardSale().ToString();
                debitSaleTextBox.Text = sale.getDebitSale().ToString();
                balReceivedCashTextBox.Text = sale.getBalReceivedCash().ToString();
                balReceivedCardTextBox.Text = sale.getBalReceivedCard().ToString();
                paytmTextBox.Text = sale.getPaytm().ToString();
                bajajTextBox.Text = sale.getBajaj().ToString();
                crntIssuedTextBox.Text = sale.getCrntIssued().ToString();
                crntRecvTextBox.Text = sale.getCrntRecv().ToString();
                startBillTextBox.Text = sale.getStartBill();
                endBillTextBox.Text = sale.getEndBill();

                CheckBalanceMatch();
                UpdateCalculatedBalance();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //log.LogWrite("Updated Calculated Balance");
        }

        private void cashGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            DataGridViewRow row = cashGridView.Rows[rowIndex];

            int nos, denomination;
            int.TryParse(row.Cells["Nos"].Value.ToString(), out nos);
            int.TryParse(row.Cells["Denomination"].Value.ToString(), out denomination);

            string date = dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd");
            int denominationUpdated = DBHandler.UpdateDenomination(denomination.ToString(), nos.ToString(), machine, date);

            row.Cells["Total"].Value = (nos * denomination).ToString();
            CheckBalanceMatch();
            UpdateCalculatedBalance();
        }

        private void addCreditButton_Click(object sender, EventArgs e)
        {
            string particular = creditParticular.Text.ToString().Trim();
            string amount = creditAmount.Text.ToString().Trim();

            if (string.IsNullOrEmpty(particular) || particular == "Enter details...")
            {
                MessageBox.Show("Please enter credit particulars.");
                return;
            }

            int parsedAmount;
            if (!int.TryParse(amount, out parsedAmount) || parsedAmount <= 0)
            {
                MessageBox.Show("Please enter a valid credit amount.");
                return;
            }

            DBHandler.AddDailyTransactions(machine, "CR", particular, parsedAmount.ToString(), dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd"));
            
            UpdateCreditGrid();
            UpdateTotalCredit();
            CheckBalanceMatch();
            UpdateCalculatedBalance();

            creditParticular.Text = "";
            creditAmount.Text = "";
        }

        private void addDebitButton_Click(object sender, EventArgs e)
        {
            string particular = debitParticular.Text.ToString().Trim();
            string amount = debitAmount.Text.ToString().Trim();

            if (string.IsNullOrEmpty(particular) || particular == "Enter details...")
            {
                MessageBox.Show("Please enter debit particulars.");
                return;
            }

            int parsedAmount;
            if (!int.TryParse(amount, out parsedAmount) || parsedAmount <= 0)
            {
                MessageBox.Show("Please enter a valid debit amount.");
                return;
            }

            DBHandler.AddDailyTransactions(machine, "DR", particular, parsedAmount.ToString(), dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd"));
            
            UpdateDebitGrid();
            UpdateTotalDebit();
            CheckBalanceMatch();
            UpdateCalculatedBalance();

            debitParticular.Text = "";
            debitAmount.Text = "";
        }

        private void creditParticular_Enter(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text == "Enter details..." || ((TextBox)sender).Text == "Enter Amount...")
            {
                ((TextBox)sender).Text = "";
                ((TextBox)sender).ForeColor = Color.Black;
            }
        }

        private void submitPrintButton_Click(object sender, EventArgs e)
        {
            
            String toEmail = "";
            String source = "";
            //if (machine == "HP-PC") 
            if (machine == jrMachineName)
            {
                toEmail = "sneh.nagrecha@gmail.com";
                source = jrSourceName;
            }
            else
            {
                toEmail = "kishor.nagrecha@gmail.com";
                source = phSourceName;
            }

            DateTime date = dailyBalanceDateTimePicker.Value;

            WriteToExcel.writeToExcel(this, source, date);

            WriteToExcel.sendEmail(toEmail, source, date);

            MessageBox.Show("Daily Balance Submitted Successfully!");
        }

        private void deleteCreditButton_Click(object sender, EventArgs e)
        {
            if (creditGridView.SelectedRows.Count > 0)
            {
                string date = dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd");
                List<DataGridViewRow> rowsToDelete = creditGridView.SelectedRows.Cast<DataGridViewRow>().ToList();
                foreach (DataGridViewRow row in rowsToDelete)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        string particular = row.Cells[0].Value.ToString();
                        string amount = row.Cells[1].Value.ToString();
                        DBHandler.DeleteDailyTransaction(machine, "CR", particular, amount, date);
                        creditGridView.Rows.Remove(row);
                    }
                }
            }

            UpdateTotalCredit();
            CheckBalanceMatch();
            UpdateCalculatedBalance();
        }

        private void deleteDebitButton_Click(object sender, EventArgs e)
        {
            if (debitGridView.SelectedRows.Count > 0)
            {
                string date = dailyBalanceDateTimePicker.Value.ToString("yyyy-MM-dd");
                List<DataGridViewRow> rowsToDelete = debitGridView.SelectedRows.Cast<DataGridViewRow>().ToList();
                foreach (DataGridViewRow row in rowsToDelete)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        string particular = row.Cells[0].Value.ToString();
                        string amount = row.Cells[1].Value.ToString();
                        DBHandler.DeleteDailyTransaction(machine, "DR", particular, amount, date);
                        debitGridView.Rows.Remove(row);
                    }
                }
            }

            UpdateTotalDebit();
            CheckBalanceMatch();
            UpdateCalculatedBalance();
        }

        private void updateMobileButton_Click(object sender, EventArgs e)
        {
            string oldMobile = textBox1.Text;
            string billNo = "", billDate = "", source = "", newMobile = "", newName = "";
            
            NewMobileDialog newMobileDialog = new NewMobileDialog();

            newMobileDialog.newNameDialogText.Text = nameLabel.Text.ToString();

            if (newMobileDialog.ShowDialog(this) == DialogResult.OK)
            {
                newMobile = newMobileDialog.newMobileDialogText.Text.ToString();
                newName = newMobileDialog.newNameDialogText.Text.ToString();

                if (newName.Length == 0) newName = nameLabel.Text.ToString();

                bool exists = DBHandler.CheckIfMemberExists(newMobile);

                if (!exists)
                {
                    if (newMobile.Length == 10)
                    {
                        DBHandler.InsertIntoTable("Overview", "", "'" + newMobile + "', '" + newName + "', 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP");
                        DBHandler.AddTransaction("Overview", machine + "-Bill", "Added new Customer. Mobile: " + newMobile + " Name: " + newName, "", newMobile + " " + newName + " 0", newMobile);
                    }
                }

                for (int index = 0; index < dataGridView1.RowCount - 1; index++)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[index].Cells["Update"].Value) == true)
                    {
                        DateTime date = Convert.ToDateTime(dataGridView1.Rows[index].Cells["Bill Date"].Value.ToString());
                        billNo = dataGridView1.Rows[index].Cells["Bill No"].Value.ToString();
                        source = dataGridView1.Rows[index].Cells["Source"].Value.ToString();
                        billDate = date.ToString("MM-dd-yyyy");
                        DBHandler.UpdateMobile(oldMobile, newMobile, billDate, billNo, source);
                    }
                }

                VerifyBalance();
                textBox1.Text = newMobile;
                //UpdateHomeGrid(newMobile);
                MessageBox.Show("Bills Transferred to new number Successflly!");
            }
        }

        private void checkBalance_Click(object sender, EventArgs e)
        {

        }

        /*private void label12_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label12, "Host Server: " + hostServerName + "\tHost DB: " + hostDBName + 
                                         "\nTPH Server: " + tphServerName + "\tTPH DB: " + tphDBName + "\tTPH Machine: " + tphMachineName + 
                                         "\nJR Server: " + jrServerName + "\tJR DB: " + jrDBName + "\tJR Machine: " + jrMachineName);
        }*/

        private void searchButton_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            dt = DBHandler.GetCustomerList(hostServerName, hostDBName);

            customerListDataGrid.DataSource = dt;
            StyleGridCommon(customerListDataGrid);

            totalAccountsLabel.Text = "Total Accounts: " + customerListDataGrid.Rows.Count;

        }

        private void exportButton_Click(object sender, EventArgs e)
        {/*
            // creating Excel Application  
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();

            // creating new WorkBook within Excel application  
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);

            // creating new Excelsheet in workbook  
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            // see the excel sheet behind the program  
            //app.Visible = true;

            // get the reference of first sheet. By default its name is Sheet1.  
            // store its reference to worksheet  
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;

            // changing the name of active sheet  
            //worksheet.Name = "Exported from gridview";  

            for(int i = 1; i < customerListDataGrid.Rows.Count; i++)
            {
                worksheet.Cells[i + 1, 1] = customerListDataGrid.Rows[i].Cells["Mobile"].Value.ToString();
                worksheet.Cells[i + 1, 2] = customerListDataGrid.Rows[i].Cells["Name"].Value.ToString();
            }

            // save the application  
            workbook.SaveAs("D:\\Customer Loyalty Portal\\CustomerList.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            MessageBox.Show("Export Successful");
            // Exit from the application  
            app.Quit();*/
        }

        private void ShowBillDetail(DataGridView grid, string serverName, string dbname, string sourceName, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;
            DataGridViewRow row = grid.Rows[rowIndex];
            string dateStr = row.Cells["Date"].Value != null ? row.Cells["Date"].Value.ToString() : "";
            string timeStr = row.Cells["Time"].Value != null ? row.Cells["Time"].Value.ToString() : "";
            string voucherNo = row.Cells["Bill No"].Value != null ? row.Cells["Bill No"].Value.ToString() : "";
            string mobile = row.Cells["Mobile"].Value != null ? row.Cells["Mobile"].Value.ToString() : "";
            string customerName = row.Cells["Name"].Value != null ? row.Cells["Name"].Value.ToString() : "";
            string salesID = (grid.Columns.Contains("SalesID") && row.Cells["SalesID"].Value != null) ? row.Cells["SalesID"].Value.ToString() : "";

            BillDetailDialog dlg = new BillDetailDialog(serverName, dbname, sourceName, salesID, voucherNo, customerName, mobile, dateStr, timeStr);
            dlg.ShowDialog(this);
        }

        private void dataGridViewPH_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ShowBillDetail(dataGridViewPH, tphServerName, tphDBName, "TPH", e.RowIndex);
            }
        }

        private void dataGridViewJunior_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ShowBillDetail(dataGridViewJunior, jrServerName, jrDBName, "Junior", e.RowIndex);
            }
        }

        public void SetupBagButtonStates()
        {
            Button[] addButtons = { button3, button5, button8, button10, button15 };
            foreach (var btn in addButtons)
            {
                btn.EnabledChanged += AddBagButton_EnabledChanged;
                AddBagButton_EnabledChanged(btn, EventArgs.Empty);
            }

            Button[] removeButtons = { remove3, remove5, remove8, remove10, remove15 };
            foreach (var btn in removeButtons)
            {
                btn.EnabledChanged += RemoveBagButton_EnabledChanged;
                RemoveBagButton_EnabledChanged(btn, EventArgs.Empty);
            }
        }

        private void AddBagButton_EnabledChanged(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if (btn.Enabled)
            {
                btn.BackColor = Color.FromArgb(40, 167, 69);
                btn.ForeColor = Color.White;
                btn.Cursor = Cursors.Hand;
            }
            else
            {
                btn.BackColor = Color.FromArgb(233, 236, 239);
                btn.ForeColor = Color.FromArgb(173, 181, 189);
                btn.Cursor = Cursors.Default;
            }
        }

        private void RemoveBagButton_EnabledChanged(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            if (btn.Enabled)
            {
                btn.BackColor = Color.FromArgb(220, 53, 69);
                btn.ForeColor = Color.White;
                btn.Cursor = Cursors.Hand;
            }
            else
            {
                btn.BackColor = Color.FromArgb(233, 236, 239);
                btn.ForeColor = Color.FromArgb(173, 181, 189);
                btn.Cursor = Cursors.Default;
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell pointsCell = dataGridView1.CurrentCell;
            int rowIndex = pointsCell.RowIndex;
            
            if (pointsCell.OwningColumn.Name == "Points")
            {
                UpdatePointsDialog updatePointsDialog = new UpdatePointsDialog();

                //updatePointsDialog.ShowDialog();
                string oldPoints = pointsCell.Value.ToString();
                updatePointsDialog.oldPointsTextBox.Text = oldPoints;
                
                if(updatePointsDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string newPoints = updatePointsDialog.newPointsTextBox.Text.ToString();

                    int newPointsInt;
                    bool isNumeric = int.TryParse(newPoints, out newPointsInt);

                    if (isNumeric)
                    {
                        string mobile = textBox1.Text.ToString();
                        string billNo = dataGridView1.Rows[rowIndex].Cells["Bill No"].Value.ToString();
                        string source = dataGridView1.Rows[rowIndex].Cells["Source"].Value.ToString();

                        DateTime date = Convert.ToDateTime(dataGridView1.Rows[rowIndex].Cells["Bill Date"].Value.ToString());
                        string billDate = date.ToString("MM-dd-yyyy");

                        DBHandler.UpdatePoints(mobile, newPoints, billDate, billNo, source);

                        VerifyBalance();

                        textBox1.Text = "";
                        textBox1.Text = mobile;
                        

                        MessageBox.Show(mobile + '\n' + billNo + '\n' + billDate + '\n' + source + "\nOld Points: " + oldPoints + "\nNew Points: " + newPoints);
                    }
                }

            }
        }

        private void mobileTextBox_TextChanged(object sender, EventArgs e)
        {
            String mobile = mobileTextBox.Text.ToString();
            //float balance = 0;
            String name = "";
            
            if (mobile.Length == 10)
            {
                
                DataTable dt = DBHandler.SelectQueryOnTable("Overview", "*", "WHERE Mobile = '" + mobile + "'");

                foreach (DataRow row in dt.Rows)
                {
                    balance = float.Parse(row["Balance"].ToString());
                    name = row["Name"].ToString();
                }

                if (name.Length > 0)
                {
                    nameTextBox.Text = name;
                    nameTextBox.ReadOnly = true;
                    enterNameLabel.Visible = false;
                }

                else
                {
                    nameTextBox.ReadOnly = false;
                    enterNameLabel.Visible = true;
                    
                }
                //Microsoft.Scripting..ScriptEngine py = Python.CreateEngine();
            }


            else
            {
                nameTextBox.ReadOnly = true;
                nameTextBox.Text = "";
                enterNameLabel.Visible = false;
            }
        }

        private void billNoTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //IF ENTERED CHARACTER IS NOT A DIGIT THEN SKIP THE EVENT
            if(!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void pointsTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //IF ENTERED CHARACTER IS NOT A DIGIT THEN SKIP THE EVENT
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void addBillButton_Click(object sender, EventArgs e)
        {
            string billNo = billNoTextBox.Text;
            string billDate = ((DateTime)billDateTime.Value).Date.ToString("MM-dd-yyyy");
            int points = 0;
            int.TryParse(pointsTextBox.Text, out points);
            string year = Utilities.GetFinYearFromDate((DateTime)billDateTime.Value);
            string mobile = mobileTextBox.Text;
            string name = nameTextBox.Text;
            string source = billSourceComboBox.SelectedItem.ToString();
            string finYearID = Utilities.GetFinYearIDFromDate((DateTime)billDateTime.Value, source);

            string bagsGiven = "";
            int pointsRedeemed = 0;

            int qty3 = (int)qty3k.Value;
            int qty5 = (int)qty5k.Value;
            int qty8 = (int)qty8k.Value;
            int qty10 = (int)qty10k.Value;
            int qty15 = (int)qty15k.Value;
            int qty20 = (int)qty20k.Value;

            if (qty3 > 0) { bagsGiven += qty3 + "x3000"; pointsRedeemed += qty3 * 3000; }
            if (qty5 > 0) { bagsGiven += qty5 + "x5000"; pointsRedeemed += qty5 * 5000; }
            if (qty8 > 0) { bagsGiven += qty8 + "x8000"; pointsRedeemed += qty8 * 8000; }
            if (qty10 > 0) { bagsGiven += qty10 + "x10000"; pointsRedeemed += qty10 * 10000; }
            if (qty15 > 0) { bagsGiven += qty15 + "x15000"; pointsRedeemed += qty15 * 15000; }
            if (qty20 > 0) { bagsGiven += qty20 + "x20000"; pointsRedeemed += qty20 * 20000; }

            MessageBox.Show($"Bill No: {billNo} \nBill Date: {billDate} \nPoints: {points} \nYear: {year} \nMobile: {mobile} \nName: {name} \nFinYearId: {finYearID} \nSource: {source}");

            UpdateListOfBills(mobile, "401", billNo, source, points.ToString(), "0", year, bagsGiven, billDate, name, pointsRedeemed.ToString(), finYearID, "NA");
            UpdateBalance();
            DBHandler.UpdateBagsGiven(mobile, "401", billNo, source, year, bagsGiven, pointsRedeemed.ToString());

            lastBillAddedLabel.Text = $"Last Bill Added: {mobile} - {billNo} - {billDate} - {points} - {source} on {DateTime.Now}";
            //RESET FIELDS AFTER ADDING A BILL
            qty3k.Value = qty5k.Value = qty8k.Value = qty10k.Value = qty15k.Value = qty20k.Value = 0;
            billNoTextBox.Text = "0" + ((int.Parse(billNo)+1));
            mobileTextBox.Text = "";
            pointsTextBox.Text = "";
            mobileTextBox.Focus();
        }

        private void SelectAllText(object sender, EventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void paytmTextBox_MouseHover(object sender, EventArgs e)
        {

        }

        private void saleDetails_MouseHover(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string cardAccountID = "";

            if (textBox.Name.StartsWith("paytm")) cardAccountID = paytmId;
            else if (textBox.Name.StartsWith("bajaj")) cardAccountID = bajajId;
            else cardAccountID = cardId;

            ToolTip toolTip = new ToolTip();
            string toolTipText = "Bill No   \tMobile    \tName      \tCard\tBill\n";
            DataTable dt = DBHandler.GetDetailedSales(machineServerNameDict[machine], machineDbNameDict[machine], cardAccountID);
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    string data = row[col].ToString();

                    if (data.Length >= 10) data = data.Substring(0, 10);
                    else if (data.Length < 10) data = data.PadRight(10, ' ');

                    toolTipText += data + "\t";
                }
                toolTipText += "\n";
            }

            // Set the tooltip text and show the tooltip.
            toolTip.SetToolTip((TextBox)sender, toolTipText);
        }

        private void saleDetails_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string cardAccountID = "";

            if (textBox.Name.StartsWith("paytm")) cardAccountID = paytmId;
            else if (textBox.Name.StartsWith("bajaj")) cardAccountID = bajajId;
            else cardAccountID = cardId;

            // Create a new form for the pop-up window
            Form popupForm = new Form();
            popupForm.Text = "Paytm Pop-Up";
            //popupForm.Size = new Size(400, 300);

            // Create a DataGridView control on the pop-up form
            DataGridView saleDetailsDataGridView = new DataGridView();
            saleDetailsDataGridView.Dock = DockStyle.Fill;

            DataTable dt = DBHandler.GetDetailedSales(machineServerNameDict[machine], machineDbNameDict[machine], cardAccountID);

            // Set the DataSource of the DataGridView to display the DataTable
            saleDetailsDataGridView.DataSource = dt;

            // Add the DataGridView to the pop-up form
            popupForm.Controls.Add(saleDetailsDataGridView);

            // Show the pop-up form as a dialog
            popupForm.ShowDialog();
        }

        private void saleDetailsInfo_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            string cardAccountID = "";
            string popupTitle = "";

            if (pictureBox.Name.StartsWith("paytm"))
            {
                cardAccountID = paytmId;
                popupTitle = "UPI Details";
            }
            else if (pictureBox.Name.StartsWith("bajaj"))
            {
                cardAccountID = bajajId;
                popupTitle = "Bajaj Details";
            }
            else
            {
                cardAccountID = cardId;
                popupTitle = "Credit Card Details";
            }

            // Create a new form for the pop-up window
            Form popupForm = new Form();
            popupForm.Text = popupTitle;
            //popupForm.Size = new Size(400, 300);

            // Create a DataGridView control on the pop-up form
            DataGridView saleDetailsDataGridView = new DataGridView();
            saleDetailsDataGridView.Dock = DockStyle.Fill;

            DataTable dt = DBHandler.GetDetailedSales(machineServerNameDict[machine], machineDbNameDict[machine], cardAccountID);

            // Set the DataSource of the DataGridView to display the DataTable
            saleDetailsDataGridView.DataSource = dt;
            saleDetailsDataGridView.BackgroundColor = Color.White;
            saleDetailsDataGridView.ReadOnly = true;
            saleDetailsDataGridView.RowHeadersVisible = false;
            saleDetailsDataGridView.AllowUserToAddRows = false;

            // Add the DataGridView to the pop-up form
            popupForm.Controls.Add(saleDetailsDataGridView);
            popupForm.Size = new Size(550, 600);

            // Show the pop-up form as a dialog
            popupForm.ShowDialog();
        }

        private void UpdateCommissionDateLabel()
        {
            DateTime selectedDate = comissionDateTimePicker.Value;
            if (dateCommissionLabel != null)
            {
                dateCommissionLabel.Text = "Date: " + selectedDate.ToString("dd-MM-yyyy") + " (" + selectedDate.ToString("dddd") + ")";
            }
        }

        public void UpdateCommissionData()
        {
            UpdateCommissionDateLabel();

            string dbName = "";
            string serverName = "";
            if (machine == tphMachineName)
            {
                dbName = tphDBName;
                serverName = tphServerName;
            }
            else if (machine == jrMachineName)
            {
                dbName = jrDBName;
                serverName = jrServerName;
            }
            else
            {
                if (machineDbNameDict.ContainsKey(machine))
                    dbName = machineDbNameDict[machine];
                if (machineServerNameDict.ContainsKey(machine))
                    serverName = machineServerNameDict[machine];
            }

            string currentDate = comissionDateTimePicker.Value.ToString("yyyy-MM-dd");

            // 1. Query Sales Items
            DataTable dt_sales = DBHandler.SelectQueryOnTable(
                "trnSalesItem as a INNER JOIN mstSalesman as b ON a.SalesmanID = b.SalesmanID",
                "b.ShortName as SalesmanNo, b.SalesmanName as Salesman, COUNT(a.SalesTypeSR) as Qty, ROUND(SUM(a.TaxableAmt), 0) as TaxableAmt, ROUND(SUM(a.TaxableAmt) * 0.01, 0) as Comission",
                $"WHERE a.SalesId IN (SELECT SalesID FROM trnSales WHERE CAST(VoucherDate AS DATE) = '{currentDate}') AND a.SalesTypeSR = 'S' GROUP BY a.SalesmanID, b.SalesmanName, b.ShortName",
                serverName, dbName);

            // 2. Query Return Items
            DataTable dt_return = DBHandler.SelectQueryOnTable(
                "trnSalesItem as a INNER JOIN mstSalesman as b ON a.SalesmanID = b.SalesmanID",
                "b.ShortName as SalesmanNo, b.SalesmanName as Salesman, COUNT(a.SalesTypeSR) as Qty, ROUND(SUM(a.TaxableAmt), 0) as TaxableAmt, ROUND(SUM(a.TaxableAmt) * 0.01, 0) as Comission",
                $"WHERE a.SalesId IN (SELECT SalesID FROM trnSales WHERE CAST(VoucherDate AS DATE) = '{currentDate}') AND a.SalesTypeSR = 'R' GROUP BY a.SalesmanID, b.SalesmanName, b.ShortName",
                serverName, dbName);

            // 3. Query Per-Bill Sales with VoucherNo, MobileNo, AccountName for single-customer bill incentive calculations
            DataTable dt_bill_sales = DBHandler.SelectQueryOnTable(
                "trnSalesItem as a INNER JOIN mstSalesman as b ON a.SalesmanID = b.SalesmanID INNER JOIN trnSales as s ON a.SalesId = s.SalesID",
                "a.SalesId, s.VoucherNo, s.MobileNo, s.AccountName, a.SalesmanID, b.ShortName as SalesmanNo, b.SalesmanName as Salesman, ROUND(SUM(a.NetAmt), 0) as BillNetAmt, ROUND(SUM(a.TaxableAmt), 0) as BillTaxableAmt",
                $"WHERE CAST(s.VoucherDate AS DATE) = '{currentDate}' AND a.SalesTypeSR = 'S' GROUP BY a.SalesId, s.VoucherNo, s.MobileNo, s.AccountName, a.SalesmanID, b.SalesmanName, b.ShortName",
                serverName, dbName);

            Dictionary<string, SalesmanCommissionData> stats = new Dictionary<string, SalesmanCommissionData>();

            if (dt_sales != null)
            {
                foreach (DataRow row in dt_sales.Rows)
                {
                    string salesmanNo = row["SalesmanNo"] != DBNull.Value ? row["SalesmanNo"].ToString().Trim() : "";
                    string salesmanName = row["Salesman"] != DBNull.Value ? row["Salesman"].ToString().Trim() : "";
                    string key = salesmanNo + "|" + salesmanName;

                    if (!stats.ContainsKey(key))
                    {
                        stats[key] = new SalesmanCommissionData
                        {
                            SalesmanNo = salesmanNo,
                            SalesmanName = salesmanName
                        };
                    }

                    int qty = row["Qty"] != DBNull.Value ? Convert.ToInt32(row["Qty"]) : 0;
                    decimal taxableAmt = row["TaxableAmt"] != DBNull.Value ? Convert.ToDecimal(row["TaxableAmt"]) : 0m;

                    stats[key].SalesQty += qty;
                    stats[key].SalesTaxableAmt += taxableAmt;
                }
            }

            if (dt_return != null)
            {
                foreach (DataRow row in dt_return.Rows)
                {
                    string salesmanNo = row["SalesmanNo"] != DBNull.Value ? row["SalesmanNo"].ToString().Trim() : "";
                    string salesmanName = row["Salesman"] != DBNull.Value ? row["Salesman"].ToString().Trim() : "";
                    string key = salesmanNo + "|" + salesmanName;

                    if (!stats.ContainsKey(key))
                    {
                        stats[key] = new SalesmanCommissionData
                        {
                            SalesmanNo = salesmanNo,
                            SalesmanName = salesmanName
                        };
                    }

                    int qty = row["Qty"] != DBNull.Value ? Convert.ToInt32(row["Qty"]) : 0;
                    decimal taxableAmt = row["TaxableAmt"] != DBNull.Value ? Convert.ToDecimal(row["TaxableAmt"]) : 0m;

                    stats[key].ReturnQty += qty;
                    stats[key].ReturnTaxableAmt += taxableAmt;
                }
            }

            // Calculate Bill Amount Incentives (combining same-day bills for the same customer)
            if (dt_bill_sales != null && IncentiveManager.Enabled)
            {
                // Group sales per customer per salesman
                Dictionary<string, Dictionary<string, CustomerSalesSummary>> salesmanCustomerSales = new Dictionary<string, Dictionary<string, CustomerSalesSummary>>();

                foreach (DataRow row in dt_bill_sales.Rows)
                {
                    string salesmanNo = row["SalesmanNo"] != DBNull.Value ? row["SalesmanNo"].ToString().Trim() : "";
                    string salesmanName = row["Salesman"] != DBNull.Value ? row["Salesman"].ToString().Trim() : "";
                    string key = salesmanNo + "|" + salesmanName;

                    if (!stats.ContainsKey(key)) continue;

                    string voucherNo = row.Table.Columns.Contains("VoucherNo") && row["VoucherNo"] != DBNull.Value ? row["VoucherNo"].ToString().Trim() : "";
                    string mobile = row.Table.Columns.Contains("MobileNo") && row["MobileNo"] != DBNull.Value ? row["MobileNo"].ToString().Trim() : "";
                    string accName = row.Table.Columns.Contains("AccountName") && row["AccountName"] != DBNull.Value ? row["AccountName"].ToString().Trim() : "";
                    string salesId = row["SalesId"] != DBNull.Value ? row["SalesId"].ToString() : "";

                    // Identify customer (by mobile number if valid, or account name, or per-bill for anonymous walk-in)
                    string custKey;
                    string custDisplay;
                    if (!string.IsNullOrEmpty(mobile) && mobile != "0" && mobile.Length >= 6 && mobile != "9999999999")
                    {
                        custKey = "MOB:" + mobile;
                        custDisplay = mobile;
                    }
                    else if (!string.IsNullOrEmpty(accName) && 
                             !accName.Equals("cash", StringComparison.OrdinalIgnoreCase) && 
                             !accName.Equals("walk-in", StringComparison.OrdinalIgnoreCase) && 
                             !accName.Equals("walk in", StringComparison.OrdinalIgnoreCase) &&
                             !accName.Equals("retail customer", StringComparison.OrdinalIgnoreCase))
                    {
                        custKey = "ACC:" + accName.ToUpperInvariant();
                        custDisplay = accName;
                    }
                    else
                    {
                        custKey = "BILL:" + salesId;
                        custDisplay = "";
                    }

                    if (!salesmanCustomerSales.ContainsKey(key))
                    {
                        salesmanCustomerSales[key] = new Dictionary<string, CustomerSalesSummary>();
                    }

                    if (!salesmanCustomerSales[key].ContainsKey(custKey))
                    {
                        salesmanCustomerSales[key][custKey] = new CustomerSalesSummary
                        {
                            CustomerKey = custKey,
                            CustomerDisplay = custDisplay
                        };
                    }

                    decimal billNet = row["BillNetAmt"] != DBNull.Value ? Convert.ToDecimal(row["BillNetAmt"]) : 0m;
                    decimal billTaxable = row["BillTaxableAmt"] != DBNull.Value ? Convert.ToDecimal(row["BillTaxableAmt"]) : 0m;
                    decimal billAmt = billNet > 0 ? billNet : billTaxable;

                    salesmanCustomerSales[key][custKey].TotalAmount += billAmt;
                    if (!string.IsNullOrEmpty(voucherNo) && !salesmanCustomerSales[key][custKey].VoucherNumbers.Contains(voucherNo))
                    {
                        salesmanCustomerSales[key][custKey].VoucherNumbers.Add(voucherNo);
                    }
                }

                // Apply incentive slabs on combined customer sales totals
                foreach (var sPair in salesmanCustomerSales)
                {
                    string salesmanKey = sPair.Key;
                    var custDict = sPair.Value;

                    foreach (var cPair in custDict)
                    {
                        CustomerSalesSummary summary = cPair.Value;
                        decimal matchedThreshold;
                        decimal inc = IncentiveManager.CalculateBillIncentiveWithThreshold(summary.TotalAmount, out matchedThreshold);

                        if (inc > 0)
                        {
                            stats[salesmanKey].Incentive += inc;
                            stats[salesmanKey].IncentiveBillCount += summary.VoucherNumbers.Count;
                            stats[salesmanKey].QualifyingBills.Add(new BillIncentiveDetail
                            {
                                VoucherNumbers = summary.VoucherNumbers,
                                CustomerDisplay = summary.CustomerDisplay,
                                BillAmount = summary.TotalAmount,
                                IncentiveAmount = inc,
                                SlabThreshold = matchedThreshold
                            });
                        }
                    }
                }
            }

            currentCommissionStats = stats;
            var sortedData = stats.Values.OrderBy(s => s.SalesmanNo).ToList();

            DataTable dt_combined = new DataTable();
            dt_combined.Columns.Add("SalesmanNo", typeof(string));
            dt_combined.Columns.Add("Salesman", typeof(string));
            dt_combined.Columns.Add("Qty", typeof(int));
            dt_combined.Columns.Add("TaxableAmt", typeof(decimal));
            dt_combined.Columns.Add("Comission", typeof(decimal));
            dt_combined.Columns.Add("Incentive", typeof(decimal));
            dt_combined.Columns.Add("TotalPayable", typeof(decimal));

            int totalQty = 0;
            decimal totalTaxable = 0m;
            decimal totalCommission = 0m;
            decimal totalIncentive = 0m;
            decimal totalPayable = 0m;

            foreach (var item in sortedData)
            {
                dt_combined.Rows.Add(item.SalesmanNo, item.SalesmanName, item.NetQty, item.NetTaxableAmt, item.NetCommission, item.Incentive, item.TotalPayable);
                totalQty += item.NetQty;
                totalTaxable += item.NetTaxableAmt;
                totalCommission += item.NetCommission;
                totalIncentive += item.Incentive;
                totalPayable += item.TotalPayable;
            }

            comissionDataGrid.DataSource = dt_combined;
            StyleGridCommon(comissionDataGrid);
            StyleAllDataGrids();

            // Update KPI Cards
            if (lblTotalCommQty != null) lblTotalCommQty.Text = totalQty.ToString("N0") + " pcs";
            if (lblTotalCommSales != null) lblTotalCommSales.Text = "₹ " + totalTaxable.ToString("N0");
            if (lblTotalCommAmt != null) lblTotalCommAmt.Text = "₹ " + totalCommission.ToString("N0");
            if (lblTotalCommTotal != null) lblTotalCommTotal.Text = "₹ " + totalPayable.ToString("N0");

            // Chart 1: Qty (Sales vs Returns)
            chartQty.ChartAreas.Clear();
            chartQty.Titles.Clear();
            chartQty.Series.Clear();
            chartQty.Legends.Clear();

            ChartArea qtyArea = new ChartArea("QtyChartArea");
            qtyArea.AxisX.Interval = 1;
            qtyArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);
            qtyArea.AxisX.MajorGrid.LineColor = Color.FromArgb(235, 238, 242);
            qtyArea.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 238, 242);
            qtyArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            chartQty.ChartAreas.Add(qtyArea);

            Title qtyTitle = new Title("Sales & Returns Qty by Salesman", Docking.Top, new Font("Segoe UI", 10.5F, FontStyle.Bold), Color.FromArgb(41, 60, 90));
            chartQty.Titles.Add(qtyTitle);

            Legend qtyLegend = new Legend("Legend1")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Far,
                Font = new Font("Segoe UI", 8.5F)
            };
            chartQty.Legends.Add(qtyLegend);

            Series salesSeries = new Series("Sales Qty")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(13, 110, 253),
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            Series returnSeries = new Series("Return Qty")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(220, 53, 69),
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            foreach (var item in sortedData)
            {
                salesSeries.Points.AddXY(item.SalesmanName, item.SalesQty);
                returnSeries.Points.AddXY(item.SalesmanName, item.ReturnQty);
            }

            chartQty.Series.Add(salesSeries);
            chartQty.Series.Add(returnSeries);

            // Chart 2: Commission & Incentive by Salesman
            chartCommission.ChartAreas.Clear();
            chartCommission.Titles.Clear();
            chartCommission.Series.Clear();
            chartCommission.Legends.Clear();

            ChartArea commArea = new ChartArea("CommissionChartArea");
            commArea.AxisX.Interval = 1;
            commArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);
            commArea.AxisX.MajorGrid.LineColor = Color.FromArgb(235, 238, 242);
            commArea.AxisY.MajorGrid.LineColor = Color.FromArgb(235, 238, 242);
            commArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);
            chartCommission.ChartAreas.Add(commArea);

            Title commTitle = new Title("Commission & Incentive by Salesman (₹)", Docking.Top, new Font("Segoe UI", 10.5F, FontStyle.Bold), Color.FromArgb(41, 60, 90));
            chartCommission.Titles.Add(commTitle);

            Legend commLegend = new Legend("Legend2")
            {
                Docking = Docking.Top,
                Alignment = StringAlignment.Far,
                Font = new Font("Segoe UI", 8.5F)
            };
            chartCommission.Legends.Add(commLegend);

            Series commissionSeries = new Series("Commission")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(40, 167, 69),
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            Series incentiveSeries = new Series("Incentive")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(245, 158, 11), // Warm Amber
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            foreach (var item in sortedData)
            {
                commissionSeries.Points.AddXY(item.SalesmanName, item.NetCommission);
                incentiveSeries.Points.AddXY(item.SalesmanName, item.Incentive);
            }

            chartCommission.Series.Add(commissionSeries);
            chartCommission.Series.Add(incentiveSeries);
        }

        private void comissionDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateCommissionData();
        }

        private void prevCommissionDateButton_Click(object sender, EventArgs e)
        {
            comissionDateTimePicker.Value = comissionDateTimePicker.Value.AddDays(-1);
        }

        private void nextCommissionDateButton_Click(object sender, EventArgs e)
        {
            comissionDateTimePicker.Value = comissionDateTimePicker.Value.AddDays(1);
        }

        private void todayCommissionButton_Click(object sender, EventArgs e)
        {
            if (comissionDateTimePicker.Value.Date == DateTime.Today)
            {
                UpdateCommissionData();
            }
            else
            {
                comissionDateTimePicker.Value = DateTime.Today;
            }
        }

        private void incentiveSettingsButton_Click(object sender, EventArgs e)
        {
            using (IncentiveConfigDialog dialog = new IncentiveConfigDialog())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    UpdateCommissionData();
                }
            }
        }

        private void comissionDataGrid_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = comissionDataGrid.Columns[e.ColumnIndex].Name;
            if (colName == "Incentive" || colName == "TotalPayable" || colName == "Salesman" || colName == "SalesmanNo")
            {
                string sNo = comissionDataGrid.Rows[e.RowIndex].Cells["SalesmanNo"].Value?.ToString() ?? "";
                string sName = comissionDataGrid.Rows[e.RowIndex].Cells["Salesman"].Value?.ToString() ?? "";
                string key = sNo + "|" + sName;

                if (currentCommissionStats != null && currentCommissionStats.ContainsKey(key))
                {
                    e.ToolTipText = currentCommissionStats[key].GetIncentiveTooltip();
                }
            }
        }

        private void printCommissionButton_Click(object sender, EventArgs e)
        {
            if (comissionDataGrid.DataSource == null || ((DataTable)comissionDataGrid.DataSource).Rows.Count == 0)
            {
                MessageBox.Show("No commission data available to print for the selected date.", "Print Commission", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            PaperSize a5Paper = new PaperSize("A5", 583, 827); // A5 size in hundredths of an inch (5.83" x 8.27")
            printDocument.DefaultPageSettings.PaperSize = a5Paper;
            printDocument.PrintPage += PrintDocument_PrintPage;

            printDialog.Document = printDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            DataTable dt_combined = (DataTable)comissionDataGrid.DataSource;
            if (dt_combined == null || dt_combined.Rows.Count == 0)
            {
                e.HasMorePages = false;
                return;
            }

            Graphics graphics = e.Graphics;
            int y = 20; // Start printing at top with nice margin

            // Print Title Header
            Font titleFont = new Font("Arial", 13, FontStyle.Bold);
            Font subTitleFont = new Font("Arial", 9, FontStyle.Regular);
            string titleText = $"{machine} Commission Report";
            string dateText = $"Date: {comissionDateTimePicker.Value.ToString("dd-MM-yyyy")} ({comissionDateTimePicker.Value.ToString("dddd")})";

            graphics.DrawString(titleText, titleFont, Brushes.Black, 20, y);
            y += 24;
            graphics.DrawString(dateText, subTitleFont, Brushes.Black, 20, y);
            y += 25;

            // Print Table Header
            Font headerFont = new Font("Arial", 9, FontStyle.Bold);
            Font rowFont = new Font("Arial", 9, FontStyle.Regular);
            Font totalFont = new Font("Arial", 9, FontStyle.Bold);
            Pen borderPen = new Pen(Color.Black, 1);

            int cellHeight = 22;
            int tableX = 20;
            // Columns: No (40), Salesman (185), Net Qty (60), Commission (105), Incentive (95), Total (95) -> Total = 580
            int[] columnWidths = { 40, 185, 60, 105, 95, 95 };
            StringFormat leftFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center };

            // Header Background & Borders
            Rectangle[] headerRects = {
                new Rectangle(tableX, y, columnWidths[0], cellHeight),
                new Rectangle(tableX + columnWidths[0], y, columnWidths[1], cellHeight),
                new Rectangle(tableX + columnWidths[0] + columnWidths[1], y, columnWidths[2], cellHeight),
                new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2], y, columnWidths[3], cellHeight),
                new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3], y, columnWidths[4], cellHeight),
                new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3] + columnWidths[4], y, columnWidths[5], cellHeight)
            };

            for (int i = 0; i < headerRects.Length; i++)
            {
                graphics.FillRectangle(Brushes.LightGray, headerRects[i]);
                graphics.DrawRectangle(borderPen, headerRects[i]);
            }

            graphics.DrawString("No.", headerFont, Brushes.Black, headerRects[0], centerFormat);
            graphics.DrawString("Salesman Name", headerFont, Brushes.Black, new Rectangle(headerRects[1].X + 4, headerRects[1].Y, headerRects[1].Width - 8, headerRects[1].Height), leftFormat);
            graphics.DrawString("Net Qty", headerFont, Brushes.Black, new Rectangle(headerRects[2].X + 4, headerRects[2].Y, headerRects[2].Width - 8, headerRects[2].Height), rightFormat);
            graphics.DrawString("Commission (₹)", headerFont, Brushes.Black, new Rectangle(headerRects[3].X + 4, headerRects[3].Y, headerRects[3].Width - 8, headerRects[3].Height), rightFormat);
            graphics.DrawString("Incentive (₹)", headerFont, Brushes.Black, new Rectangle(headerRects[4].X + 4, headerRects[4].Y, headerRects[4].Width - 8, headerRects[4].Height), rightFormat);
            graphics.DrawString("Total (₹)", headerFont, Brushes.Black, new Rectangle(headerRects[5].X + 4, headerRects[5].Y, headerRects[5].Width - 8, headerRects[5].Height), rightFormat);

            y += cellHeight;

            int totalQty = 0;
            decimal totalCommission = 0m;
            decimal totalIncentive = 0m;
            decimal totalPayable = 0m;

            // Print Data Rows
            foreach (DataRow row in dt_combined.Rows)
            {
                Rectangle[] rowRects = {
                    new Rectangle(tableX, y, columnWidths[0], cellHeight),
                    new Rectangle(tableX + columnWidths[0], y, columnWidths[1], cellHeight),
                    new Rectangle(tableX + columnWidths[0] + columnWidths[1], y, columnWidths[2], cellHeight),
                    new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2], y, columnWidths[3], cellHeight),
                    new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3], y, columnWidths[4], cellHeight),
                    new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3] + columnWidths[4], y, columnWidths[5], cellHeight)
                };

                for (int i = 0; i < rowRects.Length; i++)
                {
                    graphics.DrawRectangle(borderPen, rowRects[i]);
                }

                string sNo = row["SalesmanNo"] != DBNull.Value ? row["SalesmanNo"].ToString() : "";
                string sName = row["Salesman"] != DBNull.Value ? row["Salesman"].ToString() : "";
                int qty = row["Qty"] != DBNull.Value ? Convert.ToInt32(row["Qty"]) : 0;
                decimal comm = row["Comission"] != DBNull.Value ? Convert.ToDecimal(row["Comission"]) : 0m;
                decimal inc = dt_combined.Columns.Contains("Incentive") && row["Incentive"] != DBNull.Value ? Convert.ToDecimal(row["Incentive"]) : 0m;
                decimal tot = dt_combined.Columns.Contains("TotalPayable") && row["TotalPayable"] != DBNull.Value ? Convert.ToDecimal(row["TotalPayable"]) : (comm + inc);

                totalQty += qty;
                totalCommission += comm;
                totalIncentive += inc;
                totalPayable += tot;

                graphics.DrawString(sNo, rowFont, Brushes.Black, rowRects[0], centerFormat);
                graphics.DrawString(sName, rowFont, Brushes.Black, new Rectangle(rowRects[1].X + 4, rowRects[1].Y, rowRects[1].Width - 8, rowRects[1].Height), leftFormat);
                graphics.DrawString(qty.ToString("N0"), rowFont, Brushes.Black, new Rectangle(rowRects[2].X + 4, rowRects[2].Y, rowRects[2].Width - 8, rowRects[2].Height), rightFormat);
                graphics.DrawString(comm.ToString("N0"), rowFont, Brushes.Black, new Rectangle(rowRects[3].X + 4, rowRects[3].Y, rowRects[3].Width - 8, rowRects[3].Height), rightFormat);
                graphics.DrawString(inc.ToString("N0"), rowFont, Brushes.Black, new Rectangle(rowRects[4].X + 4, rowRects[4].Y, rowRects[4].Width - 8, rowRects[4].Height), rightFormat);
                graphics.DrawString(tot.ToString("N0"), rowFont, Brushes.Black, new Rectangle(rowRects[5].X + 4, rowRects[5].Y, rowRects[5].Width - 8, rowRects[5].Height), rightFormat);

                y += cellHeight;
            }

            // Print Grand Total Row
            Rectangle[] totalRects = {
                new Rectangle(tableX, y, columnWidths[0] + columnWidths[1], cellHeight),
                new Rectangle(tableX + columnWidths[0] + columnWidths[1], y, columnWidths[2], cellHeight),
                new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2], y, columnWidths[3], cellHeight),
                new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3], y, columnWidths[4], cellHeight),
                new Rectangle(tableX + columnWidths[0] + columnWidths[1] + columnWidths[2] + columnWidths[3] + columnWidths[4], y, columnWidths[5], cellHeight)
            };

            for (int i = 0; i < totalRects.Length; i++)
            {
                graphics.FillRectangle(Brushes.WhiteSmoke, totalRects[i]);
                graphics.DrawRectangle(borderPen, totalRects[i]);
            }

            graphics.DrawString("TOTAL", totalFont, Brushes.Black, new Rectangle(totalRects[0].X + 8, totalRects[0].Y, totalRects[0].Width - 16, totalRects[0].Height), leftFormat);
            graphics.DrawString(totalQty.ToString("N0"), totalFont, Brushes.Black, new Rectangle(totalRects[1].X + 4, totalRects[1].Y, totalRects[1].Width - 8, totalRects[1].Height), rightFormat);
            graphics.DrawString(totalCommission.ToString("N0"), totalFont, Brushes.Black, new Rectangle(totalRects[2].X + 4, totalRects[2].Y, totalRects[2].Width - 8, totalRects[2].Height), rightFormat);
            graphics.DrawString(totalIncentive.ToString("N0"), totalFont, Brushes.Black, new Rectangle(totalRects[3].X + 4, totalRects[3].Y, totalRects[3].Width - 8, totalRects[3].Height), rightFormat);
            graphics.DrawString(totalPayable.ToString("N0"), totalFont, Brushes.Black, new Rectangle(totalRects[4].X + 4, totalRects[4].Y, totalRects[4].Width - 8, totalRects[4].Height), rightFormat);

            y += cellHeight + 15;

            // Print summary timestamp
            Font footerFont = new Font("Arial", 8, FontStyle.Italic);
            graphics.DrawString($"Report generated on: {DateTime.Now.ToString("dd-MM-yyyy hh:mm tt")}", footerFont, Brushes.Gray, 20, y);

            e.HasMorePages = false; // Indicate no more pages
        }

        public void UpdateDateLabel()
        {
            DateTime selectedDate = dailyBalanceDateTimePicker.Value;
            dateLabel.Text = "Date: " + selectedDate.ToString("dd-MM-yyyy") + " (" + selectedDate.ToString("dddd") + ")";
        }

        private void dailyBalanceDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateDateLabel();

            UpdateCashGrid();
            UpdateTotalCash();

            UpdateCreditGrid();
            UpdateTotalCredit();

            UpdateDebitGrid();
            UpdateTotalDebit();

            dailyBalanceTab_Click(sender, e);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            StyleAllDataGrids();

            if (tabControl1.SelectedTab == dailyBalanceTab)
            {
                UpdateDateLabel();

                UpdateCashGrid();
                UpdateTotalCash();

                UpdateCreditGrid();
                UpdateTotalCredit();

                UpdateDebitGrid();
                UpdateTotalDebit();

                dailyBalanceTab_Click(sender, e);
            }
            else if (tabControl1.SelectedTab == commissionTab)
            {
                UpdateCommissionData();
            }
        }

        public void StyleGridCommon(DataGridView grid)
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
            grid.RowTemplate.Height = 24;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 249, 252);
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = Color.FromArgb(222, 226, 230);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }

        public void StyleDailyBalanceGrids()
        {
            StyleAllDataGrids();
        }

        public void StyleAllDataGrids()
        {
            DataGridView[] grids = { creditGridView, debitGridView, cashGridView, dataGridView1, dataGridViewPH, dataGridViewJunior, customerListDataGrid, comissionDataGrid };
            foreach (var grid in grids)
            {
                StyleGridCommon(grid);
            }

            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            if (creditGridView.Columns.Count > 1)
            {
                creditGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                creditGridView.Columns[1].Width = 85;
                creditGridView.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                creditGridView.Columns[1].DefaultCellStyle.Padding = new Padding(0, 0, 8, 0);
            }

            if (debitGridView.Columns.Count > 1)
            {
                debitGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                debitGridView.Columns[1].Width = 85;
                debitGridView.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                debitGridView.Columns[1].DefaultCellStyle.Padding = new Padding(0, 0, 8, 0);
            }

            if (cashGridView.Columns.Count > 2)
            {
                cashGridView.ScrollBars = ScrollBars.None;
                cashGridView.Columns[0].Width = 110;
                cashGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cashGridView.Columns[1].Width = 80;
                cashGridView.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cashGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                cashGridView.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                cashGridView.Columns[2].DefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
                cashGridView.Columns[2].DefaultCellStyle.Padding = new Padding(0, 0, 10, 0);
            }

            if (comissionDataGrid != null && comissionDataGrid.Columns.Count > 0)
            {
                if (comissionDataGrid.Columns.Contains("SalesmanNo"))
                {
                    comissionDataGrid.Columns["SalesmanNo"].HeaderText = "No.";
                    comissionDataGrid.Columns["SalesmanNo"].Width = 55;
                    comissionDataGrid.Columns["SalesmanNo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (comissionDataGrid.Columns.Contains("Salesman"))
                {
                    comissionDataGrid.Columns["Salesman"].HeaderText = "Salesman Name";
                    comissionDataGrid.Columns["Salesman"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                if (comissionDataGrid.Columns.Contains("Qty"))
                {
                    comissionDataGrid.Columns["Qty"].HeaderText = "Net Qty";
                    comissionDataGrid.Columns["Qty"].Width = 70;
                    comissionDataGrid.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    comissionDataGrid.Columns["Qty"].DefaultCellStyle.Format = "N0";
                    comissionDataGrid.Columns["Qty"].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
                }
                if (comissionDataGrid.Columns.Contains("TaxableAmt"))
                {
                    comissionDataGrid.Columns["TaxableAmt"].HeaderText = "Taxable Amount (₹)";
                    comissionDataGrid.Columns["TaxableAmt"].Width = 125;
                    comissionDataGrid.Columns["TaxableAmt"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    comissionDataGrid.Columns["TaxableAmt"].DefaultCellStyle.Format = "N0";
                    comissionDataGrid.Columns["TaxableAmt"].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
                }
                if (comissionDataGrid.Columns.Contains("Comission"))
                {
                    comissionDataGrid.Columns["Comission"].HeaderText = "Commission (₹)";
                    comissionDataGrid.Columns["Comission"].Width = 110;
                    comissionDataGrid.Columns["Comission"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    comissionDataGrid.Columns["Comission"].DefaultCellStyle.Format = "N0";
                    comissionDataGrid.Columns["Comission"].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
                }
                if (comissionDataGrid.Columns.Contains("Incentive"))
                {
                    comissionDataGrid.Columns["Incentive"].HeaderText = "Incentive (₹)";
                    comissionDataGrid.Columns["Incentive"].Width = 95;
                    comissionDataGrid.Columns["Incentive"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    comissionDataGrid.Columns["Incentive"].DefaultCellStyle.Format = "N0";
                    comissionDataGrid.Columns["Incentive"].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
                }
                if (comissionDataGrid.Columns.Contains("TotalPayable"))
                {
                    comissionDataGrid.Columns["TotalPayable"].HeaderText = "Total (₹)";
                    comissionDataGrid.Columns["TotalPayable"].Width = 100;
                    comissionDataGrid.Columns["TotalPayable"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    comissionDataGrid.Columns["TotalPayable"].DefaultCellStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
                    comissionDataGrid.Columns["TotalPayable"].DefaultCellStyle.Format = "N0";
                    comissionDataGrid.Columns["TotalPayable"].DefaultCellStyle.Padding = new Padding(0, 0, 6, 0);
                }
            }
        }

        private void prevDateButton_Click(object sender, EventArgs e)
        {
            dailyBalanceDateTimePicker.Value = dailyBalanceDateTimePicker.Value.AddDays(-1);
        }

        private void nextDateButton_Click(object sender, EventArgs e)
        {
            dailyBalanceDateTimePicker.Value = dailyBalanceDateTimePicker.Value.AddDays(1);
        }
    }

    public class CustomerSalesSummary
    {
        public string CustomerKey { get; set; }
        public string CustomerDisplay { get; set; }
        public List<string> VoucherNumbers { get; set; } = new List<string>();
        public decimal TotalAmount { get; set; }
    }

    public class BillIncentiveDetail
    {
        public List<string> VoucherNumbers { get; set; } = new List<string>();
        public string CustomerDisplay { get; set; }
        public decimal BillAmount { get; set; }
        public decimal IncentiveAmount { get; set; }
        public decimal SlabThreshold { get; set; }

        public string GetDisplayLabel()
        {
            if (VoucherNumbers == null || VoucherNumbers.Count == 0)
            {
                return $"• Bill: ₹ {BillAmount:N0} (+₹ {IncentiveAmount:N0})";
            }
            else if (VoucherNumbers.Count == 1)
            {
                return $"• Bill #{VoucherNumbers[0]}: ₹ {BillAmount:N0} (+₹ {IncentiveAmount:N0})";
            }
            else
            {
                string billsStr = string.Join(", #", VoucherNumbers);
                string custStr = !string.IsNullOrEmpty(CustomerDisplay) ? $" (Cust: {CustomerDisplay})" : "";
                return $"• Bills #{billsStr}{custStr}: ₹ {BillAmount:N0} (+₹ {IncentiveAmount:N0})";
            }
        }
    }

    public class SalesmanCommissionData
    {
        public string SalesmanNo { get; set; }
        public string SalesmanName { get; set; }
        public string Salesman => SalesmanName;
        public int SalesQty { get; set; }
        public int ReturnQty { get; set; }
        public int NetQty => SalesQty - ReturnQty;
        public decimal SalesTaxableAmt { get; set; }
        public decimal ReturnTaxableAmt { get; set; }
        public decimal NetTaxableAmt => SalesTaxableAmt - ReturnTaxableAmt;
        public decimal NetCommission => Math.Round(NetTaxableAmt * 0.01m, 0, MidpointRounding.AwayFromZero);
        public decimal Incentive { get; set; }
        public int IncentiveBillCount { get; set; }
        public decimal TotalPayable => NetCommission + Incentive;
        public List<BillIncentiveDetail> QualifyingBills { get; set; } = new List<BillIncentiveDetail>();

        public string GetIncentiveTooltip()
        {
            if (Incentive == 0 || QualifyingBills == null || QualifyingBills.Count == 0)
            {
                return $"Salesman: {SalesmanName}\nBase Commission: ₹ {NetCommission:N0}\nIncentive: ₹ 0 (No qualifying bills)";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"--- Incentive Breakdown: {SalesmanName} ---");
            int totalQualifyingBills = QualifyingBills.Sum(q => q.VoucherNumbers != null ? q.VoucherNumbers.Count : 1);
            sb.AppendLine($"Total Incentive: ₹ {Incentive:N0} (from {totalQualifyingBills} qualifying bill{(totalQualifyingBills > 1 ? "s" : "")})");
            sb.AppendLine("--------------------------------------------------");
            foreach (var item in QualifyingBills)
            {
                sb.AppendLine(item.GetDisplayLabel());
            }
            sb.AppendLine("--------------------------------------------------");
            sb.AppendLine($"Base Commission: ₹ {NetCommission:N0}");
            sb.Append($"Total Payout: ₹ {TotalPayable:N0}");
            return sb.ToString();
        }
    }

    public class IncentiveSlab
    {
        public decimal MinAmount { get; set; }
        public decimal IncentiveAmount { get; set; }

        public IncentiveSlab() { }
        public IncentiveSlab(decimal minAmount, decimal incentiveAmount)
        {
            MinAmount = minAmount;
            IncentiveAmount = incentiveAmount;
        }
    }

    public class IncentiveManager
    {
        private static string ConfigFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IncentiveSettings.txt");

        public static bool Enabled { get; set; } = true;
        public static List<IncentiveSlab> Slabs { get; set; } = new List<IncentiveSlab>();

        static IncentiveManager()
        {
            LoadSettings();
        }

        public static void LoadSettings()
        {
            Slabs.Clear();
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string[] lines = File.ReadAllLines(ConfigFilePath);
                    foreach (var line in lines)
                    {
                        string trimmed = line.Trim();
                        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;

                        if (trimmed.StartsWith("Enabled=", StringComparison.OrdinalIgnoreCase))
                        {
                            bool en;
                            if (bool.TryParse(trimmed.Substring(8).Trim(), out en))
                            {
                                Enabled = en;
                            }
                        }
                        else if (trimmed.Contains(":"))
                        {
                            string[] parts = trimmed.Split(':');
                            if (parts.Length == 2)
                            {
                                decimal minAmt, incAmt;
                                if (decimal.TryParse(parts[0].Trim(), out minAmt) && decimal.TryParse(parts[1].Trim(), out incAmt))
                                {
                                    Slabs.Add(new IncentiveSlab(minAmt, incAmt));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading incentive settings: " + ex.Message);
            }

            // Defaults if empty
            if (Slabs.Count == 0)
            {
                Enabled = true;
                Slabs.Add(new IncentiveSlab(5000, 15));
                Slabs.Add(new IncentiveSlab(7500, 20));
            }

            Slabs = Slabs.OrderBy(s => s.MinAmount).ToList();
        }

        public static void SaveSettings(bool enabled, List<IncentiveSlab> slabs)
        {
            Enabled = enabled;
            Slabs = slabs.OrderBy(s => s.MinAmount).ToList();
            try
            {
                List<string> lines = new List<string>();
                lines.Add("# Salesman Commission Incentive Settings");
                lines.Add("Enabled=" + (Enabled ? "true" : "false"));
                foreach (var slab in Slabs)
                {
                    lines.Add(slab.MinAmount.ToString("0") + ":" + slab.IncentiveAmount.ToString("0"));
                }
                File.WriteAllLines(ConfigFilePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving incentive settings: " + ex.Message);
            }
        }

        public static decimal CalculateBillIncentive(decimal billAmount)
        {
            decimal threshold;
            return CalculateBillIncentiveWithThreshold(billAmount, out threshold);
        }

        public static decimal CalculateBillIncentiveWithThreshold(decimal billAmount, out decimal matchedThreshold)
        {
            matchedThreshold = 0m;
            if (!Enabled || Slabs == null || Slabs.Count == 0 || billAmount <= 0) return 0m;

            var sortedDesc = Slabs.OrderByDescending(s => s.MinAmount);
            foreach (var slab in sortedDesc)
            {
                if (billAmount >= slab.MinAmount)
                {
                    matchedThreshold = slab.MinAmount;
                    return slab.IncentiveAmount;
                }
            }
            return 0m;
        }
    }
}
