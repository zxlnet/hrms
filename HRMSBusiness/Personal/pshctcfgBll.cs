using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Personal;
using GotWell.HRMS.HRMSBusiness.Common;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class pshctcfgBll : BaseBll
    {
        pshctcfgDal dal = null;

        public pshctcfgBll()
        {
            dal = new pshctcfgDal();
            baseDal = dal;
        }
    }
}
