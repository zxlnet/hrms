using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysMonthStart : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            return new DateTime(store.PeriodStart.Year, store.PeriodStart.Month, 1);
        }
    }
}
