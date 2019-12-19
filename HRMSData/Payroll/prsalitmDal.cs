using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class prsalitmDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("itcd,itnm,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprsalitms.Where(sSql1)
                        join t in gDB.tprsaltyps.Where(sSql2) on p.stcd equals t.stcd into t1
                        from c1 in t1.DefaultIfEmpty() 
                        join s in gDB.tprsaitfgs on p.sifg equals s.sifg into t2
                        from c2 in t2.DefaultIfEmpty()
                        select new
                        {
                           p.csal,
                           p.irim,
                           p.iscu,
                           p.itcd,
                           p.itnm,
                           p.lmtm,
                           p.lmur,
                           p.opty,
                           p.sifg,
                           p.stcd,
                           p.vapc,
                           c1.stnm,
                           c2.dscr,
                           p.remk,
                           p.rfid,
                           p.bkal
                        };

                List<T> obj = q.Cast<T>().ToList();

                totalRecordCount = obj.Count;

                List<T> appList = null;

                if (paging)
                    appList = obj.Skip(start).Take(num).ToList();
                else
                    appList = obj;

                return appList;

            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }
    }
}
