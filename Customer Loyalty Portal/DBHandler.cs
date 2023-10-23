using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer_Loyalty_Portal
{
    class DBHandler
    {
        public static String hostServerName = Home.hostServerName;
        public static String hostDBName = Home.hostDBName;

        public static SqlConnection ConnectToDB(String serverName, String dbname)
        {
            SqlConnection con = null;
            String ConnectionString = "Server=" + serverName + ";Initial Catalog=" + dbname + ";UID=sa;PWD=aa;Pooling=False";
            //String ConnectionString = ConfigurationManager.ConnectionStrings["StockHP"].ConnectionString;
            //String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Stock"].ToString();
            
            //SqlConnection con = new SqlConnection(ConnectionString);
            con = new SqlConnection(ConnectionString);
            
            //Console.WriteLine(ConnectionString);

            return con;
        }

        public static SaleParameters getDailySales(String serverName, String dbname, String date, string paytmId, string bajajId)
        {
            int netAmt = 0;
            int cashAmt = 0;
            int cardAmt = 0;
            int debitAmt = 0;
            int balReceivedAmt_cash = 0;
            int balReceivedAmt_card = 0;
            int paytm = 0;
            int bajaj = 0;
            int crntIssued = 0;
            int crntRecv = 0;

            SqlConnection con = ConnectToDB(serverName, dbname);

           
            //String query = "SELECT BIL_NO, DB_CODE, BIL_DT, CO_YEAR, PHONE, TOT_AMT, AC_NAME FROM SALE_DATA WHERE BIL_NO > '" + billNo + "' AND DB_CODE = '" + dbCode + "' AND CO_YEAR = '" + year + "'";
            String query = "SELECT SUM(NetAmt) AS NetAmt, SUM(CardAmt) AS CardAmt, SUM(CashAmt) AS CashAmt, SUM(OsAmt) AS OsAmt, SUM(CreditNoteIssueAmt) AS CrntIssued, SUM(CreditNoteUseAmt) AS CrntRecv FROM trnSales WHERE VoucherDate = '" + date + "'";
            String query2 = "SELECT SUM(CashAmt) AS CashReceiptAmt, SUM(CardAmt) AS CardReceiptAmt FROM trnBillWiseReceiptAcct WHERE AddDate >= '" + date + "'";
            String query3 = "SELECT CardAccountID, SUM(CardAmt) AS AltCardAmt FROM trnPaymentDetail WHERE AddDate >= '" + date + "' AND CardAccountID IN (" + paytmId + "," + bajajId + ") GROUP BY CardAccountID";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            log = new LogWriter(query2);
            Console.WriteLine(query2);

            log = new LogWriter(query3);
            Console.WriteLine(query3);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);
            SqlDataAdapter adapter2 = new SqlDataAdapter(query2, con);
            SqlDataAdapter adapter3 = new SqlDataAdapter(query3, con);

            DataTable dt = new DataTable();
            DataTable dt2 = new DataTable();
            DataTable dt3 = new DataTable();

            adapter.Fill(dt);
            adapter2.Fill(dt2);
            adapter3.Fill(dt3);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["NetAmt"].ToString().Length > 0) netAmt = int.Parse(dt.Rows[0]["NetAmt"].ToString());
                if (dt.Rows[0]["CashAmt"].ToString().Length > 0) cashAmt = int.Parse(dt.Rows[0]["CashAmt"].ToString());
                if (dt.Rows[0]["CardAmt"].ToString().Length > 0) cardAmt = int.Parse(dt.Rows[0]["CardAmt"].ToString());
                if (dt.Rows[0]["OsAmt"].ToString().Length > 0) debitAmt = int.Parse(dt.Rows[0]["OsAmt"].ToString());
                if (dt.Rows[0]["CrntIssued"].ToString().Length > 0) crntIssued = int.Parse(dt.Rows[0]["CrntIssued"].ToString());
                if (dt.Rows[0]["CrntRecv"].ToString().Length > 0) crntRecv = int.Parse(dt.Rows[0]["CrntRecv"].ToString());
            }

            if (dt2.Rows.Count > 0)
            {
                if (dt2.Rows[0]["CashReceiptAmt"].ToString().Length > 0) balReceivedAmt_cash = int.Parse(dt2.Rows[0]["CashReceiptAmt"].ToString());

                if (dt2.Rows[0]["CardReceiptAmt"].ToString().Length > 0) balReceivedAmt_card = int.Parse(dt2.Rows[0]["CardReceiptAmt"].ToString());
            }

            if (dt3.Rows.Count > 0)
            {
                foreach (DataRow row in dt3.Rows)
                {
                    if (row["CardAccountID"].ToString().Equals(paytmId)) paytm = int.Parse(row["AltCardAmt"].ToString());
                    if (row["CardAccountID"].ToString().Equals(bajajId)) bajaj = int.Parse(row["AltCardAmt"].ToString());
                }
            }

            cardAmt = cardAmt - paytm - bajaj;
            
            SaleParameters sales  = new SaleParameters(netAmt, cashAmt, cardAmt, debitAmt, balReceivedAmt_cash, balReceivedAmt_card, paytm, bajaj, crntIssued, crntRecv);

            return sales;
        }

        public static void AddTransaction(String tableName, String source, String transaction, String oldValue, String newValue, String mobile)
        {
            //SqlConnection con = ConnectToDB("HP-PC\\SQLExpress", "CustomerLoyalty");
            SqlConnection con = ConnectToDB(hostServerName, hostDBName);

            String query = "INSERT INTO Transactions VALUES('" + tableName + "', '" + source + "', '" + transaction + "', '" + oldValue + "', '" + newValue + "', CURRENT_TIMESTAMP, '" + mobile + "')";
            //InsertIntoTable("Transactions", "", "'" + table + "', 'Added New Record', '', '" + values + "', CURRENT_TIMESTAMP");
            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

             cmd.ExecuteNonQuery();

            con.Close();
        }

        public static int GetSales(String serverName, String dbname, String date)
        {
            int totSales = 0;

            SqlConnection con = ConnectToDB(serverName, dbname);

            //String query = "SELECT BIL_NO, DB_CODE, BIL_DT, CO_YEAR, PHONE, TOT_AMT, AC_NAME FROM SALE_DATA WHERE BIL_NO > '" + billNo + "' AND DB_CODE = '" + dbCode + "' AND CO_YEAR = '" + year + "'";
            String query = "SELECT SUM(NetAmt) FROM trnSales WHERE VoucherDate = '" + date + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString().Length > 0) totSales = int.Parse(dt.Rows[0][0].ToString());
            
            return totSales;
        }

        public static DataTable GetDetailedSales(String serverName, String dbname, String cardAccountID = "", string date = "")
        {
            int totSales = 0;

            string cardAccountIDClause = "";

            if(cardAccountID.Length > 0)
            {
                cardAccountIDClause = " AND trnPaymentDetail.CardAccountID = '" + cardAccountID + "' ";
            }

            SqlConnection con = ConnectToDB(serverName, dbname);

            if (date.Length == 0) date = DateTime.Now.ToString("D");

            //String query = "SELECT BIL_NO, DB_CODE, BIL_DT, CO_YEAR, PHONE, TOT_AMT, AC_NAME FROM SALE_DATA WHERE BIL_NO > '" + billNo + "' AND DB_CODE = '" + dbCode + "' AND CO_YEAR = '" + year + "'";
            String query = $"SELECT trnSales.VoucherNo, trnSales.MobileNo, trnSales.AccountName, trnPaymentDetail.CardTotAmt, trnSales.NetAmt FROM trnSales INNER JOIN trnPaymentDetail ON trnSales.SalesID = trnPaymentDetail.ModuleID WHERE trnSales.VoucherDate >= '{date}' {cardAccountIDClause} ORDER BY trnSales.VoucherNo DESC";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            return dt;
        }

        public static DataTable GetModifiedBills(String serverName, String dbname)
        {
            DataTable dt = new DataTable();

            SqlConnection con = ConnectToDB(serverName, dbname);

            String items = "";
            //if (dbname.Equals("THEPANTNEW"))
            //{
            //    items = "('00450', '00451', '00811', '00837', '00838', '00839')";
            //}

            //else
            //{
            //    items = "('00281', '00283', '00310', '00282', '00413', '00414')";
            //}

            //if (serverName.Equals("LENOVO-PC\\SQL2008"))
            if (serverName.Equals("SNEH-CUSTOM"))
            {
                items = "('001451', '001452', '001812', '001838', '001839', '001840')";
            }

            else
            {
                items = "('001282', '001284', '001311', '001283', '001414', '001415')";
            }

            String query = "SELECT CO_CODE, LC_CODE, CO_YEAR, DB_CODE, TRN_TYPE, BIL_NO, SR_NO, IT_CODE, IT_QTY FROM SALE_DETL WHERE IT_CODE IN " + items + " AND ALTR = 'Y'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            adapter.Fill(dt);
            return dt;
        }

        public static DataTable GetPendingBills(String voucherNo, String dbname, String dbCode,  String year, String serverName, string finYearID)
        {
            SqlConnection con = ConnectToDB(serverName, dbname);
            DataTable dt = new DataTable();

            //String query = "SELECT BIL_NO, DB_CODE, BIL_DT, CO_YEAR, PHONE, TOT_AMT, AC_NAME FROM SALE_DATA WHERE BIL_NO > '" + billNo + "' AND DB_CODE = '" + dbCode + "' AND CO_YEAR = '" + year + "'";
            String query = "SELECT SalesID, VoucherNo, DayBookID, VoucherDate, FinYearID, MobileNo, NetAmt, AccountName FROM trnSales WHERE VoucherNo > '" + voucherNo + "' AND DayBookID = '" + dbCode + "' AND FinYearID = '" + finYearID + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);


                adapter.Fill(dt);
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dt;
        }

        public static String GetLastBill(String source, String dbcode, String serverName, String dbname, string year)
        {
            String lastBill = "";

            SqlConnection con = ConnectToDB(serverName, dbname);
            String query = "SELECT MAX(BillNo) FROM BillDetails WHERE BookCode = '" + dbcode + "' AND Source = '" + source + "' AND Year = '" + year + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString().Length > 0) lastBill = row[0].ToString();
            }

            return lastBill;
        }

        //public static int UpdateMobile(String oldMobile, String newMobile, String billDate, String billNo, String source, String serverName = "HP-PC\\SQLExpress")
        public static int UpdateMobile(String oldMobile, String newMobile, String billDate, String billNo, String source, String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "UPDATE BillDetails SET Mobile = '" + newMobile + "' WHERE Mobile = '" + oldMobile + "' AND BillDate = '" + billDate + "' AND BillNo = '" + billNo + "' AND Source = '" + source + "'";
            //DBHandler.InsertIntoTable("Transactions", "", "'LastUpdate', 'Updated Record of Last Update', '', 'PH=" + lastUpdatedList[1] + " Junior=" + lastUpdatedList[2] + "', CURRENT_TIMESTAMP");
            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            con.Open();

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();

            con.Close();
            return 1;
        }

        public static int AddCustomer(String mobile, String name, int balance = 0)
        {

            return 1;
        }

        //public static int UpdateLastUpdated(List<string> lastUpdatedList, String serverName = "HP-PC\\SQLExpress")
        public static int UpdateLastUpdated(List<string> lastUpdatedList, String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "UPDATE LastUpdate SET LastUpdated = CURRENT_TIMESTAMP, PH = '" + lastUpdatedList[1] + "', Junior = '" + lastUpdatedList[2] + "' WHERE BookCode = '401'";
            //DBHandler.InsertIntoTable("Transactions", "", "'LastUpdate', 'Updated Record of Last Update', '', 'PH=" + lastUpdatedList[1] + " Junior=" + lastUpdatedList[2] + "', CURRENT_TIMESTAMP");
            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            String query2 = "UPDATE LastUpdate SET LastUpdated = CURRENT_TIMESTAMP, PH = '" + lastUpdatedList[4] + "', Junior = '" + lastUpdatedList[5] + "' WHERE BookCode = '403'";
            //DBHandler.InsertIntoTable("Transactions", "", "'LastUpdate', 'Updated Record of Last Update', '', 'PH=" + lastUpdatedList[4] + " Junior=" + lastUpdatedList[5] + "', CURRENT_TIMESTAMP");

            LogWriter log2 = new LogWriter(query2);
            Console.WriteLine(query2);

            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            cmd.ExecuteNonQuery();

            cmd = new SqlCommand(query2, con);
            cmd.ExecuteNonQuery();
            con.Close();
            
            return 1;
        }

        //public static bool CheckIfMemberExists(String mobile, String serverName = "HP-PC\\SQLExpress")
        public static bool CheckIfMemberExists(String mobile, String serverName = "")
        {
            bool flag = false;

            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);
            
            String query = "SELECT COUNT(Mobile) FROM Overview WHERE Mobile = '" + mobile + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            if (int.Parse(dt.Rows[0][0].ToString()) == 1) flag = true;
            
            return flag;
        }

        //public static bool CheckIfBillExists(String bookCode, String billNo, String source, String year, String serverName = "HP-PC\\SQLExpress")
        public static bool CheckIfBillExists(String bookCode, String billNo, String source, String year, String serverName = "")
        {
            bool flag = false;

            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "SELECT COUNT(billNo) FROM BillDetails WHERE BookCode = '" + bookCode + "' AND BillNo = '" + billNo + "' AND Source = '" + source + "' AND Year = '" + year + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            if (int.Parse(dt.Rows[0][0].ToString()) == 1) flag = true;
            
            return flag;
        }

        //public static int UpdateBalance(String mobile, String balance, String serverName = "HP-PC\\SQLExpress")
        public static int UpdateBalance(String mobile, String balance, String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "UPDATE Overview SET Balance = " + balance + ", DateModified = CURRENT_TIMESTAMP WHERE MOBILE = '" + mobile + "'";
            //DBHandler.InsertIntoTable("Transactions", "", "'Overview', 'Updated Balance of " + mobile +"', '', '" + balance + "', CURRENT_TIMESTAMP");

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            con.Open();

            //SqlTransaction transaction = con.BeginTransaction("UpdateBalanceTransaction");

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();

            con.Close();

            return 1;
        }

        //public static int CorrectBalanceForUpdatedNumber(String serverName = "HP-PC\\SQLExpress")
        public static int CorrectBalanceForUpdatedNumber(String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);
            
            String query = "UPDATE Overview SET Balance = 0, DateModified = CURRENT_TIMESTAMP WHERE (Mobile NOT IN (SELECT DISTINCT Mobile FROM BillDetails)) AND Balance <> 0";
            //DBHandler.InsertIntoTable("Transactions", "", "'Overview', 'Updated Balance of " + mobile +"', '', '" + balance + "', CURRENT_TIMESTAMP");

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            con.Open();

            //SqlTransaction transaction = con.BeginTransaction("UpdateBalanceTransaction");

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();

            con.Close();

            return 1;
        }

        public static DataTable GetBagsGiven(String salesID, String dbname, String dbCode, String year, String serverName)
        {
            DataTable dt = new DataTable();

            SqlConnection con = ConnectToDB(serverName, dbname);

            String query = "";
            String items = "";

            //if (serverName.Equals("LENOVO-PC\\SQL2008"))
            if (serverName.Equals("SNEH-CUSTOM"))
            {
                items = "('002450', '002451', '002811', '002837', '002838', '002839')";
            }

            else
            {
                items = "('001282', '001284', '001311', '001283', '001414', '001415')";
            }
            query = "SELECT ItemID,Qty FROM trnSalesItem WHERE ItemID IN " + items + " AND SalesID = '" + salesID + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            adapter.Fill(dt);
            
            return dt;
        }

        //public static int UpdateBagsGiven(String mobile, String dbCode, String billNo, String source, String Year, String BagsGiven, String pointsRedeemed, String serverName = "HP-PC\\SQLExpress")
        public static int UpdateBagsGiven(String mobile, String dbCode, String billNo, String source, String Year, String BagsGiven, String pointsRedeemed, String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);
            
            String query = "UPDATE BillDetails SET BagsGiven = '" + BagsGiven + "', PointsRedeemed = '" + pointsRedeemed + "', DateModified = CURRENT_TIMESTAMP WHERE MOBILE = '" + mobile + "' AND BookCode = '" + dbCode + "' AND BillNo = '" + billNo + "' AND Source = '" + source + "' AND Year = " + Year;

            //DBHandler.InsertIntoTable("Transactions", "", "'BillDetails', 'Updated BagsGiven,pointsRedeemed for " + mobile + "," + dbCode + "," + billNo + "," + source + "," + Year + "', '', '" + BagsGiven + "," + pointsRedeemed + "', CURRENT_TIMESTAMP");
            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            return 1;
        }

        public static DataTable GetLastBills(String serverName, String dbname, String no_of_bills, string finYearId)
        {
            DataTable dt = new DataTable();

            SqlConnection con = ConnectToDB(serverName, dbname);

            String query = "SELECT TOP " + no_of_bills + " SalesID, VoucherNo, DayBookID, VoucherDate, MobileNo, NetAmt, AccountName FROM trnSales WHERE FinYearID = '" + finYearId + "' AND DayBookID = 401 ORDER BY VoucherNo DESC";
            //String query2 = "SELECT TOP " + no_of_bills + " SalesID, VoucherNo, DayBookID, VoucherDate, MobileNo, NetAmt, AccountName FROM trnSales WHERE DayBookID = 403 ORDER BY VoucherNo DESC";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);
            //LogWriter log2 = new LogWriter(query2);
            //Console.WriteLine(query2);

            //DataTable dt2 = new DataTable();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                //SqlDataAdapter adapter2 = new SqlDataAdapter(query2, con);

                adapter.Fill(dt);
                //adapter2.Fill(dt2);
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //dt.Merge(dt2);
            
            return dt;
        }

        public static bool DayInitialized(string date)
        {
            return true;
        }

        //public static DataTable SelectQueryOnTable(String table, String item = "*", String clause = "", String serverName = "HP-PC\\SQLExpress")
        public static DataTable SelectQueryOnTable(String table, String item = "*", String clause = "", String serverName = "")
        {
            DataTable dt = new DataTable();

            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "SELECT " + item + " FROM " + table + " " + clause;

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);

                adapter.Fill(dt);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dt;
        }

        public static int AddDailyTransactions(String machine, String type, String particular, String amount)
        {
            //SqlConnection con = ConnectToDB("HP-PC\\SQLExpress", "CustomerLoyalty");
            SqlConnection con = ConnectToDB(hostServerName, hostDBName);

            String query = "INSERT INTO DailyTransactions VALUES('" + machine + "', '" + type + "', '" + particular + "', " + amount + ")";
            //InsertIntoTable("Transactions", "", "'" + table + "', 'Added New Record', '', '" + values + "', CURRENT_TIMESTAMP");
            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            return 1;
        }

        //public static int InitializeDailyCash(String date, String machine, String initialised, String serverName = "HP-PC\\SQLExpress")
        public static int InitializeDailyCash(String date, String machine, String initialised, String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "UPDATE DailyCash SET Date = '" + date + "', x2000 = 0, x500 = 0, x200 = 0, x100 = 0, x50 = 0, x20 = 0, x10 = 0, x5 = 0 , verified = 0, initialised = " + initialised + " WHERE Machine = '" + machine + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            return 1;
        }

        //public static void ClearDailyTransactions(String machine, String serverName = "HP-PC\\SQLExpress")
        public static void ClearDailyTransactions(String machine, String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "DELETE FROM DailyTransactions WHERE Machine = '" + machine + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();        
        }

        //public static int UpdateDenomination(string denomination, string nos, string machine, string serverName = "HP-PC\\SQLExpress")
        public static int UpdateDenomination(string denomination, string nos, string machine, string serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);
            
            String query = "UPDATE DailyCash SET x" + denomination + " = " + nos + " WHERE Machine = '" + machine + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            return 1;
        }

        //public static int InsertIntoTable(String table, String columns, String values, String serverName = "HP-PC\\SQLExpress")
        public static int InsertIntoTable(String table, String columns, String values, String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            if (columns != "") columns = '(' + columns + ')';

            String query = "INSERT INTO " + table + columns + " VALUES(" + values + ")";
            //InsertIntoTable("Transactions", "", "'" + table + "', 'Added New Record', '', '" + values + "', CURRENT_TIMESTAMP");
            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();
            return 1;
        }

        //public static int GetBalance(String mobile, String serverName = "HP-PC\\SQLExpress")
        public static int GetBalance(String mobile, String serverName = "")
        {
            int bal = 0;

            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "SELECT Balance FROM Overview WHERE Mobile = '" + mobile + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                bal = int.Parse(dt.Rows[0]["Balance"].ToString());
            }
            
            return bal;
        }

        //public static DataTable GetBalanceDiscrepencies(String serverName = "HP-PC\\SQLExpress")
        public static DataTable GetBalanceDiscrepencies(String serverName = "")
        {
            DataTable dt = new DataTable();

            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "SELECT Mobile, ActualBalance, Balance FROM (SELECT a.Mobile, SUM(b.Points) - SUM(b.PointsRedeemed) AS ActualBalance, a.Balance FROM Overview AS a INNER JOIN BillDetails AS b ON a.Mobile = b.Mobile GROUP BY a.Mobile, a.Balance) AS derivedtbl_1 WHERE (ActualBalance <> Balance)";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            adapter.Fill(dt);
            
            return dt;
        }
        
        public static int RemoveModifiedFlag(String CO_CODE, String LC_CODE, String CO_YEAR, String DB_CODE, String TRN_TYPE, String BIL_NO, String SR_NO, String dbname, String serverName)
        {
            SqlConnection con = ConnectToDB(serverName, dbname);
            
            String query = "UPDATE SALE_DETL SET Altr = 'N' WHERE CO_CODE = '" + CO_CODE + "' AND LC_CODE = '" + LC_CODE + "' AND CO_YEAR = '" + CO_YEAR + "' AND DB_CODE = '" + DB_CODE + "' AND TRN_TYPE = '" + TRN_TYPE + "' AND BIL_NO = '" + BIL_NO + "' AND SR_NO = " + SR_NO;
            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlCommand cmd = new SqlCommand(query, con);

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            return 1;
        }

        public static String GetCustomerMobile(String billNo, String dbCode, String coYear, String dbname, String serverName)
        {
            String mobile = "";

            SqlConnection con = ConnectToDB(serverName, dbname);

            String query = "SELECT PHONE FROM SALE_DATA WHERE BIL_NO = '" + billNo + "' AND DB_CODE = '" + dbCode + "' AND CO_YEAR = '" + coYear + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                mobile = dt.Rows[0]["PHONE"].ToString();
            }
            
            return mobile;
        }

        public static String GetCustomerName(String mobile)
        {
            String name = "";

            //SqlConnection con = ConnectToDB("HP-PC\\SQLExpress", "CustomerLoyalty");
            SqlConnection con = ConnectToDB(hostServerName, hostDBName);

            String query = "SELECT Name FROM Overview WHERE Mobile = '" + mobile + "'";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                name = dt.Rows[0]["Name"].ToString();
            }
            
            return name;
        }

        public static DataTable GetCustomerList(string serverName, string dbname)
        {
            SqlConnection con = ConnectToDB(serverName, dbname);

            String query = "SELECT * FROM Overview WHERE LEN(Mobile) = 10";
            
            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);
            
            DataTable dt = new DataTable();
            
            adapter.Fill(dt);
            
            return dt;
        }

        public static string GetSalesID(string serverName, string dbname, string billNo, string billDate, string mobile)
        {
            string salesId = "";

            SqlConnection con = ConnectToDB(serverName, dbname);

            String query = "SELECT SalesID FROM trnSales WHERE VoucherNo = " + billNo + " AND VoucherDate = " + billDate + " AND MobileNo = " + mobile;

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                salesId = dt.Rows[0]["SalesID"].ToString();
            }

            return salesId;
        }
        public static DataTable GetBillParticulars(string serverName, string dbname, string salesID)
        {
            SqlConnection con = ConnectToDB(serverName, dbname);

            String query = "SELECT SalesTypeSR, Barcode, Size1, SalesmanID, MRP, DiscPrc, DiscAmt, NetAmt FROM trnSalesItem WHERE SalesID = " + salesID + " ORDER BY SrNo";

            LogWriter log = new LogWriter(query);
            Console.WriteLine(query);

            SqlDataAdapter adapter = new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            adapter.Fill(dt);

            return dt;
        }

        public static int UpdatePoints(String mobile, String newPoints, String billDate, String billNo, String source, String serverName = "")
        {
            if (serverName == "") serverName = hostServerName;
            SqlConnection con = ConnectToDB(serverName, hostDBName);

            String query = "UPDATE BillDetails SET Points = '" + newPoints + "' WHERE Mobile = '" + mobile + "' AND BillDate = '" + billDate + "' AND BillNo = '" + billNo + "' AND Source = '" + source + "'";
            //DBHandler.InsertIntoTable("Transactions", "", "'LastUpdate', 'Updated Record of Last Update', '', 'PH=" + lastUpdatedList[1] + " Junior=" + lastUpdatedList[2] + "', CURRENT_TIMESTAMP");
            LogWriter log = new LogWriter("UPDATED Points for Mobile: " + mobile + "\tBill No: " + billNo + "\tBill Date: " + billDate + "\tSource: " + source + " to: " + newPoints);
            log = new LogWriter(query);
            Console.WriteLine(query);

            con.Open();

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();

            con.Close();
            return 1;
        }
    }
}
