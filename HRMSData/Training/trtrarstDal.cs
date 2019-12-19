using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSData.Training
{
    public class trtrarstDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("trcd,trnm,from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.ttrtrarsts.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            p.athr,
                            p.isce,
                            p.ispa,
                            p.issk,
                            p.remk,
                            p.scor,
                            p.trcd,
                            p.lmtm,
                            p.lmur,
                            p.rfid,
                            p.ttrtraing.trnm,
                            stcd=p.ttrtraing.skty,
                            ifce=p.ttrtraing.isce,
                            ifsk=p.ttrtraing.issk
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

        public  List<object> GetTrainingResult(List<ColumnInfo> _parameter)
        {
            try
            {
                string sSql1 = BuildWhereSql("trcd", _parameter);

                var q = from p in gDB.ttrtraatts.Where(sSql1)
                        join t in gDB.vw_employments on p.emno equals t.emno
                        join o in gDB.ttrtrarsts on new { p.trcd,t.emno } equals new { o.trcd,o.emno } into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            p.emno,
                            t.sfid,
                            stfn = t.ntnm,
                            c1.athr,
                            c1.isce,
                            c1.ispa,
                            c1.issk,
                            c1.remk,
                            c1.scor,
                            p.trcd,
                            c1.lmtm,
                            c1.lmur,
                            c1.rfid,
                            p.ttrtraing.trnm
                        };

                List<object> lstObj = q.Cast<object>().ToList();


                return lstObj;

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

        public List<object> GetSkillItems(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("stcd", _parameter);
                string sSql2 = BuildWhereSql("trcd,emno", _parameter);

                var q = from p in gDB.tbssklitms.Where(sSql1)
                        join o in gDB.ttrtraskis.Where(sSql2)
                        on p.sicd equals o.sicd into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            sicd = c1.sicd == null ? p.sicd : c1.sicd,
                            racd = c1.racd == null ? p.racd : c1.racd,
                            p.stcd,
                            p.sinm,
                            p.tbsrating.ranm,
                            p.tbsskltyp.stnm,
                            c1.trcd,
                            c1.emno,
                            c1.remk,
                            c1.rtcd,
                            c1.lmtm,
                            c1.lmur,
                            c1.tbsratdtl.rtnm
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
    }
}

