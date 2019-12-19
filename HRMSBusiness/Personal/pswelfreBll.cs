using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class pswelfreBll : BaseBll
    {
        pswelfreDal localDal = null;
        public pswelfreBll()
        {
            localDal = new pswelfreDal();
            baseDal = localDal;
        }

    }
}
