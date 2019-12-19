using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using System.Linq.Dynamic;
using GotWell.Utility;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSData.Recruitment
{
    public class rcaplinfDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {

                string sSql1 = BuildWhereSql("apno,jbcd,jbnm,dpcd,cnnm,ennm", _parameter);

                var q = from p in gDB.trcaplinfs.Where(sSql1)
                        select new
                        {
                           p.aino,p.apdt,p.apno,p.brdt,p.cnnm,p.doav,p.emad,p.emal,
                           p.emnm,p.emrl,p.emtp,p.ennm,p.exsa,p.hght,p.hmad,p.hmpc,
                           p.hmtp,p.jbcd,p.jbnm,p.jbty,p.lmtm,p.lmur,p.mast,p.mobi,p.oth1,
                           p.oth2,p.oth3,p.oth4,p.otr1,p.otr2,p.otr3,p.otr4,p.remk,p.rfid,
                           p.skll,p.stus,p.wght,p.wktp,p.sex,p.inho
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
