using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class psquafcaDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("innm,qunm,qfcd,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsquafcas.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbsqlfctns on p.qfcd equals s.qfcd
                        join o in gDB.tbscontries on p.cocd equals o.cocd into t1
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

                            p.cocd,
                            p.frdt,
                            p.innm,
                            p.natu,
                            p.obyr,
                            p.qfcd,
                            p.qucd,
                            p.qunm,
                            p.stge,
                            p.todt,
                            s.qfnm,
                            c1.conm,
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
