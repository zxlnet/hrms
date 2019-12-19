using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class prcstalcDal : BaseDal
    {
        public List<object> GetDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("rnno,perd", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprcstalcs.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        select new
                        {
                            p.cccd,
                            p.tbscosctr.ccnm,
                            p.amnt,
                            p.emno,
                            p.itcd,
                            p.perd,
                            p.pkty,
                            p.rnno,
                            p.sqno,
                            p.tprsalitm.itnm,
                            t.sfid,
                            sfnm = t.ntnm,
                            p.crcd,
                            p.tbscurncy.crnm
                        };

                List<object> obj = q.Cast<object>().ToList();

                totalRecordCount = obj.Count;

                List<object> appList = null;

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

        public List<object> GetSummary(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                var q = from p in gDB.tprcstalcs
                        join t in gDB.vw_employments on p.emno equals t.emno
                        where p.emno.Contains(GetColumnValue("emno", _parameter))
                        group p.amnt by new { p.cccd,p.tbscosctr.ccnm, p.emno, p.perd, p.rnno, t.ntnm, t.sfid, p.crcd, p.tbscurncy.crnm } into t1
                        select new
                        {
                            t1.Key.cccd,
                            t1.Key.ccnm,
                            t1.Key.emno,
                            sfnm = t1.Key.ntnm,
                            t1.Key.perd,
                            t1.Key.rnno,
                            t1.Key.sfid,
                            amnt = t1.Sum(),
                            t1.Key.crcd,
                            t1.Key.crnm
                        };

                List<object> obj = q.Cast<object>().ToList();

                totalRecordCount = obj.Count;

                List<object> appList = null;

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
