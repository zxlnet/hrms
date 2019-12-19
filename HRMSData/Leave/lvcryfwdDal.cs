using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using GotWell.Model.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Leave
{
    public class lvcryfwdDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("year,ltcd,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tlvcryfwds.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tlvleatyps on p.ltcd equals s.ltcd
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            p.lmtm,
                            p.lmur,
                            p.ltcd,
                            p.daro,
                            p.days,
                            p.hrcs,
                            p.year,
                            p.hors,
                            s.ltnm,
                            p.limt,
                            p.enti,
                            p.cnsu,
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

        public double getCarryDaysByEmp(vw_employment empInfo, string _ltcd,double _year)
        {
            try
            {
                if (empInfo == null) return 0;

                var q = from p in gDB.tlvcryfwds
                        where p.emno == empInfo.emno
                        && p.year == (_year-1).ToString()
                        && p.ltcd == _ltcd
                        select new { days = p.days };

                if (q == null) return 0;
                if (q.ToList().Count < 1) return 0;

                return q.ToList().Single().days.Value;

                //return q.
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
