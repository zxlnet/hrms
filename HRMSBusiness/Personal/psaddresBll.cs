using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class psaddresBll : BaseBll
    {
        psaddresDal localDal = null;
        public psaddresBll()
        {
            localDal = new psaddresDal();
            baseDal = localDal;
        }
    }
}
