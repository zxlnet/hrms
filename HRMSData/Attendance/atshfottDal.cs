﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atshfottDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("sfcd,from|lmtm,to|lmtm,lmur", _parameter);

                var q = from p in gDB.tatshfotts.Where(sSql1)
                        join o in gDB.tatshifts on p.sfcd equals o.sfcd
                        join s in gDB.tottypes on p.otcd equals s.otcd
                        select new
                        {
                            p.sfcd,
                            p.otst,
                            p.otcd,
                            p.lmtm,
                            p.lmur,
                            o.sfnm,
                            s.otnm,
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
