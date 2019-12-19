using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using System.Collections;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Authorization
{
    public class logonBll : BaseBll
    {
        public string UserValidation(string _userid, string _passwd,string _apnm)
        {
            AuthorizationBll bll = new AuthorizationBll();
            Hashtable result = bll.GetUserValidation(_userid, UtilSecurity.EncryptPassword(_passwd), _apnm);

            return result["Message"].ToString();
        }
    }
}
