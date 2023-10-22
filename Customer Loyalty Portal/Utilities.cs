using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer_Loyalty_Portal
{
    class Utilities
    {
        public static string GetFinYearFromDate(DateTime date)
        {
            string finYear = "";

            int year = date.Year;

            DateTime refDate = new DateTime(year, 04, 01);
            int startYear = 0;
            int endYear = 0;

            if(date.CompareTo(refDate) >=0 )
            {
                startYear = year;
                endYear = year + 1;
            }

            else
            {
                startYear = year - 1;
                endYear = year;
            }

            if(startYear + endYear >0)
            {
                finYear = (startYear-2000).ToString() + (endYear-2000).ToString();
            }

            return finYear;
        }
        public static string GetFinYearIDFromDate(DateTime date, string source)
        {
            string finYearID = "";

            int year = date.Year;

            DateTime refDate = new DateTime(year, 04, 01);
            int refYear = 0;

            if (source.Equals("PH")) refYear = 2015;
            else if (source.Equals("Junior")) refYear = 2017;

            
            if (date.CompareTo(refDate) >= 0)
            {
              finYearID = (year - refYear).ToString();
            }

            else
            {
                finYearID = (year - refYear - 1).ToString();
            }

            return finYearID;
        }
    }
}
