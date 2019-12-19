using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class psqurterBll : BaseBll
    {
        psqurterDal localDal = null;
        public psqurterBll()
        {
            localDal = new psqurterDal();
            baseDal = localDal;
        }

    }
}
