using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class prbaalcrDal : BaseDal   
    {
        public override List<T> GetSelectedRecords<T>(List<GotWell.Model.Common.ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprbaalcrs.Where(sSql1)
                        join s in gDB.vw_employments.Where(sSql2) on p.emno equals s.emno
                        join t in gDB.tprsalitms on p.itcd equals t.itcd into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            p.acno,
                            p.efdt,
                            p.emno,
                            p.exdt,
                            p.itcd,
                            p.lmtm,
                            p.lmur,
                            p.sqno,
                            p.valu,
                            p.vtyp,
                            s.sfid,
                            stfn = s.ntnm,
                            c1.itnm
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
