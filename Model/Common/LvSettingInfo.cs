using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.HRMS;

namespace GotWell.Model.Common
{
    public class LvSettingInfo
    {
        public double dfbyEmployment { get; set; }
        public double dfbyYear { get; set; }
        public double dfbyOthers { get; set; }
        public double WeekLimit { get; set; }
        public double MonthLimit { get; set; }
        public double YearLimit { get; set; }
        public double WeekConsume { get; set; }
        public double MonthConsume { get; set; }
        public double YearConsume { get; set; }
        public double WeekBalance { get; set; }
        public double MonthBalance { get; set; }
        public double YearBalance { get; set; }
        public double DaysCarry { get; set; }
        public double MinBalance { get; set; }

        public vw_employment emp { get; set; }
        public string ltcd { get; set; }

        public List<string> SummaryText { get; set; }

        public LvSettingInfo()
        {
            SummaryText = new List<string>();
            WeekLimit = -1;
            MonthLimit = -1;
            YearLimit = -1;
            WeekBalance = -1;
            MonthBalance = -1;
            YearBalance = -1;
        }
    }
}
