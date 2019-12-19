using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class prfmularDal : BaseDal
    {
        public prfmularDal()
        {
        }

        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("frcd,frnm", _parameter);

                var q = from p in gDB.tprfmulars.Where(sSql1)
                        select new
                        {
                           p.dscr,
                           p.frcd,
                           p.frnm,
                           p.chfn,
                           p.lmtm,
                           p.lmur
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

        public List<tprfmudtl> GetFormulaDtl(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                return base.GetSelectedRecords<tprfmudtl>(_parameter, paging, start, num, ref totalRecordCount);
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
            finally
            {
            }
        }
    }
}
