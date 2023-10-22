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
using System.Configuration;

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
        DataTable phNewBills = new DataTable();
        DataTable jrNewBills = new DataTable();
        List<List<String>> updationList= new List<List<String>>();
        List<List<String>> modifiedList = new List<List<string>>();



        public string paytmId = "";
        public string bajajId = "";

        float balance = 0;

        public void UpdateItemValueDictionary()
        {
            //itemValue.Add("00450", 3000);
            //itemValue.Add("00451", 6000);
            //itemValue.Add("00811", 10000);
            //itemValue.Add("00837", 5000);
            //itemValue.Add("00838", 8000);
            //itemValue.Add("00839", 15000);   
            //itemValue.Add("00281", 3000);
            //itemValue.Add("00283", 6000);
            //itemValue.Add("00310", 10000);
            //itemValue.Add("00282", 5000);
            //itemValue.Add("00413", 8000);
            //itemValue.Add("00414", 15000);
            //itemValue.Add("00125", 3000);
            //itemValue.Add("00126", 3000);

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

            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            chk.Name = "Update";
            chk.Width = 50;
            chk.ReadOnly = false;         
            dataGridView1.Columns.Add(chk);

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
                lastUpdatedList.Add(row["PH"].ToString());
                lastUpdatedList.Add(row["Junior"].ToString());
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

            dataGridViewPH.ColumnCount = 5;
            dataGridViewPH.Columns[0].Name = "Date";
            dataGridViewPH.Columns[1].Name = "Bill No";
            dataGridViewPH.Columns[2].Name = "Mobile";
            dataGridViewPH.Columns[3].Name = "Name";
            dataGridViewPH.Columns[4].Name = "Amount";
            //dataGridViewPH.Columns[5].Name = "Add Bag";

            dataGridViewPH.Columns[0].Width = 90;
            dataGridViewPH.Columns[1].Width = 80;
            dataGridViewPH.Columns[4].Width = 80;

            foreach(DataRow row in dt.Rows)
            {
                dataGridViewPH.Rows.Add(((DateTime)row["VoucherDate"]).ToString("dd-MM-yyyy"), row["VoucherNo"], row["MobileNo"], row["AccountName"], row["NetAmt"]);
            }

            dataGridViewJunior.Rows.Clear();

            dataGridViewJunior.ColumnCount = 5;
            dataGridViewJunior.Columns[0].Name = "Date";
            dataGridViewJunior.Columns[1].Name = "Bill No";
            dataGridViewJunior.Columns[2].Name = "Mobile";
            dataGridViewJunior.Columns[3].Name = "Name";
            dataGridViewJunior.Columns[4].Name = "Amount";
            //dataGridViewJunior.Columns[5].Name = "Add Bag";

            dataGridViewJunior.Columns[0].Width = 90;
            dataGridViewJunior.Columns[1].Width = 80;
            dataGridViewJunior.Columns[4].Width = 80;

            foreach (DataRow row in dt2.Rows)
            {
                dataGridViewJunior.Rows.Add(((DateTime)row["VoucherDate"]).ToString("dd-MM-yyyy"), row["VoucherNo"], row["MobileNo"], row["AccountName"], row["NetAmt"]);
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

        //public void GetModifiedListFromDT(DataTable dt, String dbname)
        //{
        //    //modifiedList.Clear();

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        List<String> temp = new List<string>();

        //        temp.Add(row["CO_CODE"].ToString());  //[0]
        //        temp.Add(row["LC_CODE"].ToString());  //[1]
        //        temp.Add(row["CO_YEAR"].ToString());  //[2]
        //        temp.Add(row["DB_CODE"].ToString());  //[3]
        //        temp.Add(row["TRN_TYPE"].ToString()); //[4]
        //        temp.Add(row["BIL_NO"].ToString());   //[5]
        //        temp.Add(row["SR_NO"].ToString());    //[6]
        //        temp.Add(dbname);                     //[7]
        //        temp.Add(row["IT_QTY"].ToString());   //[8]
        //        temp.Add(row["IT_CODE"].ToString());  //[9]

        //        modifiedList.Add(temp);
        //    }
            
        //}

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

            // GET EXCHANGE BILLS FROM DAYBOOK CODE 403

            //phNewBills.Merge(DBHandler.GetPendingBills(lastUpdatedList[4], "GRExtreme_PantHouseJ", "403", "2019-2020", "LENOVO-PC\\SQL2008"));
            /*phNewBills.Merge(DBHandler.GetPendingBills(lastUpdatedList[4], tphDBName, "403", "2019-2020", tphServerName));
            if (phNewBills.Rows.Count - oldListSize > 0)
            {
                lastUpdatedList[4] = phNewBills.Rows[phNewBills.Rows.Count - 1]["VoucherNo"].ToString();
                //MessageBox.Show("Ph Exch Bill: " + lastUpdatedList[4]);
            }
            */
            foreach (DataRow row in phNewBills.Rows)
            {
                DateTime date = (DateTime)row["VoucherDate"];
                String voucherDate = date.ToString("MM-dd-yyyy");
                UpdateListOfBills(row["MobileNo"].ToString(), row["DayBookID"].ToString(), row["VoucherNo"].ToString(), "PH", row["NetAmt"].ToString(), "1", year, "", voucherDate, row["AccountName"].ToString(), "", row["FinYearID"].ToString(), row["SalesID"].ToString());
                //UpdateListOfBills(row["MobileNo"].ToString(), row["DayBookID"].ToString(), row["VoucherNo"].ToString(), "PH", "0", "1", year, "", voucherDate, row["AccountName"].ToString(), "", row["FinYearID"].ToString(), row["SalesID"].ToString());
                //MessageBox.Show("PH " + row["AccountName"] + " " + row["VoucherNo"] + " " + row["DayBookID"]);
            }

            //jrNewBills = DBHandler.GetPendingBills(lastUpdatedList[2], "G_PANTHOUSE", "401", "2019-2020", "HP-PC\\SQL2008");
            jrNewBills = DBHandler.GetPendingBills(lastUpdatedList[2], jrDBName, "401", "2019-2020", jrServerName, jrFinYearID);
            if (jrNewBills.Rows.Count > 0)
            {
                oldListSize = jrNewBills.Rows.Count;
                lastUpdatedList[2] = jrNewBills.Rows[jrNewBills.Rows.Count - 1]["VoucherNo"].ToString();
            }
            //jrNewBills.Merge(DBHandler.GetPendingBills(lastUpdatedList[5], "G_PANTHOUSE", "403", "2019-2020", "HP-PC\\SQL2008"));
            
            // GET EXCHANGE BILLS FROM DAYBOOK CODE 403
            /*jrNewBills.Merge(DBHandler.GetPendingBills(lastUpdatedList[5], jrDBName, "403", "2019-2020", jrServerName));
            if (jrNewBills.Rows.Count - oldListSize > 0)
            {
                lastUpdatedList[5] = jrNewBills.Rows[jrNewBills.Rows.Count - 1]["VoucherNo"].ToString();
            }*/
            foreach (DataRow row in jrNewBills.Rows)
            {
                DateTime date = (DateTime)row["VoucherDate"];
                String voucherDate = date.ToString("MM-dd-yyyy");
                UpdateListOfBills(row["MobileNo"].ToString(), row["DayBookID"].ToString(), row["VoucherNo"].ToString(), "Junior", row["NetAmt"].ToString(), "1", year, "", voucherDate, row["AccountName"].ToString(), "", row["FinYearID"].ToString(), row["SalesID"].ToString());
                //UpdateListOfBills(row["MobileNo"].ToString(), row["DayBookID"].ToString(), row["VoucherNo"].ToString(), "Junior", "0", "1", year, "", voucherDate, row["AccountName"].ToString(), "", row["FinYearID"].ToString(), row["SalesID"].ToString());
                //MessageBox.Show("Junior " + row["AccountName"] + " " + row["VoucherNo"] + " " + row["DayBookID"]);
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
                    DataTable dt = DBHandler.SelectQueryOnTable("BillDetails", "SUM(Points) - SUM(PointsRedeemed) as ActualBalance", "WHERE Mobile = '" + bill[0] + "'");
                    foreach (DataRow row in dt.Rows)
                    {
                        old_bal = int.Parse(row["ActualBalance"].ToString());
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
            
            //String phSale = DBHandler.GetLastBill("PH", "401", "HP-PC\\SQLExpress", "CustomerLoyalty", year);
            //String phExch = DBHandler.GetLastBill("PH", "403", "HP-PC\\SQLExpress", "CustomerLoyalty", year);
            //String juniorSale = DBHandler.GetLastBill("Junior", "401", "HP-PC\\SQLExpress", "CustomerLoyalty", year);
            //String juniorExch = DBHandler.GetLastBill("Junior", "403", "HP-PC\\SQLExpress", "CustomerLoyalty", year);

            String phSale = DBHandler.GetLastBill("PH", "401", hostServerName, hostDBName, year);
            String phExch = DBHandler.GetLastBill("PH", "403", hostServerName, hostDBName, year);
            String juniorSale = DBHandler.GetLastBill("Junior", "401", hostServerName, hostDBName, year);
            String juniorExch = DBHandler.GetLastBill("Junior", "403", hostServerName, hostDBName, year);

            DBHandler.AddTransaction("LastUpdate", machine + "-App", "Updated LastUpdated", "PHSale = " + lastUpdatedList[1] + ", JuniorSale = " + lastUpdatedList[2] + ", PHExch = " + lastUpdatedList[4] + ", JuniorExch = " + lastUpdatedList[5], "PHSale = " + phSale + ", JuniorSale = " + juniorSale + ", PHExch = " + phExch + ", JuniorExch = " + juniorExch, "");

            lastUpdatedList[1] = phSale;
            lastUpdatedList[2] = juniorSale;
            lastUpdatedList[4] = phExch;
            lastUpdatedList[5] = juniorExch;

            DBHandler.UpdateLastUpdated(lastUpdatedList);

            return 1;
        }

        //public void GetNewBags()
        //{
        //    DataTable dt = DBHandler.GetModifiedBills("THEPANTNEW");
        //    GetModifiedListFromDT(dt, "THEPANTNEW");

        //    dt = DBHandler.GetModifiedBills("PHOUSE_JUNIOUR");
        //    GetModifiedListFromDT(dt, "PHOUSE_JUNIOUR");           
        //}

        //public void UpdateNewBags()
        //{
        //    foreach (List<String> bill in modifiedList)
        //    {
        //        //GET MOBILE OF THE CUSTOMER WHOSE BILL IS BEING UPDATED
        //        String mobile = DBHandler.GetCustomerMobile(bill[5], bill[3], bill[2], bill[7]);
                
        //        //GET CURRENT BALANCE OF THE CUSTOMER
        //        int old_bal = DBHandler.GetBalance(mobile);

        //        //GET NEW BALANCE OF THE CUSTOMER
        //        int new_bal = old_bal - (int.Parse(bill[8]) * itemValue[bill[9]]);

        //        //UPDATE BALANCE OF CUSTOMER IN DB
        //        DBHandler.UpdateBalance(mobile, new_bal.ToString());

        //        //UPDATE THE BagsGiven COLUMN OF BillDetails TABLE
        //        DataTable dt = DBHandler.GetBagsGiven(bill[5], bill[7], bill[3], bill[2]);
        //        String bagsGiven = "";
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            bagsGiven += row["IT_QTY"].ToString() + "x" + itemValue[row["IT_CODE"].ToString()] + " ";
        //        }
        //        bagsGiven += bill[8] + "x" + itemValue[bill[9]];
        //        String year = bill[2].Substring(2,2) + bill[2].Substring(7);
        //        DBHandler.UpdateBagsGiven(mobile, bill[3], bill[5], dbnameReverseDict[bill[7]], year, bagsGiven); 

        //        //SET Altr to 'N' SO THAT THE BAG DOESN'T GET SCANNED NEXT TIME THE PROGRAM IS RUN
        //        DBHandler.RemoveModifiedFlag(bill[0], bill[1], bill[2], bill[3], bill[4], bill[5], bill[6], bill[7]);
        //    }
        //}


        


        //public Home(SplashScreen splashScreen)
        public void UpdateCashGrid()
        {
            int opCash = 0;
            cashGridView.Rows.Clear();
            int x2000 = 0, x500 = 0, x200 = 0, x100 = 0, x50 = 0, x20 = 0, x10 = 0, x5 = 0;

            //Replace HP-PC with machine****************************
            //DataTable dt = DBHandler.SelectQueryOnTable("DailyCash", "*", "WHERE Date = '" + DateTime.Now.Date.ToString("yyyy-MM-dd").ToString() + "' AND Machine = 'HP-PC' AND Initialised = '1'");
            DataTable dt = DBHandler.SelectQueryOnTable("DailyCash", "*", "WHERE Date = '" + DateTime.Now.Date.ToString("yyyy-MM-dd").ToString() + "' AND Machine = '" + machine + "' AND Initialised = '1'");

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
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
        }

        public void UpdateCreditGrid()
        {
            List<String> crParticular = new List<string>();
            List<String> crAmount = new List<string>();
            creditGridView.Rows.Clear();

            //Replace HP-PC with machine****************************
            //DataTable dt = DBHandler.SelectQueryOnTable("DailyTransactions", "*", "WHERE Machine = 'HP-PC' AND Type = 'CR'");
            DataTable dt = DBHandler.SelectQueryOnTable("DailyTransactions", "*", "WHERE Machine = '" + machine + "' AND Type = 'CR'");

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
            List<String> crParticular = new List<string>();
            List<String> crAmount = new List<string>();
            debitGridView.Rows.Clear();

            //Replace HP-PC with machine****************************
            //DataTable dt = DBHandler.SelectQueryOnTable("DailyTransactions", "*", "WHERE Machine = 'HP-PC' AND Type = 'DR'");
            DataTable dt = DBHandler.SelectQueryOnTable("DailyTransactions", "*", "WHERE Machine = '" + machine + "' AND Type = 'DR'");

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
            for(int i=0; i<creditGridView.Rows.Count; i++)
            {
                int value = 0;
                value = int.Parse(creditGridView.Rows[i].Cells[1].Value.ToString());
                total += value;
                
            }
            totalCreditLabel.Text = total.ToString();
            return total;
        }
        
        public int UpdateTotalDebit()
        {
            int total = 0;
            for (int i = 0; i < debitGridView.Rows.Count; i++)
            {
                int value = 0;
                value = int.Parse(debitGridView.Rows[i].Cells[1].Value.ToString());
                total += value;
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

            int difference = calculatedBalance - totalCash;

            if (difference == 0)
            {
                balanceMatchLabel.Text = "Yes";
                balanceMatchLabel.ForeColor = Color.Green;
            }

            else
            {
                balanceMatchLabel.Text = difference.ToString();
                balanceMatchLabel.ForeColor = Color.Red;
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
                DBHandler.ClearDailyTransactions(machine);

                OpeningCashDialog cashDialog = new OpeningCashDialog();
                
                if (cashDialog.ShowDialog(this) == DialogResult.OK)
                {
                    String openingCash = cashDialog.opBalDialogText.Text.ToString();
                    creditGridView.Rows.Add("Opening Balance", cashDialog.opBalDialogText.Text.ToString());
                    creditGridView.Rows[0].ReadOnly = true;

                    //Replace HP-PC with machine****************************
                    //DBHandler.InitializeDailyCash(DateTime.Now.Date.ToString("yyyy-MM-dd").ToString(), "HP-PC", "1");
                    //DBHandler.AddDailyTransactions("HP-PC", "CR", "Opening Balance", openingCash);
                    DBHandler.InitializeDailyCash(DateTime.Now.Date.ToString("yyyy-MM-dd").ToString(), machine, "1");
                    DBHandler.AddDailyTransactions(machine, "CR", "Opening Balance", openingCash); 
                }
            }
        }

        public Home()
        
        {
            InitializeComponent();

            //if (machine.Equals("SNEH-PC")) machine = jrMachineName;
            //if (machine.Equals("ACER-Laptop")) machine = jrMachineName;
            if (!(machine.Equals(jrMachineName) || machine.Equals(tphMachineName))) machine = jrMachineName;
            
            //if (machine.Equals("HP-PC"))
            if (machine.Equals(jrMachineName))
            {
                paytmId = "149545";
                bajajId = "149546";
            }

            //else if (machine.Equals("LENOVO-PC"))
            else if (machine.Equals(tphMachineName))
            {
                paytmId = "219587";
                bajajId = "219588";
            }

            LogWriter log = new LogWriter("Initializing App! " + DateTime.Now.ToString());

            //dbnameDict.Add("PH", "GRExtreme_PantHouseJ");
            dbnameDict.Add("PH", tphDBName);

            //dbnameDict.Add("Junior", "G_PANTHOUSE");
            dbnameDict.Add("Junior", jrDBName);

            //machineDbNameDict.Add("LENOVO-PC", "GRExtreme_PantHouseJ");
            machineDbNameDict.Add(tphMachineName, tphDBName);
            //machineDbNameDict.Add("HP-PC", jrDBName);
            machineDbNameDict.Add(jrMachineName, jrDBName);

            machineServerNameDict.Add(tphMachineName, tphServerName);
            machineServerNameDict.Add(jrMachineName, jrServerName);

            //dbnameReverseDict.Add("GRExtreme_PantHouseJ", "PH");
            dbnameReverseDict.Add(tphMachineName, "PH");
            //dbnameReverseDict.Add("G_PANTHOUSE", "Junior");
            dbnameReverseDict.Add(jrMachineName, "Junior");

            //servernameDict.Add("PH", "LENOVO-PC\\SQL2008");
            servernameDict.Add("PH", tphServerName);
            //servernameDict.Add("Junior", "HP-PC\\SQL2008");
            servernameDict.Add("Junior", jrServerName);

            dateLabel.Text = dateLabel.Text.ToString() + DateTime.Now.ToShortDateString();

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
            //SqlConnection con = DBHandler.ConnectToDB("LENOVO-PC\\SQL2008", "GRetailExtreme_THEPANTHOUSEE");
            //try
            //{
            //    con.Open();
            //}
            //catch (Exception exception)
            //{
            //    MessageBox.Show("Unable to Connect to Server LENOVO-PC\\SQL2008");
            //}
            //if (con.State == ConnectionState.Open)
            //    label3.ForeColor = Color.Green;
            //con.Close();

            //SqlConnection con2 = DBHandler.ConnectToDB("HP-PC\\SQL2008", "GRetailExtreme_PANTHOUSE");
            //con2.Open();
            //if (con2.State == ConnectionState.Open)
            //    label4.ForeColor =Color.Green;
            //con2.Close();


        
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
            String mobile = dataGridViewPH.Rows[e.RowIndex].Cells["Mobile"].Value.ToString();
            textBox1.Text = mobile;
        }

        private void dataGridViewJunior_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            String mobile = dataGridViewJunior.Rows[e.RowIndex].Cells["Mobile"].Value.ToString();
            textBox1.Text = mobile;
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
            SaleParameters sale = DBHandler.getDailySales(machineServerNameDict[machine], machineDbNameDict[machine], DateTime.Now.Date.ToString("yyyy-MM-dd"), paytmId, bajajId);

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
                SaleParameters sale = DBHandler.getDailySales(machineServerNameDict[machine], machineDbNameDict[machine], DateTime.Now.Date.ToString("yyyy-MM-dd"), paytmId, bajajId);

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

            //Replace HP-PC with machine****************************
            //int denominationUpdated = DBHandler.UpdateDenomination(denomination.ToString(), nos.ToString(), "HP-PC");
            int denominationUpdated = DBHandler.UpdateDenomination(denomination.ToString(), nos.ToString(), machine);

            row.Cells["Total"].Value = (nos * denomination).ToString();
            CheckBalanceMatch();
            UpdateCalculatedBalance();
        }

        private void addCreditButton_Click(object sender, EventArgs e)
        {
            String particular = creditParticular.Text.ToString();
            String amount = creditAmount.Text.ToString();

            //Replace HP-PC with machine****************************
            //DBHandler.AddDailyTransactions("HP-PC", "CR", particular, amount);
            DBHandler.AddDailyTransactions(machine, "CR", particular, amount);
            
            UpdateCreditGrid();
            UpdateTotalCredit();
            CheckBalanceMatch();
            UpdateCalculatedBalance();

            creditParticular.Text = "";
            creditAmount.Text = "";
        }

        private void addDebitButton_Click(object sender, EventArgs e)
        {
            String particular = debitParticular.Text.ToString();
            String amount = debitAmount.Text.ToString();

            //Replace HP-PC with machine****************************
            //DBHandler.AddDailyTransactions("HP-PC", "DR", particular, amount);
            DBHandler.AddDailyTransactions(machine, "DR", particular, amount);
            
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
                source = "Junior";
            }
            else
            {
                toEmail = "kishor.nagrecha@gmail.com";
                source = "PH";
            }

            WriteToExcel.writeToExcel(this, source);

            WriteToExcel.sendEmail(toEmail, source);

            MessageBox.Show("Daily Balance Submitted Successfully!");
        }

        private void deleteCreditButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in creditGridView.Rows)
            {
                if (Convert.ToBoolean(row.Cells["Delete"].Value))
                {
                    //MessageBox.Show(row.Cells["Particulars"].Value.ToString() + " Selected");
                    creditGridView.Rows.RemoveAt(row.Index);
                }
            }

            CheckBalanceMatch();
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

            totalAccountsLabel.Text = "Total Accounts: " + customerListDataGrid.Rows.Count;

        }

        private void exportButton_Click(object sender, EventArgs e)
        {
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
            app.Quit();
        }

        private void dataGridViewPH_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

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

                    bool isNumeric = int.TryParse(newPoints, out int newPointsInt);

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
    }
}
