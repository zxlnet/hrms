using System;
using System.Collections.Generic;
using System.Text;
using GotWell.Common;

namespace GotWell.Model.Alarm
{
    /// <summary>
    /// 实体类PBALARMBOARDDELIVERY 。(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public class AlarmBoardDeliveryMdl
    {
        #region Construct
        public AlarmBoardDeliveryMdl()
        {
            _albo_sysid = String.Empty;
            _subject = String.Empty;
            _recipient = String.Empty;
            _bodytext = String.Empty;
            _alarm_status = String.Empty;
            _created_time = Function.GetNullDateTime();

        }
        #endregion

        #region Model
        private string _albo_sysid;
        private string _subject;
        private string _recipient;
        private string _bodytext;
        private string _alarm_status;
        private DateTime _created_time;
        /// <summary>
        /// 
        /// </summary>
        public string ALBO_SYSID
        {
            set { _albo_sysid = value; }
            get { return _albo_sysid; }
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
        #endregion Model
    }
}
