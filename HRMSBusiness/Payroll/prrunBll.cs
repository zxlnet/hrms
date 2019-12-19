using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using System.Transactions;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prrunBll : BaseBll
    {
        prrunDal dal = null;

        public prrunBll()
        {
            dal = new prrunDal();
            baseDal = dal;
        }

        public override void DoDelete<T>(List<ColumnInfo> _parameter)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete salary history
                    baseDal.DoMultiDelete<tprsalhi>(_parameter);

                    //delete bank allocate
                    baseDal.DoMultiDelete<tpraccalc>(_parameter);

                    //delete cost center allocate
                    baseDal.DoMultiDelete<tprcstalc>(_parameter);

                    //delete accumulation
                    baseDal.DoMultiDelete<tprcumitm>(_parameter);

                    //delete run
                    baseDal.DoMultiDelete<tprrun>(_parameter);

                    scope.Complete();
                }
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
