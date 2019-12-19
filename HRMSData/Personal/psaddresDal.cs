using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class psaddresDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("addr,atcd,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsaddres.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbsaddtyps on p.atcd equals s.atcd
                        join o in gDB.tbsareas on p.arcd equals o.arcd into t1
                        from cp in t1.DefaultIfEmpty()
                        select new
                        {
                            p.addr,
                            p.atcd,
                            p.arcd,
                            p.emno,
                            p.isdf,
                            p.lmtm,
                            p.lmur,
                            p.pscd,
                            p.sqno,
                            p.remk,
                            s.atnm,
                            t.sfid,
                            stfn = t.ntnm,
                            cp.arnm,
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
