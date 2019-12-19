using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSData.Reporting
{
    public class rpexrpdfDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("rpcd,rpnm,from|lmtm,to|lmtm,lmur", _parameter);

                var q = from p in gDB.trpexrpdfs.Where(sSql1)
                        select new
                        {
                            p.lmtm,p.lmur,p.remk,p.rfid,p.rpcd,p.rpnm,
                            p.rptt,p.rpty
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
