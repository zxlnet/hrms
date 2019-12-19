using System;
using System.Collections.Generic;
using System.Text;
using GotWell.Common;


namespace GotWell.Model.Alarm
{
    public class AlarmMdl
    {
        public AlarmMdl()
        {
            Alarm_SysID = String.Empty;
            Alarm_Type = String.Empty;
            Subject = String.Empty;
            Recipients = String.Empty;
            CC = string.Empty;
            BodyText = String.Empty;
            Comments = String.Empty;
            Alarm_Status = Alarm_AlarmStatus.Unhandled.ToString();
            Application = string.Empty;
            Creator = string.Empty;
            Created_Time = Function.GetNullDateTime();
        }

        #region Model
        public string Alarm_SysID { get; set; }
        public string Alarm_Type { get; set; }
        public string Subject { get; set; }
        public string Recipients { get; set; }
        public string CC { get; set; }
        public string BodyText { get; set; }
        public string Comments { get; set; }
        public string Alarm_Status { get; set; }
        public string Application { get; set; }
        public string Creator { get; set; }
        public DateTime Created_Time { get; set; }
        #endregion Model
    }
}




