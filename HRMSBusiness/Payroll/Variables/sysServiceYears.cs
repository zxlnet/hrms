using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysServiceYears : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            double d = store.CurrentEmployment.yearservice.HasValue ? store.CurrentEmployment.yearservice.Value : 0; 
            if (d == null)
                d = 0;

            if (d < 0)
                d = 0;

            return d;
        }
    }
}
