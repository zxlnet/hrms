using System;
using System.Collections.Generic;
using System.Text;

namespace GotWell.Model.Alarm
{
    public class MessageMdl
    {
        public MessageMdl()
        { }

        #region Model
        private string _msg_sysid;
        private string _msg_by;
        private string _msg_owner;

        private string _msg_title;
        private string _bodytext;
        private string _comments;
        private string _msg_status;
        private DateTime _created_time;
        private string _created_user;

        /// <summary>
        /// 
        /// </summary>
        public string MSG_SYSID
        {
            set { _msg_sysid = value; }
            get { return _msg_sysid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string MSG_By
        {
            set { _msg_by = value; }
            get { return _msg_by; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string MSG_Owner
        {
            set { _msg_owner = value; }
            get { return _msg_owner; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string MSG_Title
        {
            set { _msg_title = value; }
            get { return _msg_title; }
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
        public string COMMENTS
        {
            set { _comments = value; }
            get { return _comments; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string MSG_STATUS
        {
            set { _msg_status = value; }
            get { return _msg_status; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CREATED_TIME
        {
            set { _created_time = value; }
            get { return _created_time; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Created_User
        {
            set { _created_user = value; }
            get { return _created_user; }
        }
        #endregion Model
    }

}
