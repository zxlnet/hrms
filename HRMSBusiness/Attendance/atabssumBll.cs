using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atabssumBll : BaseBll
    {
         atabssumDal dal = null;

         public atabssumBll()
         {
             dal = new atabssumDal();
             baseDal = dal;
         }

    }
}
