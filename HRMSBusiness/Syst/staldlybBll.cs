using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Syst;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.HRMS;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class staldlybBll : BaseBll
    {
        staldlyDal dal = null;

        public staldlybBll()
        {
            dal = new staldlyDal();
            baseDal = dal;
        }

        public void HandleMesssage(string adid)
        {
            try
            {
                List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "adid", ColumnValue = adid } };
                tstaldlyb obj = GetSelectedObject<tstaldlyb>(parameters);

                if (obj != null)
                {
                    obj.alst = "Handled";
                }

                DoUpdate<tstaldlyb>(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
