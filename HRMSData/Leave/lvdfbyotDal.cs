using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.LanguageResources;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Leave
{
    public class lvdfbyotDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("ltcd", _parameter);

                var q = from p in gDB.tlvdfbyots.Where(sSql1)
                        join s in gDB.tlvleatyps on p.ltcd equals s.ltcd
                        join o in gDB.tstdefcfgs on p.dfby equals o.dfnm
                        select new
                        {
                            p.dfby,
                            dfbytext=HRMSRes.ResourceManager.GetString(o.dfrs),
                            p.dfno,
                            p.dfva,
                            p.dftx,
                            p.lmtm,
                            p.lmur,
                            p.ltcd,
                            p.remk,
                            s.ltnm,
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

        public List<object> GetDefDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                var q = from p in gDB.tlvdfbyods
                        where p.dfno.Equals(GetColumnValue("dfno", _parameter))
                        orderby p.sqno
                        select new
                        {
                            p.days,
                            p.fryr,
                            p.remk,
                            p.sqno,
                            p.toyr,
                            p.dfno
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

        public List<tlvdfbyod> getLeaveSettingsByOther(vw_employment empInfo, string _ltcd)
        {
            try
            {
                if (empInfo==null) return null;

                var q = from p in gDB.tlvdfbyots
                        join s in gDB.tstdefcfgs on p.dfby equals s.dfnm
                        join o in gDB.tlvdfbyods on p.dfno equals o.dfno
                        //where p.dfva == empInfo.GetType().GetProperty(s.fieldname).GetValue(empInfo, null).ToString()
                        select o;

                return q.ToList();
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
