using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using System.Linq.Dynamic;
using GotWell.Utility;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSData.Recruitment
{
    public class rcintrdtDal : BaseDal
    {
         public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                string sSql1 = BuildWhereSql("isno", _parameter);
                string sSql2 = BuildWhereSql("iacd", _parameter);

                var q = from p in gDB.trcintadts.Where(sSql2)
                        join o in gDB.trcintrdts.Where(sSql1) on new { p.iacd, p.sqno } equals new { o.iacd, o.sqno } into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            p.iacd,
                            c1.isno,
                            p.sqno,
                            p.itnm,
                            p.itde,
                            p.racd,
                            c1.rtcd,
                            c1.remk,
                            c1.rfid,
                            c1.tbsratdtl.rtnm,
                            c1.tbsrating.ranm
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
