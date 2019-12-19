using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysMonthDays:ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            double result = 0;
            result = Convert.ToDouble((store.PeriodEnd - store.PeriodStart).TotalDays + 1);
            return result;
        }
    }
}
