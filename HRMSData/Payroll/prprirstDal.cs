using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class prprirstDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<GotWell.Model.Common.ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprprirsts
                        join s in gDB.vw_employments.Where(sSql2) on p.emno equals s.emno
                        select new
                        {
                            p.emno,
                            p.crcd,
                            p.cred,
                            p.efdt,
                            p.exdt,
                            p.isca,
                            p.itcd,
                            p.lmtm,
                            p.lmur,
                            p.pdcd,
                            p.rlcd,
                            p.sqno,
                            p.ssty,
                            p.valu,
                            p.vtyp,
                            p.cnfr,
                            p.tbscurncy.crnm,
                            padt = p.tprpayday == null ? -1 : p.tprpayday.padt,
                            p.tprsalitm.itnm
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

        public override int? GetMaxsqno(List<ColumnInfo> _parameters)
        {
            int? ret = gDB.tprprirsts.Where(p => p.emno == _parameters[0].ColumnValue).Select(p => p.sqno).Max();

            return ret;
        }

        public List<tprprirst> GetPrivateRuleSetDtlForPayroll(string _sStaffSql,DateTime _startDate, DateTime _endDate,string _payDay)
        {
            string sSql = "select a.* from tprprirst a,vw_employment emp where a.emno = emp.emno " +
                           (_sStaffSql.Trim() == string.Empty ? "" : " and " + _sStaffSql) +
                        " and a.efdt <='" + UtilDatetime.FormateDateTime1(_startDate) + "' " +
                        " and (a.exdt is null or a.exdt >'" + UtilDatetime.FormateDateTime1(_endDate) + "') " +
                        (_payDay.Equals(string.Empty) ? string.Empty : " and pdcd ='" + _payDay + "'");

            IEnumerable<tprprirst> ret = gDB.ExecuteQuery<tprprirst>(sSql);
            return ret.ToList();

        }

    }
}
