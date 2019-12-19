using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Overtime;

namespace GotWell.HRMS.HRMSBusiness.Overtime
{
    public class otsummryBll : BaseBll
    {
        otsummryDal dal = null;

        public otsummryBll()
         {
             dal = new otsummryDal();
             baseDal = dal;
         }
    }
}
