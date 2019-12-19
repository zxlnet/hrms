using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class pscontctBll : BaseBll
    {
        pscontctDal localDal = null;
        public pscontctBll()
        {
            localDal = new pscontctDal();
            baseDal = localDal;
        }
    }
}
