using System;
using System.Collections.Generic;
using System.Text;
using GotWell.Common;

namespace GotWell.Model.Alarm
{
    /// <summary>
    /// 实体类PBALMMAILDELIVERY 。
    /// </summary>
    public class AlarmEMailDeliveryMdl
    {
        #region Construct
        public AlarmEMailDeliveryMdl()
        {
            _amd_sysid = String.Empty;
            _subject = String.Empty;
            _recipient = String.Empty;
            _bodytext = String.Empty;
            _alarm_status = String.Empty;
            _created_time = Function.GetNullDateTime();
            _sent_time = Function.GetNullDateTime();
            Group_ID = string.Empty;
            //Alarm_SysID = string.Empty;
        }
        #endregion

        #region Model
        private string _amd_sysid;
        private string _subject;
        private string _recipient;
        private string _bodytext;
        private string _alarm_status;
        private DateTime _created_time;
        private DateTime _sent_time;

        /// <summary>
        /// 
        /// </summary>
        public string AMD_SYSID
        {
            set { _amd_sysid = value; }
            get { return _amd_sysid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Subject
        {
            set { _subject = value; }
            get { return _subject; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Recipient
        {
            set { _recipient = value; }
            get { return _recipient; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BodyText
        {
            set { _bodytext = value; }
            get { return _bodytext; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Alarm_Status
        {
            set { _alarm_status = value; }
            get { return _alarm_status; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Created_Time
        {
            set { _created_time = value; }
            get { return _created_time; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Sent_Time
        {
            set { _sent_time = value; }
            get { return _sent_time; }
        }

        public string Group_ID { get; set; }
        //public string Alarm_SysID { get; set; }

        #endregion Model
    }
}