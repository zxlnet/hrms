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
    public class psinsurnDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                string sSql1 = BuildWhereSql("innm,iccd,itcd,iscd,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsinsurns.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbsinscmps on p.iccd equals s.iccd
                        join o in gDB.tbsinstyps on p.itcd equals o.itcd 
                        join r in gDB.tbsinsscms on p.iscd equals r.iscd into t2
                        from c2 in t2.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            p.sqno,
                            p.lmtm,
                            p.lmur,
                            t.sfid,
                            stfn = t.ntnm,

                            p.efdt,
                            p.exdt,
                            p.iscd,
                            p.iccd,
                            p.itcd,
                            p.isdt,
                            p.momn,
                            p.resu,
                            p.ttmo,
                            p.wima,
                            s.icnm,
                            c2.isnm,
                            o.itnm,
                            p.innm,
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
