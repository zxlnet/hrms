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
    public class psqurterDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("addr,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsqurters.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        select new
                        {
                            p.emno,
                            p.sqno,
                            p.remk,
                            p.lmtm,
                            p.lmur,
                            t.sfid,
                            stfn = t.ntnm,

                            p.addr,
                            p.agad,
                            p.agcn,
                            p.agfe,
                            p.apag,
                            p.crcd,
                            p.dpam,
                            p.endt,
                            p.mafe,
                            p.natu,
                            p.otfe,
                            p.ownr,
                            p.owad,
                            p.owcn,
                            p.pkfe,
                            p.pbem,
                            p.pbcm,
                            p.rnfe,
                            p.stdt,
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
