using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Customer_Loyalty_Portal
{
    class WriteToExcel
    {
        public static String email = "thepanthouseonline@gmail.com";
        //public static String toEmail = "sneh.nagrecha@gmail.com";
        public static String pwd = "vvinbgaabmrpsstl";
        public static string workingDirectory = "";
       


        public static void drawBorder(Range c1, Range c2, _Worksheet worksheet)
        {
            worksheet.get_Range(c1, c2).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);
        }

        public static int sendEmail(String toEmail, String source)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(email);
                mail.To.Add(toEmail);
                mail.Subject = "Daily Balance Sheet for " + source + " Dated " + DateTime.Now.ToShortDateString();
                mail.Body = "PFA Daily Balance details for " + source + " Dated " + DateTime.Now.ToString();

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(workingDirectory + source + DateTime.Now.ToString("ddMMyy") + ".xls");
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(email, pwd);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                //MessageBox.Show("Mail Sent!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex.ToString());
                return 0;
            }
            return 1;
        }

        public static void writeToExcel(Home home, String source)
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

            worksheet.Cells.Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            worksheet.Columns.ColumnWidth = 12;
            
            //Merge 4 set of cells.
            worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 9]].Merge(); //Daily Balance for... line
            worksheet.Range[worksheet.Cells[8, 1], worksheet.Cells[8, 2]].Merge(); //CREDIT cell
            worksheet.Range[worksheet.Cells[8, 4], worksheet.Cells[8, 5]].Merge(); //DEBIT cell
            worksheet.Range[worksheet.Cells[8, 7], worksheet.Cells[8, 9]].Merge(); //TOTAL CASH cell

            //Set font to bold for cells
            worksheet.Cells[1, 1].Font.Bold = worksheet.Cells[7, 1].Font.Bold = worksheet.Cells[7, 4].Font.Bold = worksheet.Cells[7, 7].Font.Bold = true;
            //worksheet.Cells[3, 1].Font.Bold = worksheet.Cells[3, 4].Font.Bold = worksheet.Cells[3, 7].Font.Bold = true;
            worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[6, 1]].Font.Bold = true;
            worksheet.Range[worksheet.Cells[3, 4], worksheet.Cells[6, 4]].Font.Bold = true;
            worksheet.Range[worksheet.Cells[3, 7], worksheet.Cells[6, 7]].Font.Bold = true;
            
            worksheet.Cells[1, 1] = "Daily Balance Sheet for " + DateTime.Now.ToShortDateString() + " (" + DateTime.Now.ToString("HH:mm:ss") + ")";
            worksheet.Cells[1, 1].Font.Size = 16;

            worksheet.Cells[3, 1] = "Total Sale";
            worksheet.Cells[3, 2] = home.totalSaleTextBox.Text;

            worksheet.Cells[4, 1] = "Debit Sale";
            worksheet.Cells[4, 2] = home.debitSaleTextBox.Text;

            worksheet.Cells[5, 1] = "Cr. Note Recv";
            worksheet.Cells[5, 2] = home.crntRecvTextBox.Text;

            worksheet.Cells[6, 1] = "Start Bill No";
            worksheet.Cells[6, 2] = home.startBillTextBox.Text;

            worksheet.Cells[3, 4] = "Cash Sale";
            worksheet.Cells[3, 5] = home.cashSaleTextBox.Text;

            worksheet.Cells[4, 4] = "Bal Recv (Cash)";
            worksheet.Cells[4, 5] = home.balReceivedCashTextBox.Text;

            worksheet.Cells[5, 4] = "Cr. Note Issued";
            worksheet.Cells[5, 5] = home.crntIssuedTextBox.Text;

            worksheet.Cells[6, 4] = "End Bill No";
            worksheet.Cells[6, 5] = home.endBillTextBox.Text;

            worksheet.Cells[3, 7] = "Card Sale";
            worksheet.Cells[3, 8] = home.cardSaleTextBox.Text;

            worksheet.Cells[4, 7] = "Bal Recv (Card)";
            worksheet.Cells[4, 8] = home.balReceivedCardTextBox.Text;

            worksheet.Cells[5, 7] = "PayTm";
            worksheet.Cells[5, 8] = home.paytmTextBox.Text;

            worksheet.Cells[6, 7] = "Bajaj";
            worksheet.Cells[6, 8] = home.bajajTextBox.Text;

            worksheet.Cells[8, 1] = "CREDIT";
            worksheet.Cells[8, 4] = "DEBIT";
            worksheet.Cells[8, 7] = "TOTAL CASH";

            int x = 9;
            int y = 1;
            int max = 0;

            for (int j = 0; j < home.creditGridView.Columns.Count; j++)
            {
                if (home.creditGridView.Rows.Count > max) max = home.creditGridView.Rows.Count;

                worksheet.Cells[x, j + y] = home.creditGridView.Columns[j].HeaderText;
                worksheet.Cells[x, j + y].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);

                for (int i = 0; i < home.creditGridView.Rows.Count; i++)
                {
                    worksheet.Cells[x + i + 1, j + y] = home.creditGridView.Rows[i].Cells[j].Value.ToString();
                }
            }

            worksheet.Cells[x + home.creditGridView.RowCount + 1, y] = "Total Credit";
            worksheet.Cells[x + home.creditGridView.RowCount + 1, y].Font.Bold = true;
            worksheet.Cells[x + home.creditGridView.RowCount + 1, y + 1] = home.totalCreditLabel.Text;//Replace with totalCreditLabel text

            y += home.creditGridView.Columns.Count + 1;

            for (int j = 0; j < home.debitGridView.Columns.Count; j++)
            {
                if (home.debitGridView.Rows.Count > max) max = home.debitGridView.Rows.Count;
                worksheet.Cells[x, j + y] = home.debitGridView.Columns[j].HeaderText;
                worksheet.Cells[x, j + y].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);

                for (int i = 0; i < home.debitGridView.Rows.Count; i++)
                {
                    worksheet.Cells[x + i + 1, j + y] = home.debitGridView.Rows[i].Cells[j].Value.ToString();
                }
            }

            worksheet.Cells[x + home.debitGridView.RowCount + 1, y] = "Total Debit";
            worksheet.Cells[x + home.debitGridView.RowCount + 1, y].Font.Bold = true;
            worksheet.Cells[x + home.debitGridView.RowCount + 1, y + 1] = home.totalDebitLabel.Text;//Replace with totalDebitLabel text

            y += home.debitGridView.Columns.Count + 1;

            for (int j = 0; j < home.cashGridView.Columns.Count; j++)
            {
                if (home.cashGridView.Rows.Count > max) max = home.cashGridView.Rows.Count;
                worksheet.Cells[x, j + y] = home.cashGridView.Columns[j].HeaderText;
                worksheet.Cells[x, j + y].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);

                for (int i = 0; i < home.cashGridView.Rows.Count; i++)
                {
                    worksheet.Cells[x + i + 1, j + y] = home.cashGridView.Rows[i].Cells[j].Value.ToString();
                }
            }

            worksheet.Range[worksheet.Cells[x + home.cashGridView.RowCount + 1, y], worksheet.Cells[x + home.cashGridView.RowCount + 1, y + 1]].Merge();
            worksheet.Cells[x + home.cashGridView.RowCount + 1, y] = "Total Cash";
            worksheet.Cells[x + home.cashGridView.RowCount + 1, y].Font.Bold = true;
            worksheet.Cells[x + home.cashGridView.RowCount + 1, y + 2] = home.totalCashLabel.Text;//Replace with totalCashLabel text

            worksheet.Range[worksheet.Cells[x + home.cashGridView.RowCount + 2, y], worksheet.Cells[x + home.cashGridView.RowCount + 2, y + 1]].Merge();
            worksheet.Cells[x + home.cashGridView.RowCount + 2, y] = "Calculated Cash";
            worksheet.Cells[x + home.cashGridView.RowCount + 2, y].Font.Bold = true;
            worksheet.Cells[x + home.cashGridView.RowCount + 2, y + 2] = home.calculatedBalanceLabel.Text;//Replace with totalCashLabel text

            worksheet.Range[worksheet.Cells[x + home.cashGridView.RowCount + 2, 1], worksheet.Cells[x + home.cashGridView.RowCount + 2, 6]].Merge();
            worksheet.Cells[x + home.cashGridView.RowCount + 2, 1] = "*Calculated cash = Cash Sale + total credit - total debit";
            
        
        
        
        /*x = x + max + 2;

            Range c1 = worksheet.Cells[x, 1];
            Range c2 = worksheet.Cells[x + 3, 1];
            worksheet.get_Range(c1, c2).Font.Bold = true;
            
            worksheet.Cells[x, 1] = "Total Credit"; 
            worksheet.Cells[x++, 2] = ""; //Replace with text from TotalCreditLabel

            worksheet.Cells[x, 1] = "Total Debit";
            worksheet.Cells[x++, 2] = ""; //Replace with text from TotalDebitLabel

            worksheet.Cells[x, 1] = "Total Cash Present";
            worksheet.Cells[x++, 2] = ""; //Replace with text from TotalCashLabel

            worksheet.Cells[x, 1] = "Calculated Cash";
            worksheet.Cells[x++, 2] = ""; //Replace with text from BalanceMatch
            */

            //Draw borders around Tables
            drawBorder(worksheet.Cells[x, 1], worksheet.Cells[x + home.creditGridView.RowCount, 2], worksheet);
            drawBorder(worksheet.Cells[x, 4], worksheet.Cells[x + home.debitGridView.RowCount, 5], worksheet);
            drawBorder(worksheet.Cells[x, 7], worksheet.Cells[x + home.cashGridView.RowCount, 9], worksheet);

            worksheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
            worksheet.PageSetup.PrintGridlines = true;
            worksheet.PrintOutEx();

            app.DisplayAlerts = false;
            //app.Visible = true;
            //app.ActiveWindow.PrintPreview();
            // save the application  
            if(!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }
            app.DisplayAlerts = false;
            workbook.SaveAs(workingDirectory + source + DateTime.Now.ToString("ddMMyy") + ".xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            // Exit from the application  
            workbook.Close();
            app.Quit();
            //sendEmail();

        }
    }
}
