using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atshflatBll : BaseBll
    {
        atshflatDal dal = null;

        public atshflatBll()
         {
             dal = new atshflatDal();
             baseDal = dal;
         }

    }
}
