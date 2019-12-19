using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Common;

namespace GotWell.HRMS.HRMSData.Training
{
    public class trtraingDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("trcd,trnm,cunm,orga,ttcd,incm,from|lmtm,to|lmtm,lmur", _parameter);

                var q = from p in gDB.ttrtraings.Where(sSql1)
                        join t in gDB.tbstratyps on p.ttcd equals t.ttcd into t1
                        from c1 in t1.DefaultIfEmpty()
                        select new
                        {
                            p.trcd,p.trnm,p.crcd,p.ttcd,
                            p.orga,p.tchr,p.stdt,p.endt,
                            p.redt,p.cunm,p.natu,p.loca,
                            p.ctpw,p.tttm,p.paby,p.cufe,
                            p.bnag,p.tmpe,p.tget,p.remk,
                            p.incm,p.isce,p.cenm,p.isnm,
                            p.ceid,p.ceed,p.cexd,p.ctcd,
                            p.issk,p.skty,p.sknm,p.skle,
                            p.ispb,p.lmtm,p.lmur,p.rfid,
                            c1.ttnm
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