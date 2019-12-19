using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Overtime
{
    public class otsummryDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("period,otcd,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.totsummries.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join o in gDB.tottypes on p.otcd equals o.otcd
                        select new
                        {
                            p.emno,
                            t.sfid,
                            sfnm = t.ntnm,
                            p.lmtm,
                            p.lmur,

                            p.othr,
                            p.perd,
                            p.otcd,
                            p.tottype.otnm
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
