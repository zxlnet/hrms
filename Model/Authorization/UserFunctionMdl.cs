using System;
using System.Collections.Generic;
using System.Text;

namespace GotWell.Model.Authorization
{
    public class UserFunctionMdl
    {
        public UserFunctionMdl()
		{
            _user_sysid = String.Empty;
            _func_id = String.Empty;
            _permis = String.Empty;
        }
        #region model
        private string _user_sysid;
        private string _func_id;
        private string _permis;
        /// <summary>
        /// 
        /// </summary>
        public string user_sysid
        {
            set { _user_sysid = value; }
            get { return _user_sysid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string func_id
        {
            set { _func_id = value; }
            get { return _func_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string permis
        {
            set { _permis = value; }
            get { return _permis; }
        }
        #endregion model
    }
}
