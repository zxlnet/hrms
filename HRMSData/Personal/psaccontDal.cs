using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class psaccontDal : BaseDal
    {

        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount) 
        {
            try
            {

                string sSql1 = BuildWhereSql("bkcd,idno,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsacconts.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbsbanks on p.bkcd equals s.bkcd
                        join o in gDB.tbspaytyps on p.ptcd equals o.ptcd into t1
                        from c1 in t1.DefaultIfEmpty()
                        join r in gDB.tbsreltyps on p.rtcd equals r.rtcd into t2
                        from c2 in t2.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            p.sqno,
                            p.lmtm,
                            p.lmur,
                            t.sfid,
                            stfn = t.ntnm,

                            p.bkcd,
                            p.acno,
                            p.bfnm,
                            p.idno,
                            p.rtcd,
                            p.isdf,
                            p.ptcd,
                            p.ifpe,
                            p.amnt,
                            p.perc,
                            p.ifot,
                            p.funm,
                            p.icno,
                            p.fval,
                            p.ftyp,
                            s.bknm,
                            c1.ptnm,
                            c2.rtnm,
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
