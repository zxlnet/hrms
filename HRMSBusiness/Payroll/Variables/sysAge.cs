using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysAge : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            return store.CurrentEmployment.age.HasValue ? store.CurrentEmployment.age.Value : 0;
        }
    }
}
