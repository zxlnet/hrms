using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Syst;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class stsyscfgBll : BaseBll
    {
        stsyscfgDal localDal;

        public stsyscfgBll()
        {
            localDal = new stsyscfgDal();

        }

        public StSystemConfig GetSystemSetting()
        {
            return localDal.GetSystemSetting();
        }
    }
}
