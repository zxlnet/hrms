using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class prpubrstDal : BaseDal
    {
        public List<object> GetPubRuleSetDtl(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("rscd,rsnm", _parameter);

                var q = from p in gDB.tprprsdtls.Where(sSql1)
                        select new
                        {
                           p.crcd,p.cred,p.efdt,p.exdt,p.isca,p.itcd,p.lmtm,p.lmur,
                           p.pdcd,p.rlcd,p.rscd,p.sqno,p.ssty,p.valu,p.vtyp,
                           p.tbscurncy.crnm,padt=p.tprpayday==null?-1:p.tprpayday.padt,p.tprpubrst.rsnm,
                           p.tprsalitm.itnm
                        };

                List<object> obj = q.Cast<object>().ToList().Skip(start).Take(num).ToList();

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

        public List<tprprsdtl> GetPubRuleSetDtlForPayroll(DateTime _startDate, DateTime _endDate,string _payDay)
        {
            var  q=from p in gDB.tprprsdtls
                    where (p.efdt < _endDate)
                    && ((p.exdt.HasValue == false) || (p.exdt >= _endDate))
                    select p;

            if (_payDay.Equals(string.Empty))
                //all
                return q.ToList();

            //pay day assigned
            return q.Where(p => p.pdcd == _payDay).ToList();
        }

        public List<object> GetRelItem(string rlcd,string itcd)
        {
            try
            {
                var q = from p in gDB.tprsalitms
                        join t in gDB.tprrstsits.Where(s=>s.rlcd==rlcd) on p.itcd equals t.itcd into t1
                        from c1 in t1.DefaultIfEmpty()
                        where p.itcd != itcd
                        orderby (c1.rlcd == null ? "N" : "Y") descending,p.itcd
                        select new
                        {
                            sele = (c1.rlcd == null ? "N" : "Y"),
                            c1.rlcd,
                            p.itcd,
                            c1.lmtm,
                            c1.lmur,
                            p.itnm
                        };

                List<object> obj = q.Cast<object>().ToList();

                return obj;

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
