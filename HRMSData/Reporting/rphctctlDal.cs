using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Collections;

namespace GotWell.HRMS.HRMSData.Reporting
{
    public class rphctctlDal : BaseDal
    {
        public List<ValueInfo> GetActualValue(tstdefcfg xdef, tstdefcfg ydef, string y)
        {
            string sSql = "select " + xdef.finm + @" as DisplayField,convert(varchar(10),count(*)) as ValueField from vw_employment
                        where " + ydef.finm + "='" + y + @"'
                        and (tmdt is null or tmdt > getdate())
                        group by " + xdef.finm;

            IEnumerable<ValueInfo> lstRet = gDB.ExecuteQuery<ValueInfo>(sSql);

            return lstRet.ToList();
        }

        public List<ValueInfo> GetActualValue(string hccd,tstdefcfg xdef, tstdefcfg ydef, string y)
        {
            var q = (from p in gDB.tpshctvals
                     where p.hccd == hccd
                     && p.yval == y
                     select new ValueInfo
                     {
                         DisplayField = p.xval,
                         ValueField = p.hcnt.Value.ToString()
                     }).ToList();
            return q;
        }

    }
}
