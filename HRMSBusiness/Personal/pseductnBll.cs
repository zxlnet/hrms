using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class pseductnBll : BaseBll
    {
        pseductnDal localDal = null;
        public pseductnBll()
        {
            localDal = new pseductnDal();
            baseDal = localDal;
        }
    }
}
