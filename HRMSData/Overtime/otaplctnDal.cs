using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Overtime
{
    public class otaplctnDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("otcd,from|apdt,to|apdt,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.totaplctns.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tottypes on p.otcd equals s.otcd
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            p.lmtm,
                            p.lmur,
                            p.remk,
                            p.apdt,
                            p.frtm,
                            p.apno,
                            p.othr,
                            p.othm,
                            p.otst,
                            p.otcd,
                            p.totm,
                            frtm_date = UtilDatetime.FormatDate1(p.frtm),
                            frtm_time = UtilDatetime.FormatTime1(p.frtm),
                            totm_date = UtilDatetime.FormatDate1(p.totm),
                            totm_time = UtilDatetime.FormatTime1(p.totm)
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

        public string getMaxAppNo()
        {
            try
            {
                string t = UtilDatetime.FormatDate3(DateTime.Now);
                var q = (from p in gDB.totaplctns
                         where p.apno.Contains(t)
                         select p.apno).Max();

                if (q == null)
                    return string.Empty;

                return q.ToString();
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

        public double getWeekothrByEmp(vw_employment empInfo, string _otcd, DateTime _overtimeDate)
        {
            try
            {
                DateTime weekStart = _overtimeDate.AddDays(-1 * (Convert.ToInt16(_overtimeDate.DayOfWeek)));
                DateTime weekEnd = _overtimeDate.AddDays(7 - Convert.ToInt16(_overtimeDate.DayOfWeek));

                //查询本周消耗数
                var q = (from p in gDB.totdetails
                         where (p.sttm >= weekStart)
                         && (p.edtm < weekEnd.AddDays(1))
                         && (p.otcd == _otcd)
                         && (p.emno == empInfo.emno)
                         select new { othr = (((p.othm.HasValue == false) || (p.othm == 0)) ? p.othr : p.othm) }).ToList();

                //double daysConsumed = Convert.ToDouble(q.Sum());

                double daysConsumed = 0;
                for (int i = 0; i < q.Count;i++ )
                {
                    daysConsumed += q[i].othr.Value;
                }

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

        public double getMonthothrByEmp(vw_employment empInfo, string _otcd, DateTime _overtimeDate)
        {
            try
            {
                //查询本月消耗数
                var q = (from p in gDB.totdetails
                         where p.sttm.Month == _overtimeDate.Month
                         && p.sttm.Year == _overtimeDate.Year
                         && p.otcd == _otcd
                         && p.emno == empInfo.emno
                         select new { othr = (((p.othm.HasValue == false) || (p.othm == 0)) ? p.othr : p.othm) }).ToList();

                //double daysConsumed = Convert.ToDouble(q);
                double daysConsumed = 0;
                for (int i = 0; i < q.Count; i++)
                {
                    daysConsumed += q[i].othr.Value;
                }

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

        public double getYearothrByEmp(vw_employment empInfo, string _otcd, DateTime _overtimeDate)
        {
            try
            {
                //查询本年消耗数
                var q = (from p in gDB.totdetails
                         where p.sttm.Year == _overtimeDate.Year
                         && p.otcd == _otcd
                         && p.emno == empInfo.emno
                         select new { othr = (((p.othm.HasValue == false) || (p.othm == 0)) ? p.othr : p.othm) }).ToList();

                //double daysConsumed = Convert.ToDouble(q);
                double daysConsumed = 0;
                for (int i = 0; i < q.Count; i++)
                {
                    daysConsumed += q[i].othr.Value;
                }

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

        public double getWeekTTLVHoursByEmp(vw_employment empInfo, string _otcd, DateTime _overtimeDate)
        {
            try
            {
                DateTime weekStart = _overtimeDate.AddDays(-1 * (Convert.ToInt16(_overtimeDate.DayOfWeek)));
                DateTime weekEnd = _overtimeDate.AddDays(7 - Convert.ToInt16(_overtimeDate.DayOfWeek));

                //查询本周消耗数
                var q = (from p in gDB.totdetails
                         where (p.sttm >= weekStart)
                         && (p.edtm < weekEnd.AddDays(1))
                         && (p.otcd == _otcd)
                         && (p.emno == empInfo.emno)
                         select p.tlhr).Sum();

                double tlhrConsumed = Convert.ToDouble(q);

                return tlhrConsumed;
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

        public double getMonthTTLVHoursByEmp(vw_employment empInfo, string _otcd, DateTime _overtimeDate)
        {
            try
            {
                //查询本月消耗数
                var q = (from p in gDB.totdetails
                         where p.sttm.Month == _overtimeDate.Month
                         && p.sttm.Year == _overtimeDate.Year
                         && p.otcd == _otcd
                         && p.emno == empInfo.emno
                         select p.tlhr).Sum();

                double tlhrConsumed = Convert.ToDouble(q);
                return tlhrConsumed;
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

        public double getYearTTLVHoursByEmp(vw_employment empInfo, string _otcd, DateTime _overtimeDate)
        {
            try
            {
                //查询本年消耗数
                var q = (from p in gDB.totdetails
                         where p.sttm.Year == _overtimeDate.Year
                         && p.otcd == _otcd
                         && p.emno == empInfo.emno
                         select p.tlhr).Sum();

                double tlhrConsumed = Convert.ToDouble(q);

                return tlhrConsumed;
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

        public void DeleteOTDtl(List<ColumnInfo> _parameters)
        {
            DoMultiDelete<totdetail>(_parameters);
        }
    }
}
