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
    public class atdatmnuDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|atdt,to|atdt,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tatdatmnus.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tatshifts on p.scdm equals s.sfcd into t1
                        from cp in t1.DefaultIfEmpty()
                        select new
                        {
                            t.sfid,
                            stfn = t.ntnm,
                            p.adam,
                            p.ahrm,
                            p.atdt,
                            p.betm,
                            p.bstm,
                            p.ectm,
                            p.emno,
                            p.ittm,
                            p.lmtm,
                            p.lmur,
                            p.lctm,
                            p.ottm,
                            p.remk,
                            p.scdm,
                            cp.sfnm,
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

        public List<tatdatmnu> GetManualData(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            string sSql = @"select a.* 
                              from tatdatmnu a 
                              inner join vw_employment emp 
                              on a.emno = emp.emno 
                            where (" + _sStaffSql + @") 
                            and atdt>='" + UtilDatetime.FormatDate1(_startDate) + @"'  
                            and atdt <'" + UtilDatetime.FormatDate1(_endDate) + "'";

            List<tatdatmnu> obj = gDB.ExecuteQuery<tatdatmnu>(sSql).ToList();

            return obj;
        }

        
        public int DeleteDummyData(string _sStaffSql,DateTime _startDate,DateTime _endDate)
        {
            string sSql = @"delete from tatdatmnu from tatdatmnu a where 
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
    }
}
