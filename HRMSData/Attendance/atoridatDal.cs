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
    public class atoridatDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|retm,to|retm,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tatoridats.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.sfid equals t.sfid
                        where p.sfid.Contains(GetColumnValue("sfid", _parameter))
                        && t.ntnm.Contains(GetColumnValue("stfn", _parameter))
                        select new
                        {
                            p.sfid,
                            stfn = t.ntnm,
                            p.retm,
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

        public List<tatoridat> GetOriginalData(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            string sSql = @"select a.* 
                              from tatoridat a 
                              inner join vw_employment emp 
                              on a.sfid = emp.sfid 
                            where (" + _sStaffSql + @") 
                            and retm>='" + UtilDatetime.FormatDate1(_startDate) + @"'  
                            and retm <'" + UtilDatetime.FormatDate1(_endDate) + "'";

            List<tatoridat> obj = gDB.ExecuteQuery<tatoridat>(sSql).ToList();

            return obj;
        }


    }
}
