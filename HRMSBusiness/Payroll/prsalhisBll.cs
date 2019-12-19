using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using GotWell.Model.Common;
using System.Transactions;
using GotWell.Model.HRMS;
using GotWell.Common;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prsalhisBll : BaseBll
    {
        prsalhisDal dal = null;

        public prsalhisBll()
        {
            dal = new prsalhisDal();
            baseDal = dal;
        }

        public void UpdateAdjustedValue(List<ColumnInfo> lstParameters, double adjustedValue)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    tprsalhi his = GetSelectedObject<tprsalhi>(lstParameters);

                    if (his != null)
                    {
                        if (his.ajva != adjustedValue)
                        {
                            his.ajva = adjustedValue;
                            his.lmtm = DateTime.Now;
                            his.lmur = Function.GetCurrentUser();

                            DoUpdate<tprsalhi>(his,lstParameters);
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
