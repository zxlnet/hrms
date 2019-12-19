using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class pschghisDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("from|lmtm,to|lmtm,lmur", _parameter);
                string sSql2 = BuildWhereSql("sfid,stfn,emno", _parameter);

                var q = from p in gDB.tpsemphis.Where(sSql1)
                        join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                        join s in gDB.tbschgtyps on p.ctcd equals s.ctcd
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
                            p.isby,
                            p.isdt,
                            p.ctcd,
                            s.ctnm,
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

    }
}