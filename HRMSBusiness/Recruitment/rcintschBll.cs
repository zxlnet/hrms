using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Recruitment;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Model.Common;
using GotWell.Common;

namespace GotWell.HRMS.HRMSBusiness.Recruitment
{
    public class rcintschBll : BaseBll
    {
        rcintschDal localDal = null;
        public rcintschBll()
        {
            localDal = new rcintschDal();
            baseDal = localDal;
        }

        public void CreateInterviewSchedule(List<trcintsch> listDtl)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < listDtl.Count; i++)
                    {
                        listDtl[i].isno = Function.GetGUID();
                        DoInsert<trcintsch>(listDtl[i]);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateInterviewSchedule(List<ColumnInfo> parameters ,List<trcintsch> listDtl)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    localDal.DeleteInterviewSchedule(parameters);

                    for (int i = 0; i < listDtl.Count; i++)
                    {
                        listDtl[i].isno = Function.GetGUID();
                        DoInsert<trcintsch>(listDtl[i]);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void DoDelete<T>(List<ColumnInfo> _parameter)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    localDal.DeleteInterviewSchedule(_parameter);
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
