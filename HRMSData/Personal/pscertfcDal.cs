using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class pscertfcDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("ctcd,cfno,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpscertfcs.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbscertyps on p.ctcd equals s.ctcd
                        join o in gDB.tbscontries on p.cocd equals o.cocd into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            p.cfno,
                            p.cfnm,
                            p.ctcd,
                            s.ctnm,
                            p.cocd,
                            c1.conm,
                            p.efdt,
                            p.emno,
                            p.exdt,
                            p.isnm,
                            p.isdt,
                            p.lmtm,
                            p.lmur,
                            p.remk,
                            p.sqno,
                            t.sfid,
                            stfn = t.ntnm,
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
