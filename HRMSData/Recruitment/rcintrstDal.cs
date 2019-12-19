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
    public class rcintrstDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                string sSql1 = BuildWhereSql("cnnm,ennm,jbcd,jbnm", _parameter);

                var q = from p in gDB.vw_rcintrsts.Where(sSql1)
                        select new
                        {
                            p.aino,p.cnnm,p.ennm,p.frtm,p.hrof,
                            p.isno,p.iacd,p.itrs,p.lmtm,p.lmur,p.otof,
                            p.ovas,p.recm,p.remk,p.rfid,p.scty,p.totm,
                            p.jbcd,p.jbnm
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
