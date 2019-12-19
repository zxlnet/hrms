using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prvarbleBll : BaseBll
    {
        prvarbleDal dal = null;

        public prvarbleBll()
        {
            dal = new prvarbleDal();
            baseDal = dal;
        }
    }
}
