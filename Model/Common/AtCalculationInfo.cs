using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.HRMS;

namespace GotWell.Model.Common
{
    public class AtCalculationInfo
    {
        public DateTime CurDay { get; set; }
        public DateTime CurDayReal { get; set; }
        public System.Nullable<DateTime> StdEarylyTimeIn { get; set; }
        public System.Nullable<DateTime> StdTimeIn { get; set; }
        public System.Nullable<DateTime> StdBreakStart { get; set; }
        public System.Nullable<DateTime> StdBreakEnd { get; set; }
        public System.Nullable<DateTime> StdTimeOut { get; set; }
        public System.Nullable<DateTime> StdLateTimeOut { get; set; }
        public System.Nullable<DateTime> StdRest1Start { get; set; }
        public System.Nullable<DateTime> StdRest2Start { get; set; }
        public System.Nullable<DateTime> StdRest1End { get; set; }
        public System.Nullable<DateTime> StdRest2End { get; set; }

        public double StdRest1Time { get; set; }
        public double StdRest2Time { get; set; }

        public System.Nullable<DateTime> ActTimeIn { get; set; }
        public System.Nullable<DateTime> ActBreakStart { get; set; }
        public System.Nullable<DateTime> ActBreakEnd { get; set; }
        public System.Nullable<DateTime> ActTimeOut { get; set; }
        public System.Nullable<DateTime> ActRest1Start { get; set; }
        public System.Nullable<DateTime> ActRest2Start { get; set; }
        public System.Nullable<DateTime> ActRest1End { get; set; }
        public System.Nullable<DateTime> ActRest2End { get; set; }

        public System.Nullable<DateTime> ActTimeInCalc { get; set; }
        public System.Nullable<DateTime> ActTimeOutCalc { get; set; }
        public System.Nullable<DateTime> ActBreakStart1 { get; set; }
        public System.Nullable<DateTime> ActBreakEnd1 { get; set; }

        public TimeSpan ActEarlyTimeOut { get; set; }
        public TimeSpan ActLateTimeIn { get; set; }
        public TimeSpan ActEarlyTimeOutReal { get; set; }
        public TimeSpan ActLateTimeInReal { get; set; }
        public TimeSpan ActLateBreakStart { get; set; }
        public TimeSpan ActEarlyBreakEnd { get; set; }
        public TimeSpan ActLateTimeInReal2 { get; set; }
        public TimeSpan ActLateBreakStartReal { get; set; }
        public TimeSpan ActEarlyBreakEndReal { get; set; }
        public double ActEarlyOutHours { get; set; }
        public double ActEarlyBreakHours { get; set; }
        public double ActLateTimeHours { get; set; }
        public double ActLateBreakHours { get; set; }

        public System.Nullable<DateTime> LvStart { get; set; }
        public System.Nullable<DateTime> LvEnd { get; set; }

        public bool IsRestDay { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarly { get; set; }
        public bool IsCalendarRest { get; set; }
        public bool IsRosterRest { get; set; }
        public bool IsPrivateRest { get; set; }

        public double OTHours { get; set; }
        public double LvHours { get; set; }
        public double LvDays { get; set; }
        public double AbsentHours { get; set; }
        public double AbsentHoursReal { get; set; }
        public double LateBreakStartHours { get; set; }

        public double EarlyHoursOut { get; set; }
        public double EarlyHoursBreak { get; set; }
        public double EarlyHoursTotal { get; set; }
        public double LateHoursIn { get; set; }
        public double LateHoursBreak { get; set; }
        public double LateHoursTotal { get; set; }


        public int LateCountIn { get; set; }
        public int LateCountBreak { get; set; }
        public int LateCountTotal { get; set; }
        public int EarlyCountOut { get; set; }
        public int EarlyCountBreak { get; set; }
        public int EarlyCountTotal { get; set; }

        public vw_employment EmpInfo;

        public string PrivateOTType { get; set; }
        public string CalendarOTType { get; set; }
        public string RosterOTType { get; set; }

        public string remark;

        public AtCalculationInfo()
        {
            ActEarlyTimeOut = new TimeSpan(0, 0, 0);
            ActLateTimeIn = new TimeSpan(0, 0, 0);
            ActEarlyTimeOutReal = new TimeSpan(0, 0, 0);
            ActLateTimeInReal = new TimeSpan(0, 0, 0);
            ActEarlyBreakEnd = new TimeSpan(0, 0, 0);
            ActLateTimeInReal2 = new TimeSpan(0, 0, 0);
            ActLateBreakStartReal = new TimeSpan(0, 0, 0);
            ActEarlyBreakEndReal = new TimeSpan(0, 0, 0);

            PrivateOTType = string.Empty;
            CalendarOTType = string.Empty;
            RosterOTType = string.Empty;
        }
    }
}
