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
    public class prpbrhisDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|efdt,to|efdt", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprpbrhis.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        select new
                        {
                           p.efdt,p.emno,p.exdt,p.lmtm,p.lmur,p.rscd,t.sfid,
                           stfn = t.ntnm,p.tprpubrst.rsnm
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

        public List<tprpbrhi> GetStaffPubRuleSetHis(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            string sSql = "select a.* from tprpbrhis a,vw_employment emp where a.emno = emp.emno " +
                           (_sStaffSql.Trim() == string.Empty ? "" : " and " + _sStaffSql) +
                        " and a.efdt <='" + UtilDatetime.FormateDateTime1(_startDate) + "' " +
                        " and (a.exdt is null or a.exdt >'" + UtilDatetime.FormateDateTime1(_endDate) + "') ";

            IEnumerable<tprpbrhi> ret = gDB.ExecuteQuery<tprpbrhi>(sSql);
            return ret.ToList();
        }
    }
}
