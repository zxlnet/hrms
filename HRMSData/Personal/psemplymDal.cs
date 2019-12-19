using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class psemplymDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsemplyms.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            p.ptcd,
                            p.cccd,
                            p.pscd,
                            p.plcd,
                            p.jccd,
                            p.stcd,
                            p.dvcd,
                            p.sccd,
                            p.grcd,
                            p.dpcd,
                            p.ttcd,
                            p.bscd,
                            p.jtcd,
                            p.wscd,
                            p.wgcd,
                            p.glcd,
                            p.clcd,
                            p.trcd,
                            p.rscd,
                            p.jidt,
                            p.prdt,
                            p.tmdt,
                            p.emst,
                            p.titl,
                            p.rsid,
                            p.assc,
                            p.rtdt,
                            p.ssdt,
                            p.reto,
                            p.isrh,
                            p.isbl,
                            p.tmrm,
                            p.remk,
                            p.lmtm,
                            p.lmur,
                            t.elnm,
                            t.scnm,
                            t.bsnm,
                            t.psnm,
                            t.stnm,
                            t.jcnm,
                            t.jtnm,
                            t.dvnm,
                            t.dpnm,
                            t.rsnm,
                            t.arcd,
                            t.arnm,
                            t.ptnm,
                            t.wsnm,
                            t.grnm,
                            t.wgnm,
                            t.ccnm,
                            t.conm,
                            t.trnm,
                            t.frnm,
                            t.elcd,
                            t.ttnm,
                            t.salu,
                            t.napl,
                            t.ennm,
                            t.cdcd,
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

        public List<object> GetSelectedRecordsForAdvQry(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("sfid,stfn,emst,stcd,dvcd,bscd,dpcd,grcd,pscd,jccd,jtcd", _parameter);

                var q = from p in gDB.vw_employments.Where(sSql1)
                        select new
                        {
                            isselected = 'Y',
                            stfn = p.ntnm,
                            p.emno,
                            p.sfid,
                            p.dvcd,
                            p.dvnm,
                            p.bscd,
                            p.bsnm,
                            p.dpcd,
                            p.dpnm,
                            p.grcd,
                            p.grnm,
                            p.pscd,
                            p.psnm,
                            p.jccd,
                            p.jcnm,
                            p.jtcd,
                            p.jtnm
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

        public List<vw_employment> GetHiringEmployment(List<ColumnInfo> _parameter, DateTime _startDate)
        {
            try
            {
                string sSql1 = BuildWhereSql(@"emp.emno,emp.sfid,emp.stfn,emp.emst,emp.stcd,emp.dvcd,emp.bscd,
                                                emp.dpcd,emp.grcd,emp.pscd,emp.jccd,emp.jtcd", _parameter);

                sSql1 = sSql1.Replace("emp.", "");
                var q = from p in gDB.vw_employments.Where(sSql1)
                        where ((p.tmdt.HasValue==false)
                        || (p.tmdt.Value.CompareTo(_startDate)>=0))
                        select p;

                List<vw_employment> obj = q.Cast<vw_employment>().ToList();

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

        public List<vw_employment> GetEmploymentForAllocation(List<ColumnInfo> _parameter)
        {
            try
            {
                string sSql1 = BuildWhereSql(@"emp.emno,emp.sfid,emp.stfn,emp.emst,emp.stcd,emp.dvcd,emp.bscd,
                                                emp.dpcd,emp.grcd,emp.pscd,emp.jccd,emp.jtcd", _parameter);

                sSql1 = sSql1.Replace("emp.", "");
                var q = from p in gDB.vw_employments.Where(sSql1)
                        select p;

                List<vw_employment> obj = q.Cast<vw_employment>().ToList();

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

        public string AnalyzeQueryCriterias(List<ColumnInfo> _parameter, DateTime _startDate)
        {
            try
            {
                string sSql = string.Empty;

                List<ColumnInfo> q = (from p in _parameter
                        where p.ColumnValue.Trim() != string.Empty
                        select p).ToList<ColumnInfo>();

                for (int i = 0; i < q.Count; i++)
                {
                    string sTemp = string.Empty;
                    switch(q[i].ColumnType)
                    {
                        case "string":
                            sTemp ="(" + q[i].ColumnName + " like '%" + q[i].ColumnValue + "%')";
                            break;
                        case "datetime":
                            string[] arr = q[i].ColumnName.Split('|');
                            if (arr[0]=="from")
                            {
                                sTemp ="(" + q[i].ColumnName + ">='" + q[i].ColumnValue + "')";
                            }

                            if (arr[0]=="to")
                            {
                                sTemp ="(" + q[i].ColumnName + "<'" + Convert.ToDateTime(q[i].ColumnValue).AddDays(1).ToString("Y-m-d") + "')";
                            }
                            break;
                    }
                    sSql = sSql == string.Empty ? sTemp : " and " + sTemp;
                }

                return sSql;
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

        public List<vw_employment> GetValidEmployment()
        {
            var q = from p in gDB.vw_employments
                    where ((p.tmdt.HasValue == false)
                        || (p.tmdt.Value.CompareTo(DateTime.Now) >= 0))
                    select p;

            return q.ToList<vw_employment>();
        }
    }
}