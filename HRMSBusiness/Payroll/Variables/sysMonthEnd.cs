using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysMonthEnd : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            return new DateTime(store.PeriodStart.Year, store.PeriodStart.Month, 1).AddMonths(1).AddDays(-1);
        }
    }
}
