using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atroshisDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("rscd,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tatroshis.Where(sSql1)
                        join s in gDB.vw_employments.Where(sSql2) on p.emno equals s.emno
                        join t in gDB.tatrosters on p.rscd equals t.rscd
                        orderby p.sqno
                        select new
                        {
                            p.efdt,
                            p.emno,
                            p.exdt,
                            p.iaod,
                            p.lmtm,
                            p.lmur,
                            p.rscd,
                            p.sqno,
                            s.sfid,
                            stfn = s.ntnm,
                            t.rsnm,
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

        public tatroshi GetSelectedObject(List<ColumnInfo> _parameter)
        {
            try
            {

                var q = from p in gDB.tatroshis
                        join s in gDB.vw_employments on p.emno equals s.emno
                        where s.emno.Equals(GetColumnValue("emno", _parameter))
                        && p.efdt.Value == Convert.ToDateTime(GetColumnValue("efdt", _parameter))
                        select p;

                List<tatroshi> obj = q.ToList();

                if (obj.Count>0)
                    return obj.Single();

                return null;

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

        public bool CheckRosterHistory(tatroshi obj)
        {
            bool r=false;
            var q = from p in gDB.tatroshis
                    where p.emno == obj.emno
                    && ((p.exdt.HasValue == false) || (p.exdt >= obj.efdt))
                    && ((obj.exdt.HasValue == false) || (p.exdt > obj.efdt))
                    && (p.efdt != obj.efdt)
                    select p;

            if (q.ToList().Count > 0)
                r = false;
            else
                r = true;

            return r;
        }

        public List<tatroshi> GetRosterHistory(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            string sSql = @"select a.* 
                              from tatroshis a inner join tatroster c 
                                   on a.rscd=c.rscd 
                              inner join vw_employment emp 
                              on a.emno = emp.emno 
                            where (isnull(a.exdt,'3000-01-01') > '" + UtilDatetime.FormatDate1(_startDate) +
                            @"' and isnull(a.efdt,'3000-01-01') <='" + UtilDatetime.FormatDate1(_endDate) +
                            @"') and (isnull(a.exdt,'3000-01-01') > '" + UtilDatetime.FormatDate1(_startDate) +
                            @"' and isnull(a.efdt,'3000-01-01') <='" + UtilDatetime.FormatDate1(_endDate) + @"') 
                            and a.iaod='Y' and (" + _sStaffSql + @") 
                            order by a.efdt";

            List<tatroshi> obj = gDB.ExecuteQuery<tatroshi>(sSql).ToList();

            return obj;
        }
    }
}