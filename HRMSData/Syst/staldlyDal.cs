using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Common;

namespace GotWell.HRMS.HRMSData.Syst
{
    public class staldlyDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                var q = from p in gDB.tstaldlybs
                        where (p.reci=="Public" || p.reci == _parameter[0].ColumnValue)
                        && p.alst == Alarm_AlarmStatus.Unhandled.ToString()
                        && p.extm > DateTime.Now
                        select p;

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
