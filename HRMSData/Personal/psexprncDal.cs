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
    public class psexprncDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                string sSql1 = BuildWhereSql("cmnm,emnm,jbds,trcd,fpos,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsexprncs.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbsterrsns on p.trcd equals s.trcd into t1
                        from c1 in t1.DefaultIfEmpty()
                        join o in gDB.tbsbusntrs on p.bncd equals o.bncd into t2
                        from c2 in t2.DefaultIfEmpty()
                        join r in gDB.tbscontries on p.cocd equals r.cocd into t3
                        from c3 in t3.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            p.sqno,
                            p.lmtm,
                            p.lmur,
                            p.remk,
                            t.sfid,
                            stfn = t.ntnm,

                            p.bott,
                            p.cmsc,
                            p.cmnm,
                            p.bncd,
                            p.ctno,
                            p.cocd,
                            p.dept,
                            p.emnm,
                            p.fpos,
                            p.frdt,
                            p.imbo,
                            p.iscm,
                            p.jbds,
                            p.saly,
                            p.stsa,
                            p.trcd,
                            p.todt,
                            p.wkyr,
                            c1.trnm,
                            c2.bnnm,
                            c3.conm,
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
