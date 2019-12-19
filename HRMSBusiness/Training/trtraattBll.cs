using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Training;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Common;
using System.Transactions;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Training
{
    public class trtraattBll : BaseBll
    {
        trtraattDal dal = null;

        public trtraattBll()
        {
            dal = new trtraattDal();
            baseDal = dal;
        }

        public void Register(string trcd, string emno)
        {
            try
            {
                List<ColumnInfo> lstParams = new List<ColumnInfo>()
                {
                    new ColumnInfo(){ColumnName="trcd",ColumnValue=trcd}
                };

                ttrtraing training = GetSelectedObject<ttrtraing>(lstParams);

                lstParams.Add(new ColumnInfo() { ColumnName = "emno", ColumnValue = emno });

                ttrtraatt att = GetSelectedObject<ttrtraatt>(lstParams);
                if (att == null) return;

                if (training.endt != null && training.endt < DateTime.Now)
                {
                    throw new UtilException("The training is out-of-date.");
                }
                
                if (att.isrg == "Y")
                {
                    throw new UtilException("You have registered the training.Don't need register again.");
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    att.isrg = "Y";
                    att.rgtm = DateTime.Now;
                    att.rgur = Function.GetCurrentUser();

                    DoUpdate<ttrtraatt>(att, lstParams);

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
