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
    public class rcapplicDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                string sSql1 = BuildWhereSql("apno,jbcd,jbnm,dpcd,from|lmtm,to|lmtm,lmur", _parameter);

                var q = from p in gDB.trcapplics.Where(sSql1)
                        select new
                        {
                            p.apdt,p.apno,p.dpcd,p.exdt,p.hcnt,p.hctl,p.jbcd,p.jbde,p.jbnm,
                            p.jbre,p.lmtm,p.lmur,p.remk,p.rfid,p.rtcd,p.stcd,p.stus,p.vacn,
                            p.whhi,p.tbsdepmnt.dpnm,p.tbsstftyp.stnm,p.trcrcttyp.rtnm
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

        public List<object> GetActiveApplication(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                string sSql1 = BuildWhereSql("apno,jbcd,jbnm,dpcd,from|lmtm,to|lmtm,lmur", _parameter);

                var q = from p in gDB.trcapplics.Where(sSql1)
                        where (p.stus == "Created" || p.stus == "Approved" || p.stus == "Processing")
                        select new
                        {
                            p.apdt,
                            p.apno,
                            p.dpcd,
                            p.exdt,
                            p.hcnt,
                            p.hctl,
                            p.jbcd,
                            p.jbde,
                            p.jbnm,
                            p.jbre,
                            p.lmtm,
                            p.lmur,
                            p.remk,
                            p.rfid,
                            p.rtcd,
                            p.stcd,
                            p.stus,
                            p.vacn,
                            p.whhi,
                            p.tbsdepmnt.dpnm,
                            p.tbsstftyp.stnm,
                            p.trcrcttyp.rtnm
                        };

                List<object> obj = q.Cast<object>().ToList();

                totalRecordCount = obj.Count;

                List<object> appList = null;

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
