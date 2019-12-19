using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;
using LinqKit;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atabsdtlDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|atdt,to|atdt,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tatabsdtls.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        select new
                        {
                            p.emno,
                            p.abda,
                            p.adam,
                            p.ahrm,
                            p.ahrr,
                            p.ahrs,
                            p.atdt,
                            p.eact,
                            p.eahr,
                            p.ectm,
                            p.ehrm,
                            p.lact,
                            p.lahr,
                            p.lctm,
                            p.lhrm,
                            t.sfid,
                            stfn = t.ntnm,
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

        public int DeleteDummyData(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            string sSql = @"delete from tatabsdtl from tatabsdtl a where 
                           exists (select b.atdt,b.emno,b.iscf from 
                           tatanarst b inner join vw_employment emp 
                           on b.emno=emp.emno and " + _sStaffSql + @"
                           where b.emno=a.emno and b.atdt=a.atdt 
                           and (b.iscf is null or b.iscf='N') and 
                           b.atdt >='" + UtilDatetime.FormatDate1(_startDate) + @"' 
                           and b.atdt<'" + UtilDatetime.FormatDate1(_endDate) + "')";

            object[] parameters = { };
            return gDB.ExecuteCommand(sSql, parameters);

        }

        public List<tatabsdtl> GetLeaveDetailForPayroll(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            //return q.ToList();
            string sSql = "select a.* from tatabsdtl a,vw_employment emp where a.emno = emp.emno " +
                           (_sStaffSql.Trim() == string.Empty ? "" : " and " + _sStaffSql) +
                        " and a.atdt >='" + UtilDatetime.FormateDateTime1(_startDate) + "' " +
                        " and a.atdt <'" + UtilDatetime.FormateDateTime1(_endDate) + "' ";

            IEnumerable<tatabsdtl> ret = gDB.ExecuteQuery<tatabsdtl>(sSql);
            return ret.ToList();

        }

   }
}
