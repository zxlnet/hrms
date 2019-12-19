using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.LanguageResources;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class psempchgDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("chre,ctcd,gpid,from|efdt,to|efdt", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsempchgs.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tstdefcfgs on p.chby equals s.dfnm
                        join o in gDB.tbschgtyps on p.ctcd equals o.ctcd
                        select new
                        {
                            p.chfi,
                            p.chre,
                            p.ctcd,
                            p.efdt,
                            p.emno,
                            p.neva,
                            p.sqno,
                            t.sfid,
                            stfn = t.ntnm,
                            s.dfrs,
                            s.firs,
                            s.finm,
                            s.dfnm,
                            p.gpid,
                            p.isby,
                            p.isdt,
                            p.isim,
                            p.issu,
                            p.lmtm,
                            p.lmur,
                            p.chft,
                            p.nevt,
                            s.dasc,
                            p.chby,
                            dfbytext=HRMSRes.ResourceManager.GetString(s.dfrs),
                            o.ctnm,
                            p.rfid};

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
