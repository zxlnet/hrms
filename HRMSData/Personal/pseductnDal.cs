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
    public class pseductnDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("elcd,stwa,scho,cocd,subj,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpseductns.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbsedulevs on p.elcd equals s.elcd into t1
                        from c1 in t1.DefaultIfEmpty() 
                        join o in gDB.tbscontries on p.cocd equals o.cocd into t2
                        from c2 in t2.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            p.sqno,
                            p.remk,
                            p.lmtm,
                            p.lmur,

                            p.acad,
                            p.cocd,
                            p.dept,
                            p.elcd,
                            p.frdt,
                            p.ishi,
                            p.loca,
                            p.natu,
                            p.resu,
                            p.scho,
                            p.spec,
                            p.stwa,
                            p.subj,
                            p.todt,
                            t.sfid,
                            stfn = t.ntnm,
                            c1.elnm,
                            c2.conm,
                            p.tstrecst.stus,
                            p.rfid
                        }
                        ;

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
