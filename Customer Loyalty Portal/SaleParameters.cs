using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer_Loyalty_Portal
{
    public class SaleParameters
    {
        int totSale;
        int cashSale;
        int cardSale;
        int debitSale;
        int balReceived_cash;
        int balReceived_card;
        int paytm;
        int bajaj;
        int crntIssued;
        int crntRecv;
        string startBillNo;
        string endBillNo;

        public SaleParameters(int totSale, int cashSale, int cardSale, int debitSale, int balReceived_cash, int balReceived_card, int paytm, int bajaj, int crntIssued, int crntRecv, string startBillNo, string endBillNo)
        {
            this.totSale = totSale;
            this.cardSale = cardSale;
            this.cashSale = cashSale;
            this.debitSale = debitSale;
            this.balReceived_cash = balReceived_cash;
            this.balReceived_card = balReceived_card;
            this.paytm = paytm;
            this.bajaj = bajaj;
            this.crntIssued = crntIssued;
            this.crntRecv = crntRecv;
            this.startBillNo = startBillNo;
            this.endBillNo = endBillNo; 
        }

        void setParameters(int totSale, int cashSale, int cardSale, int debitSale, int balReceived_cash, int balReceived_card, int paytm, int bajaj, int crntIssued, int crntRecv)
        {
            this.totSale = totSale;
            this.cardSale = cardSale;
            this.cashSale = cashSale;
            this.debitSale = debitSale;
            this.balReceived_cash = balReceived_cash;
            this.balReceived_card = balReceived_card;
            this.paytm = paytm;
            this.bajaj = bajaj;
            this.crntIssued = crntIssued;
            this.crntRecv = crntRecv;
        }

        public int getTotSale()
        {
            return totSale;
        }

        public int getCardSale()
        {
            return cardSale;
        }

        public int getCashSale()
        {
            return cashSale;
        }

        public int getDebitSale()
        {
            return debitSale;
        }

        public int getBalReceivedCash()
        {
            return balReceived_cash;
        }

        public int getBalReceivedCard()
        {
            return balReceived_card;
        }

        public int getPaytm()
        {
            return paytm;
        }

        public int getBajaj()
        {
            return bajaj;
        }

        public int getCrntIssued()
        {
            return crntIssued;
        }

        public int getCrntRecv()
        {
            return crntRecv;
        }

        public string getStartBill()
        {
            return startBillNo;
        }

        public string getEndBill()
        {
            return endBillNo;
        }
    }
}
