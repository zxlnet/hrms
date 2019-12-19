using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using System.Linq.Dynamic;
using GotWell.Utility;
using GotWell.Model.Common;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSData.Recruitment
{
    public class rcintschDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

//                string sSql = @"select b.aino,b.cnnm,b.ennm,a.isno,a.frtm,a.totm,a.hrof,a.otof,a.iacd,a.remk,a.stus,a.lmtm,a.lmur,a.rfid
//                                        from trcintsch a right outer join 
//	                                            (select b.* from trcaplinf b,trcapplic c where b.apno = c.apno and c.apno='" + _parameter[0].ColumnValue + @"') b
//                                        on a.aino = b.aino  ";

//                IEnumerable<T> appList = gDB.ExecuteQuery<T>(sSql);
//                return appList.ToList();

                string sSql1 = BuildWhereSql("apno", _parameter);

                var q = from p in gDB.vw_rcintsches.Where(sSql1)
                        select new
                        {
                           p.aino,p.apno,p.cnnm,p.ennm,p.frtm,p.hrof,p.iacd,p.isno,p.lmtm,p.lmur,
                           p.otof,p.remk,p.rfid,p.stus,p.totm,p.iade
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

        public void DeleteInterviewSchedule(List<ColumnInfo> _parameter)
        {
            try
            {
                var q = from p in gDB.trcintsches
                        where p.trcaplinf.apno == _parameter[0].ColumnValue
                        select p;

                List<trcintsch> lstDtl = q.ToList<trcintsch>();

                for (int i = 0; i < lstDtl.Count; i++)
                {
                    gDB.trcintsches.DeleteOnSubmit(lstDtl[i]);
                }

                gDB.SubmitChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
