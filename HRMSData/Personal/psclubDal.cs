using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class psclubDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("clcd,mbty,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsclubs.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbsclubs on p.clcd equals s.clcd
                        join o in gDB.tbscurncies on p.crcd equals o.crcd into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            p.sqno,
                            p.lmtm,
                            p.lmur,
                            t.sfid,
                            stfn = t.ntnm,

                            p.clcd,
                            p.exdt,
                            p.clna,
                            p.cmfe,
                            p.crcd,
                            p.jodt,
                            p.jofe,
                            p.mbty,
                            p.ssfe,
                            s.clnm,
                            c1.crnm,
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

    }
}
