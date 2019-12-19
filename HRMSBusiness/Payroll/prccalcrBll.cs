using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prccalcrBll : BaseBll
    {
        prccalcrDal dal = null;

        public prccalcrBll()
        {
            dal = new prccalcrDal();
            baseDal = dal;
        }
    }
}
