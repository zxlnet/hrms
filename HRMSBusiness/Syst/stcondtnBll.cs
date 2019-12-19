using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Syst;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class stcondtnBll : BaseBll
    {
        stcondtnDal dal = null;

        public stcondtnBll()
        {
            dal = new stcondtnDal();
            baseDal = dal;
        }

    }
}
