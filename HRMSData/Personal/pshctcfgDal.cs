using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.Utility;
using GotWell.Model.Common;
using System.Linq.Dynamic;
using GotWell.Model.HRMS;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class pshctcfgDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("hccd,hcnm,from|lmtm,to|lmtm,lmur", _parameter);

                var q = from p in gDB.tpshctcfgs.Where(sSql1)
                        join o in gDB.tstdefcfgs on p.xdef equals o.dfnm
                        join s in gDB.tstdefcfgs on p.ydef equals s.dfnm
                        select new
                        {
                            p.hccd,p.hcnm,p.lmtm,p.lmur,p.remk,p.xdef,p.ydef,
                            xtxt = HRMSRes.ResourceManager.GetString(o.dfrs),
                            ytxt = HRMSRes.ResourceManager.GetString(s.dfrs),
                            p.rfid
                        };

                List<T> obj = q.Cast<T>().Distinct().ToList();

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
