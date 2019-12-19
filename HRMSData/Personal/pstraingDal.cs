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
    public class pstraingDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("ttcd,orga,cunm,incm,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpstraings.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join o in gDB.tbstratyps on p.ttcd equals o.ttcd
                        join s in gDB.tbscurncies on p.crcd equals s.crcd into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            p.sqno,
                            p.remk,
                            p.lmtm,
                            p.lmur,
                            t.sfid,
                            stfn = t.ntnm,

                            p.bnag,
                            p.cufe,
                            p.cunm,
                            p.ctpw,
                            p.crcd,
                            p.endt,
                            p.loca,
                            p.natu,
                            p.orga,
                            p.paby,
                            p.redt,
                            p.resu,
                            p.stdt,
                            p.tmpe,
                            p.tttm,
                            p.trcd,
                            p.ttcd,
                            o.ttnm,
                            c1.crnm,
                            p.incm,
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
