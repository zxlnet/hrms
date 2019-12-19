using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class psrelshpBll:BaseBll
    {
       psrelshpDal localDal = null;
       public psrelshpBll()
        {
            localDal = new psrelshpDal();
            baseDal = localDal;
        }
    }
}
