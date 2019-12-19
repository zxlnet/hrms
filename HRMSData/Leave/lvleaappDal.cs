using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using GotWell.Model.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Leave
{
    public class lvleaappDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("ltcd,lrcd,from|apdt,to|apdt,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tlvleaapps.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tlvleatyps on p.ltcd equals s.ltcd
                        join o in gDB.tlvlearsns on p.lrcd equals o.lrcd
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            p.lmtm,
                            p.lmur,
                            p.remk,
                            p.apdt,
                            p.baaf,
                            p.csaf,
                            p.frtm,
                            p.hurs,
                            p.days,
                            p.ltcd,
                            p.lrcd,
                            p.lvst,
                            p.apno,
                            p.retm,
                            p.totm,
                            s.ltnm,
                            o.lrnm,
                            fromtime_date = UtilDatetime.FormatDate1(p.frtm),
                            fromtime_time = UtilDatetime.FormatTime1(p.frtm),
                            totime_date = UtilDatetime.FormatDate1(p.totm),
                            totime_time=UtilDatetime.FormatTime1(p.totm),
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

        public List<tlvleaapd> GetLeaveApps(string _sSqlStaff, DateTime _startDate, DateTime _endDate)
        {
            try
            {
                string sSql = @"select a.* from tlvleaapd a,vw_employment emp
                                where a.emno = emp.emno
                                and   " + _sSqlStaff + @" "; //and a.lvst='1'

                List<tlvleaapd> obj = gDB.ExecuteQuery<tlvleaapd>(sSql).ToList();

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

        public string getMaxAppNo()
        {
            try
            {
                string t = UtilDatetime.FormatDate3(DateTime.Now);
                var q = (from p in gDB.tlvleaapps
                         where p.apno.Contains(t)
                         select p.apno).Max();

                if (q == null)
                    return string.Empty;

                return q.ToString();
            }
            catch(UtilException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }

        public double getWeekConsumedByEmp(vw_employment empInfo, string _ltcd, DateTime leaveDate)
        {
            try
            {
                DateTime weekStart = leaveDate.AddDays(-1 * (Convert.ToInt16(leaveDate.DayOfWeek)));
                DateTime weekEnd = leaveDate.AddDays(7 - Convert.ToInt16(leaveDate.DayOfWeek));

                //查询本周消耗数
                var q = (from p in gDB.tlvleaapds
                         where (p.frtm >= weekStart)
                         && (p.totm < weekEnd.AddDays(1))
                         && (p.ltcd == _ltcd)
                         && (p.emno == empInfo.emno)
                         select p.hurs).ToList();

                double daysConsumed = 0;

                if (q.Count > 0)
                    daysConsumed = Convert.ToDouble(q.Sum());           

                return daysConsumed;
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

        public double getMonthConsumedByEmp(vw_employment empInfo, string _ltcd, DateTime leaveDate)
        {
            try
            {
                //查询本月消耗数
                var q = (from p in gDB.tlvleaapds
                         where p.frtm.Month == leaveDate.Month
                         && p.frtm.Year == leaveDate.Year
                         && p.ltcd == _ltcd
                         && p.emno == empInfo.emno
                         select p.hurs).ToList();

                double daysConsumed = 0;

                if (q.Count > 0)
                    daysConsumed = Convert.ToDouble(q.Sum());

                return daysConsumed;
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

        public double getYearConsumedByEmp(vw_employment empInfo, string _ltcd, DateTime leaveDate)
        {
            try
            {
                //查询本年消耗数
                var q = (from p in gDB.tlvleaapds
                         where p.frtm.Year == leaveDate.Year
                         && p.ltcd == _ltcd
                         && p.emno == empInfo.emno
                         select p.hurs).ToList();

                double daysConsumed = 0;

                if (q.Count > 0)
                    daysConsumed = Convert.ToDouble(q.Sum());

                return daysConsumed;
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


        public void DeleteLeaveAppDtl(List<ColumnInfo> _parameters)
        {
            DoMultiDelete<tlvleaapd>(_parameters);
        }

        public List<tlvleaapd> GetLeaveDetailForPayroll(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            string sSql = "select a.* from tlvleaapd a,vw_employment emp where a.emno = emp.emno " +
                           (_sStaffSql.Trim() == string.Empty ? "" : " and " + _sStaffSql) +
                        " and a.frtm >='" + UtilDatetime.FormateDateTime1(_startDate) + "' " +
                        " and a.frtm <'" + UtilDatetime.FormateDateTime1(_endDate) + "' ";

            IEnumerable<tlvleaapd> ret = gDB.ExecuteQuery<tlvleaapd>(sSql);
            return ret.ToList();

        }

    }
}
