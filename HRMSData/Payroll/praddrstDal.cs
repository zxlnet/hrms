using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class praddrstDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<GotWell.Model.Common.ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpraddrsts.Where(sSql1)
                        join s in gDB.vw_employments.Where(sSql2) on p.emno equals s.emno
                        select new
                        {
                            p.crcd,p.ctfr,p.emno,p.isca,p.itcd,p.lmtm,p.lmur,p.pdcd,
                            p.perd,p.remk,p.rlcd,p.sqno,p.valu,
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
            //int? ret = gDB.tpraddrsts.Where(p => p.emno == _parameters[0].ColumnValue).Max(p => p.sqno);
            var q = (from p in gDB.tpraddrsts
                    where p.emno == _parameters[0].ColumnValue
                    select p.sqno).ToList();

            if (q.Count <1) return 0;

            return q.Max();
        }

        public List<tpraddrst> GetAdditionalRuleSetDtlForPayroll(string _sStaffSql, DateTime _startDate, DateTime _endDate,string _payDay)
        {
            string sSql = "select a.* from tpraddrst a,vw_employment emp where a.emno = emp.emno " +
                           (_sStaffSql.Trim() == string.Empty ? "" : " and " + _sStaffSql) +
                        " and a.perd ='" + UtilDatetime.FormateDateTime1(_endDate.AddDays(-1)) + "' " +
                        (_payDay.Equals(string.Empty) ? string.Empty : " and pdcd ='" + _payDay + "'");

            IEnumerable<tpraddrst> ret = gDB.ExecuteQuery<tpraddrst>(sSql);
            return ret.ToList();
        }

    }
}
