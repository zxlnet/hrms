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
    public class atcaldarDal : BaseDal
    {

        public List<object> GetCalendarDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("clcd", _parameter);

                var q = from p in gDB.tatclddtls.Where(sSql1)
                        join t in gDB.tatcaldars on p.clcd equals t.clcd
                        join x in gDB.tathldtyps on p.htcd equals  x.htcd into t1
                        from cp in t1.DefaultIfEmpty()
                        join y in gDB.tottypes on p.otcd equals y.otcd into t2
                        from cq in t2.DefaultIfEmpty()
                        orderby p.cddt
                        select new
                        {
                            p.clcd,
                            p.cddt,
                            p.htcd,
                            p.lmtm,
                            p.lmur,
                            p.otcd,
                            p.remk,
                            t.cdnm,
                            cq.otnm,
                            cp.htnm,
                            p.rfid
                        };

                List<object> obj = q.Cast<object>().ToList();

                totalRecordCount = obj.Count;

                List<object> appList = null;

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

        public List<tatclddtl> GetCalendars(DateTime _startDate, DateTime _endDate)
        {
            try
            {
                var q = (from p in gDB.tatclddtls
                         where p.cddt >= _startDate
                         && p.cddt < _endDate
                         select p).ToList();

                return q;

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