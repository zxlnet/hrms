using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atabssumDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tatabssums
                        join t in gDB.vw_employments on p.emno equals t.emno
                        select new
                        {
                            p.emno,
                            p.lact,
                            p.abda,
                            p.abhr,
                            p.ahrr,
                            p.eact,
                            p.perd,
                            t.sfid,
                            stfn = t.ntnm,
                            p.lmtm,
                            p.lmur,
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
