using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Utility;
using GotWell.Model.Common;
using System.Linq.Dynamic;
using GotWell.Common;

namespace GotWell.HRMS.HRMSData.Personal
{
    public class pspersonDal : BaseDal
    {
        public pspersonDal()
        {
        }

        public string GetMaxemno()
        {
            try
            {
                var v = gDB.tpspersons.Max(p => p.emno);
                return v;
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

        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("emno,sfid,stfn,ennm,frnm,brpl,from|lmtm,to|lmtm,lmur", _parameter);

                var q = from p in gDB.tpspersons.Where(sSql1)
                        join z in gDB.vw_employments on p.emno equals z.emno
                        join s in gDB.tbscontries on p.cocd equals s.cocd into t1
                        from c1 in t1.DefaultIfEmpty()
                        join t in gDB.tbscontries on p.occd equals t.cocd into t2
                        from c2 in t1.DefaultIfEmpty()
                        join o in gDB.tbsareas on p.arcd equals o.arcd into t3
                        from c3 in t3.DefaultIfEmpty()
                        join r in gDB.tbsrligons on p.rgcd equals r.rgcd into t4
                        from c4 in t4.DefaultIfEmpty()
                        join v in gDB.tbsedulevs on p.elcd equals v.elcd into t5
                        from c5 in t5.DefaultIfEmpty()
                        join w in gDB.tbsnations on p.nacd equals w.nacd into t6
                        from c6 in t6.DefaultIfEmpty()
                        select new
                        {
                            p.arcd,p.blty,p.brdt,p.brpl,p.cocd,p.deds,p.dred,p.drno,
                            p.drty,p.elcd,p.emno,p.ennm,p.frnm,p.hfty,p.hght,p.idno,
                            p.idsd,p.isdi,p.itst,p.lmtm,p.lmur,p.madt,p.mast,p.nacd,
                            p.napl,p.nknm,p.ntnm,p.otnm,p.plcd,p.pped,p.ppid,p.ppip,
                            p.ppno,p.remk,p.rgcd,p.salu,p.sex,p.sfid,p.spec,p.sunm,
                            p.visi,p.wght,p.wped,p.wpno,c1.conm,ocnm=c2.conm,c3.arnm,
                            c4.rgnm,
                            c5.elnm,
                            c6.nanm,
                            p.tstrecst.stus,
                            p.rfid,
                            z.emst,p.ided
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

        public void GetAutoStaffId(ref string emno,ref string sfid)
        {
            try
            {
                emno = gDB.tpspersons.Select(p => p.emno).Max();
                sfid = gDB.tpspersons.Select(p => p.sfid).Max();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
