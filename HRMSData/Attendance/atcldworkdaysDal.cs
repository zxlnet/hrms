using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atcldwkdsDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                var q = from p in gDB.tatcldwkds
                        join o in gDB.tatcaldars on p.clcd equals o.clcd
                        where o.clcd.Contains(GetColumnValue("clcd", _parameter))
                        select new
                        {
                            p.clcd,
                            p.lmtm,
                            p.lmur,
                            p.perd,
                            p.wkda,
                            p.tatcaldar.cdnm
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
