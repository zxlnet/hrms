using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Overtime
{
    public class otttlvDal : BaseDal
    {
        public List<totdetail> GetOTDetailForTTLV(List<ColumnInfo> _personalParameters,DateTime _frtm,DateTime _totm)
        {
            string sSql2 = BuildWhereSql(@"emp.sfid,emp.sfnm,emp.emno,emp.emst,emp.stcd,emp.dvcd,emp.bscd,
                                           emp.dpcd,emp.grcd,emp.pscd,emp.jccd,emp.jtcd", _personalParameters);

            var q = from p in gDB.totdetails
                    join s in gDB.tottypes on p.otcd equals s.otcd
                    join t in gDB.vw_employments.Where(sSql2) on p.emno equals t.emno
                    where ((t.tmdt.HasValue == false)
                    || (t.tmdt.Value.CompareTo(_frtm) >= 0))
                    && p.sttm >= _frtm
                    && p.edtm < _totm
                    && ((p.istr == "N") || (p.istr==""))
                    && ((s.ttlv.Trim() != "") && (s.ttlv != null))
                    select p;

            return q.ToList<totdetail>();
        }
    }
}
