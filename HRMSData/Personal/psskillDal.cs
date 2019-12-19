using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class psskillDal:BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("stcd,sknm,skle,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsskills.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbsskltyps on p.stcd equals s.stcd
                        select new
                        {
                            p.emno,
                            p.sqno,
                            p.remk,
                            p.lmtm,
                            p.lmur,
                            t.sfid,
                            stfn = t.ntnm,
                            p.skle,
                            p.sknm,
                            p.stcd,
                            s.stnm,
                            p.tstrecst.stus,
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

        public List<object> GetSkillItems(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                var q = from p in gDB.tbssklitms
                        join o in gDB.tpsskiitms.Where(p1 => p1.emno.Contains(GetColumnValue("emno", _parameter)) && p1.sqno.Equals(GetColumnValue("sqno", _parameter)))
                        on p.sicd equals o.sicd into t1
                        from c1 in t1.DefaultIfEmpty()
                        where p.stcd.Contains(GetColumnValue("stcd", _parameter))
                        select new
                        {
                            sicd = c1.sicd == null ? p.sicd : c1.sicd,
                            racd = c1.racd == null ? p.racd : c1.racd,
                            p.stcd,
                            p.sinm,
                            p.tbsrating.ranm,
                            p.tbsskltyp.stnm,
                            sqno = c1.sqno == null ? 0 : c1.sqno,
                            c1.emno,
                            c1.remk,
                            c1.rtcd,
                            c1.lmtm,
                            c1.lmur,
                            c1.tbsratdtl.rtnm
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
    }
}
