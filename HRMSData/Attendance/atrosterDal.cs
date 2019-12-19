using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atrosterDal : BaseDal
    {
        public List<object> GetRosterDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("rscd,rsnm,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tatrosdtls.Where(sSql1)
                        join t in gDB.tatshifts.Where(sSql2) on p.sfcd equals t.sfcd into t1
                        from c1 in t1.DefaultIfEmpty()
                        join s in gDB.tottypes on p.otcd equals s.otcd into t2
                        from c2 in t2.DefaultIfEmpty()
                        orderby p.dasq
                        select new
                        {
                            p.dasq,
                            p.isrt,
                            p.lmtm,
                            p.lmur,
                            p.rscd,
                            p.sfcd,
                            c1.sfnm,
                            p.otcd,
                            c2.otnm,
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

        public List<tatrosdtl> GetRosterDetails(DateTime _startDate,DateTime _endDate)
        {
            try
            {
                var q = from p in gDB.tatrosdtls
                        join t in gDB.tatshifts on p.sfcd equals t.sfcd
                        join s in gDB.tatrosters on p.rscd equals s.rscd
                        where (s.efdt.Value.CompareTo(_startDate) <= 0)
                        //&& ((_endDate == null) || (s.efdt.Value.CompareTo(_endDate) < 0))
                        && ((s.exdt.HasValue == false) || ((_endDate == null) || (s.exdt.Value.CompareTo(_endDate) > 0)))
                        select p;

                List<tatrosdtl> obj = q.ToList();

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
    }
}