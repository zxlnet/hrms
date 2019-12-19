using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;
using System.Linq.Dynamic;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class prsalhisDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("rnno,perd", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprsalhis.Where(sSql1)
                        join s in gDB.vw_employments.Where(sSql2) on p.emno equals s.emno
                        join t in gDB.tprpubrsts on p.rscd equals t.rscd into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            p.ajva,
                            p.crcd,
                            p.tbscurncy.crnm,
                            p.emno,
                            s.sfid,
                            stfn = s.ntnm,
                            p.itcd,
                            p.tprsalitm.itnm,
                            p.lmtm,
                            p.lmur,
                            p.padt,
                            p.perd,
                            p.pkty,
                            p.rscd,
                            p.rnno,
                            p.sqno,
                            p.valu,
                            c1.rsnm,
                            p.rfid,
                            p.isca
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

        public List<tprsalhi> GetSalaryHistoryForAccountAllocate(List<ColumnInfo> _parameter)
        {
            try
            {
                string sSql1 = BuildWhereSql("rnno", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprsalhis.Where(sSql1)
                        join s in gDB.tprsalitms on p.itcd equals s.itcd
                        where s.bkal == "Y"
                        select p;

                List<tprsalhi> obj = q.Cast<tprsalhi>().ToList();

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

        public List<tprsalhi> GetSalaryHistoryForCCAllocate(List<ColumnInfo> _parameter)
        {
            try
            {
                string sSql1 = BuildWhereSql("rnno", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprsalhis.Where(sSql1)
                        join s in gDB.tprsalitms on p.itcd equals s.itcd
                        where s.csal == "Y"
                        &&      p.isca == "Y"
                        select p;

                List<tprsalhi> obj = q.Cast<tprsalhi>().ToList();

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

        public List<tprsalhi> GetSalaryHistoryForCumulation(List<ColumnInfo> _parameter)
        {
            try
            {
                string sSql1 = BuildWhereSql("rnno", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tprsalhis.Where(sSql1)
                        join s in gDB.tprsalitms on p.itcd equals s.itcd
                        where s.iscu == "Y"
                        select p;

                List<tprsalhi> obj = q.Cast<tprsalhi>().ToList();

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
