using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class psexprncBll : BaseBll
    {
        psexprncDal localDal = null;
        public psexprncBll()
        {
            localDal = new psexprncDal();
            baseDal = localDal;
        }

    }
}
