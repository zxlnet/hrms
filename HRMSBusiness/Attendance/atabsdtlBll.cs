using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atabsdtlBll : BaseBll
    {
         atabsdtlDal dal = null;

         public atabsdtlBll()
         {
             dal = new atabsdtlDal();
             baseDal = dal;
         }

    }
}
