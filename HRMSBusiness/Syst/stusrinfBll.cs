using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSBusiness.Authorization;
using System.Collections;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class stusrinfBll : BaseBll
    {
        public string GetUserInformation(string _userid,  string _apnm)
        {
            AuthorizationBll bll = new AuthorizationBll();
            Hashtable result = bll.GetUserInformation(_userid,_apnm);

            return result["Message"].ToString();
        }

        public string ChangeUserPassword(string _userid, string _opwd, string _npwd, string _apnm)
        {
            AuthorizationBll bll = new AuthorizationBll();
            Hashtable result = bll.ChangeUserPassword(_userid, UtilSecurity.EncryptPassword(_opwd), UtilSecurity.EncryptPassword(_npwd), _apnm);

            return result["Message"].ToString();
        }

    }
}
