using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Recruitment;

namespace GotWell.HRMS.HRMSBusiness.Recruitment
{
    public class rcintrdtBll : BaseBll
    {
        rcintrdtDal localDal = null;
        public rcintrdtBll()
        {
            localDal = new rcintrdtDal();
            baseDal = localDal;
        }    
    }
}
