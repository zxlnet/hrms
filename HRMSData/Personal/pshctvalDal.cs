using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.Model.Common;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class pshctvalDal : BaseDal
    {
        public List<tpshctval> GetHeadCountCfgValue(string hccd)
        {
            var q = (from p in gDB.tpshctvals
                     where p.hccd == hccd
                     select p).ToList();

            return q;
        }
    }
}
