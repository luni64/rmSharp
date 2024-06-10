//using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rmSharp
{
    public enum DateType { NULL = '.', Standard = 'D', Quaker = 'Q', Text = 'T' };
    public enum RangeType { Dash = '-', After = 'A', Before = 'B', From = 'F', Since = 'I', Or = 'O', BetAnd = 'R', FromTo = 'S', To = 'T', Until = 'U', By = 'Y', NoRange = '.' };
    public enum Era { BC = '-', AD = '+', BCE = BC, CE = AD };
    public enum Certainty
    {
        Maybe = '?', Perhaps = '1', Apparant = '2', Likely = '3', Possibly = '4', Probably = '5', Certain = '6',
        About = 'A', Circa = 'C', Estimated = 'E', Calculated = 'L', Say = 'S', Other = '.'
    };


    public class RMDate
    {
        private string[] MonthTags = new string[] { "ERR", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
        private Dictionary<RangeType, (string, string)> rangeTxt = new()
        {
            { RangeType.FromTo,("From","to")},
            { RangeType.BetAnd,("Between","and")},
            { RangeType.Dash,("","-")},
        };

        public DateType dateType { get; set; } = DateType.Standard;  // .=NULL, D=Standard, Q=Quaker, T=Text
        public RangeType rangeType { get; set; } = RangeType.NoRange;
        public Era eraFirst { get; set; } = Era.AD;
        public Era eraSecond { get; set; } = Era.AD;
        public Certainty certFirst { get; set; } = Certainty.Other;
        public Certainty certSecond { get; set; } = Certainty.Other;

        public int year { get; set; }
        public int year2 { get; set; }
        public int month { get; set; }
        public int month2 { get; set; }
        public int day { get; set; }
        public int day2 { get; set; }
        public string text { get; set; } = string.Empty;


        public string toRmDatestring()
        {
            StringBuilder sb = new();

            sb.Append((char)dateType);
            if (dateType != DateType.NULL)
            {
                sb.Append((char)rangeType);
                sb.Append((char)eraFirst);
                sb.Append(year.ToString("D4"));
                sb.Append(month.ToString("D2"));
                sb.Append(day.ToString("D2"));
                sb.Append(".");
                sb.Append((char)certFirst);
                sb.Append((char)eraSecond);
                sb.Append(year2.ToString("D4"));
                sb.Append(month2.ToString("D2"));
                sb.Append(day2.ToString("D2"));
                sb.Append(".");
                sb.Append((char)certSecond);
            }

            return sb.ToString(); ;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            if (dateType == DateType.Standard)
            {
                if (rangeType == RangeType.NoRange)
                {
                    yyyymmmddd(sb);
                }
                else if (rangeType != RangeType.Dash && rangeType != RangeType.BetAnd && rangeType != RangeType.FromTo)
                {
                    sb.Append(rangeType + " ");
                    yyyymmmddd(sb);
                }
                else
                {
                    var rt = rangeTxt[rangeType];
                    sb.Append(rt.Item1 + " ");
                    yyyymmmddd(sb, true);
                    sb.Append(" " + rt.Item2 + " ");
                    yyyymmmddd(sb, first: false);
                    //    sb.Append(month != 0 ? MonthTags[month] + " " : "");
                    //    sb.Append(year != 0 ? year.ToString() : "");
                }
            }
            return sb.ToString();

        }

        public void yyyymmmddd(StringBuilder sb, bool first = true)
        {
            if (first)
            {
                sb.Append(day != 0 ? day.ToString() + " " : "");
                sb.Append(month != 0 ? MonthTags[month] + " " : "");
                sb.Append(year != 0 ? year.ToString() : "");
            }
            else
            {
                sb.Append(day2 != 0 ? day2.ToString() + " " : "");
                sb.Append(month2 != 0 ? MonthTags[month2] + " " : "");
                sb.Append(year2 != 0 ? year2.ToString() : "");
            }
            //sb.Append(" ");
        }

        public RMDate()
        {
            dateType = DateType.NULL;
        }

        public RMDate(DateTime? dt, Certainty c = Certainty.Certain)
        {
            if (dt.HasValue)
            {
                dateType = DateType.Standard;
                year = dt.Value.Year;
                month = dt.Value.Month;
                day = dt.Value.Day;
            }
            else dateType = DateType.NULL;
        }


        public RMDate(string s)
        {
            if (s.Length == 24)
            {
                dateType = (DateType)s[0];
                rangeType = (RangeType)s[1];
                eraFirst = (Era)s[2];
                year = int.Parse(s.Substring(3, 4));
                month = int.Parse(s.Substring(7, 2));
                day = int.Parse(s.Substring(9, 2));
                //11
                certFirst = (Certainty)s[12];
                eraSecond = (Era)s[13];
                year2 = int.Parse(s.Substring(14, 4));
                month2 = int.Parse(s.Substring(18, 2));
                day2 = int.Parse(s.Substring(20, 2));
                //22
                certSecond = (Certainty)s[23];

            }
            else dateType = DateType.NULL;
        }

        public RMDate(DateTime from, DateTime to, RangeType rt = RangeType.FromTo)
        {
            dateType = DateType.Standard;
            rangeType = rt;
            year = from.Year;
            month = from.Month;
            day = from.Day;

            year2 = to.Year;
            month2 = to.Month;
            day2 = to.Day;
        }
    }
}
