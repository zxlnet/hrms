using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Leave
{
    public class lvdfbyyearsDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|lmtm,to|lmtm,lmur", _parameter);

                var q = from p in gDB.tlvdfbyyrs.Where(sSql1)
                        join s in gDB.tlvleatyps on p.ltcd equals s.ltcd
                        select new
                        {
                            p.ltcd,
                            s.ltnm
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

                var q = from p in gDB.tlvdfbyyrs
                        where p.ltcd.Equals(GetColumnValue("ltcd", _parameter))
                        orderby p.sqno
                        select new
                        {
                            p.days,
                            p.fryr,
                            p.lmtm,
                            p.lmur,
                            p.ltcd,
                            p.remk,
                            p.sqno,
                            p.toyr
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

        public double getLeaveSettingsByYear(string _ltcd,double _yearService)
        {
            try
            {
                double r = 0;

                var q = from p in gDB.tlvdfbyyrs
                        where p.ltcd == _ltcd
                        select p;

                List<double> q1 = (from p in q
                                   where p.fryr <= _yearService
                                  && p.toyr >= (_yearService - 1)
                                   select p.days).ToList<double>();

                for (int i = 0; i < q1.Count; i++)
                {
                    r = q1[i];
                }

                return r;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message,ex);
            }
        }
    }
}
