using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;

namespace GotWell.HRMS.HRMSData.Payroll
{
    public class prrunDal : BaseDal
    {
        public string GetNewRun(DateTime dt)
        {
            string runno = string.Empty;
            try
            {
                var q = (from p in gDB.tprruns
                         where p.rndt.Value == dt
                         select p.rnno).Max();

                if (q == null)
                    runno = dt.ToString("yyyyMM") + "001";
                else
                    runno = (Convert.ToDouble(runno) + 1).ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return runno;
        }
    }
}
