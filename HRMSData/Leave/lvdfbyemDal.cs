using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Leave
{
    public class lvdfbyempDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("ltcd,from|exdt,to|exdt,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tlvdfbyems.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tlvleatyps on p.ltcd equals s.ltcd
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            p.lmtm,
                            p.lmur,
                            p.days,
                            p.exdt,
                            p.ltcd,
                            p.remk,
                            p.sqno,
                            s.ltnm,
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

        public double getLeaveSettingsByEmp(string _emno,string _ltcd,DateTime _endDate)
        {
            try
            {
                var q = (from p in gDB.tlvdfbyems
                         where p.emno == _emno
                         && p.ltcd == _ltcd
                         && p.exdt >= _endDate
                         select p.days).ToList();

                double r = 0;

                if (q.Count > 0)
                {
                    r = Convert.ToDouble(q.Sum());
                }

                return r;
            }
            catch(UtilException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new UtilException(ex.Message,ex);
            }
        }
    }
}
