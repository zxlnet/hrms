using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;
using GotWell.Common;
using System.Reflection;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atanarstDal : BaseDal
    {
        private int DataScale = Int16.Parse((Parameter.CURRENT_SYSTEM_CONFIG as StSystemConfig).AtDS);

        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                string sSql1 = BuildWhereSql("from|atdt,to|atdt,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tatanarsts.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tatshifts on p.sfcd equals s.sfcd into t1
                        from cp in t1.DefaultIfEmpty()
                        join o in gDB.tatrosters on p.rscd equals o.rscd into t2
                        from cq in t2.DefaultIfEmpty()
                        select new
                        {
                            p.atdt,
                            p.athr,
                            p.atst,
                            p.bret,
                            p.betm,
                            p.brst,
                            p.bstm,
                            p.emno,
                            p.ifma,
                            p.intm,
                            p.itmm,
                            p.iscf,
                            p.isrt,
                            p.ottm,
                            p.otmm,
                            p.rscd,
                            p.sfcd,
                            cp.sfnm,
                            cq.rsnm,
                            t.sfid,
                            stfn = t.ntnm,
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

        public List<object> GetAnalyzeResult(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
//                string sSql = @"select emno,sfid,sfnm,dpcd,dpnm,
//                                    atdt,abhours,ahrr,ahrm,lact,
//                                    lctm,eact,ectm,abdays,
//                                    adam,ehrm,rsnm,sfnm,
//                                    rscd,sfcd,intm,btstt,bkedt,outtime
//                                from vw_employment a 
//                                         left outer join tatanarst b on a.emno = b.emno
//                                                on a.atdt>='" + GetColumnValue("from|atdt", _parameter) + @"' 
//                                                and a.atdt<'" + GetColumnValue("to|atdt", _parameter) + @"'
//                                         left outer join tatabsdtl c on a.emno = c.emno
//                                                on a.atdt>='" + GetColumnValue("from|atdt", _parameter) + @"' 
//                                                and a.atdt<'" + GetColumnValue("to|atdt", _parameter) + @"'
//                                where " + _sSqlStaff;

//                List<tatanarst> obj = gDB.ExecuteQuery<tatanarst>(sSql).ToList();

//                return obj;
                double zero = 0.0;

                string sSql1 = BuildWhereSql("from|atdt,to|atdt,atst", _parameter).Replace(".Value","");
                string sSql2 = BuildWhereSql("sfid,stfn,emno,emst,stcd,dvcd,bscd,dpcd,grcd,pscd,jccd,jtcd", _parameter);

                var q = from t in gDB.vw_employments.Where(sSql2)
                        join p in gDB.tatanarsts.Where(sSql1) on t.emno equals p.emno
                        join s in gDB.tatabsdtls on new { t.emno, p.atdt } equals new { s.emno, s.atdt } into t1
                        from cb in t1.DefaultIfEmpty()
                        select new
                        {
                            t.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            t.dpcd,
                            t.dpnm,
                            p.atdt,
                            p.atst,
                            athr = p.athr.HasValue ? Math.Round(p.athr.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            ahrs = cb.ahrs.HasValue ? Math.Round(cb.ahrs.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            ahrr = cb.ahrr.HasValue ? Math.Round(cb.ahrr.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            ahrm = cb.ahrm.HasValue ? Math.Round(cb.ahrm.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            lact = cb.lact.HasValue ? Math.Round(cb.lact.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            lctm = cb.lctm.HasValue ? Math.Round(cb.lctm.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            eact = cb.eact.HasValue ? Math.Round(cb.eact.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            ectm = cb.ectm.HasValue ? Math.Round(cb.ectm.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            abda = cb.abda.HasValue ? Math.Round(cb.abda.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            adam = cb.adam.HasValue ? Math.Round(cb.adam.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            ehrm = cb.ehrm.HasValue ? Math.Round(cb.ehrm.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            eahr = cb.eahr.HasValue ? Math.Round(cb.eahr.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            lahr = cb.lahr.HasValue ? Math.Round(cb.lahr.Value, DataScale, MidpointRounding.AwayFromZero) : zero,
                            p.tatroster.rsnm,
                            p.tatshift.sfnm,
                            p.rscd,
                            p.sfcd,
                            intm = p.ifma == "Y" ? p.itmm : p.intm,
                            brst = p.ifma == "Y" ? p.bstm : p.brst,
                            bret = p.ifma == "Y" ? p.betm : p.bret,
                            ottm = p.ifma == "Y" ? p.otmm : p.ottm,
                            p.remk,
                            p.iscf,
                            p.othr,
                            p.lvhr
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

        public List<tatanarst> GetAnalResult(List<ColumnInfo> _parameter, string _sSqlStaff, DateTime _startDate, DateTime _endDate)
        {
            try
            {
                string sSql = @"select a.* from tatanarst a,vw_employment emp
                                where a.emno = emp.emno
                                and   " + _sSqlStaff + @"
                                and a.atdt>='" + UtilDatetime.FormatDate1(_startDate) + @"' 
                                and a.atdt<'" + UtilDatetime.FormatDate1(_endDate) + @"'";

                List<tatanarst> obj = gDB.ExecuteQuery<tatanarst>(sSql).ToList();

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

        public int DeleteDummyData(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            string sSql = @"delete from tatanarst from 
                           tatanarst b inner join vw_employment emp 
                           on b.emno=emp.emno and " + _sStaffSql + @"
                           where (b.iscf is null or b.iscf='N') and  
                           b.atdt >='" + UtilDatetime.FormatDate1(_startDate) + @"' 
                           and b.atdt<'" + UtilDatetime.FormatDate1(_endDate) + "'";
            object[] parameters = { };
            return gDB.ExecuteCommand(sSql, parameters);

        }
    }
}
