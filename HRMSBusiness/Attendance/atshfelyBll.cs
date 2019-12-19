using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.HRMS.HRMSBusiness.Common;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atshfelyBll : BaseBll
    {
        atshfelyDal dal = null;

        public atshfelyBll()
         {
             dal = new atshfelyDal();
             baseDal = dal;
         }
    }
}
