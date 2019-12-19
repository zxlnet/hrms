using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atpvtcldDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|cddt,to|cddt,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tatpriclds.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tathldtyps on p.htcd equals s.htcd into t1
                        from cp in t1.DefaultIfEmpty()
                        join o in gDB.tottypes on p.otcd equals o.otcd into t2
                        from cq in t2.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            p.cddt,
                            p.otcd,
                            p.htcd,
                            cp.htnm,
                            cq.otnm,
                            p.lmtm,
                            p.lmur,
                            p.rfid
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

        public List<tatpricld> GetPrivateCalendar(List<ColumnInfo> _parameter, string _sSqlStaff,DateTime _startDate,DateTime _endDate)
        {
            try
            {
                string sSql = @"select a.* from tatpricld a,vw_employment emp
                                where a.emno = emp.emno
                                and   " + _sSqlStaff + @"
                                and a.cddt>='" + UtilDatetime.FormatDate1(_startDate) + @"' 
                                and a.cddt<'" + UtilDatetime.FormatDate1(_endDate) + @"'";

                List<tatpricld> obj = gDB.ExecuteQuery<tatpricld>(sSql).ToList();

                return obj;

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
