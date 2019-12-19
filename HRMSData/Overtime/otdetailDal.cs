using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using GotWell.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Overtime
{
    public class otdetailDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("otcd,from|otdt,to|otdt,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.totdetails.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tlvleatyps on p.ttlv equals s.ltcd into t1
                        from cp in t1.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            p.lmtm,
                            p.lmur,
                            p.edtm,
                            p.otdt,
                            p.othr,
                            p.othm,
                            p.oths,
                            p.otcd,
                            p.sttm,
                            p.istr,
                            p.tlhr,
                            p.tlrf,
                            p.ttlv,
                            p.tottype.otnm,
                            cp.ltnm
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

        public int DeleteDummyData(string _sStaffSql, DateTime _startDate, DateTime _endDate)
        {
            string sSql = @"delete from totdetail from totdetail a where 
                           (isnull(a.othm,0) <= 0)
                           and exists (select b.atdt,b.emno,b.iscf from 
                           tatanarst b inner join vw_employment emp 
                           on b.emno=emp.emno and " + _sStaffSql + @"
                           where b.emno=a.emno and b.atdt=a.otdt 
                           and (b.iscf is null or b.iscf='N')  and 
                           b.atdt >='" + UtilDatetime.FormatDate1(_startDate) + @"' 
                           and b.atdt<'" + UtilDatetime.FormatDate1(_endDate) + "')";

            object[] parameters = { };
            return gDB.ExecuteCommand(sSql, parameters);
        }

        public List<totdetail> GetOTDetailForPayroll(string _sStaffSql,DateTime _startDate,DateTime _endDate)
        {
            //var q = from p in gDB.totdetails
            //        join t in _lstStaff on p.emno equals t.emno
            //        where p.otdt >= _startDate
            //        && p.otdt < _endDate
            //        select p;
            string sSql = "select a.* from totdetail a,vw_employment emp where a.emno = emp.emno " +
                           (_sStaffSql.Trim() == string.Empty ? "" : " and " + _sStaffSql) +
                        " and a.otdt >='" + UtilDatetime.FormateDateTime1(_startDate) + "' " +
                        " and a.otdt <'" + UtilDatetime.FormateDateTime1(_endDate) + "' ";
            
            IEnumerable<totdetail> ret = gDB.ExecuteQuery<totdetail>(sSql);
            return ret.ToList();
        }

        public void SaveOTDetail(string _emno,string _ottype,DateTime _otdt,
                                  DateTime _sttm,DateTime _endtime,
                                  double _othr, double _othrr, double _othm)
        {
            List<ColumnInfo> parameters = new List<ColumnInfo>(){new ColumnInfo(){ColumnName="emno",ColumnValue=_emno},
                                            new ColumnInfo(){ColumnName="otdt",ColumnValue=UtilDatetime.FormatDate1(_otdt),ColumnType="datetime"},
                                            new ColumnInfo(){ColumnName="sttm",ColumnValue=UtilDatetime.FormateDateTime1(_sttm),ColumnType="datetime"},};

            List<totdetail> list = new BaseDal().GetSelectedRecords<totdetail>(parameters);


            if (list.Count <= 0)
            {
                totdetail otdetail = new totdetail();
                otdetail.emno = _emno;
                otdetail.edtm = _endtime;
                otdetail.otdt = _otdt;
                otdetail.oths = _othr;
                otdetail.othm = _othm;
                otdetail.othr = _othrr;
                otdetail.otcd = _ottype;
                otdetail.sttm = _sttm;
                otdetail.lmtm = DateTime.Now;
                otdetail.lmur = Function.GetCurrentUser();

                gDB.totdetails.InsertOnSubmit(otdetail);
            }
            else
            {
                totdetail otdetail = list.First();
                otdetail.edtm = _endtime;
                otdetail.otcd = _ottype;
                otdetail.othr = _othr;
                otdetail.othr = _othrr;
                otdetail.lmtm = DateTime.Now;
                otdetail.lmur = Function.GetCurrentUser();
            }

            gDB.SubmitChanges();
        }

    }
}