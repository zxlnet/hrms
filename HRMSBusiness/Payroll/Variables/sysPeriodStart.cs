using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysPeriodStart : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            return store.PeriodStart; ;
        }
    }
}
