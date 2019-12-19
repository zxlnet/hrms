using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysIsActive : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            bool result = false;

            if (store.CurrentEmployment.tmdt.HasValue)
            {
                if (store.CurrentEmployment.tmdt > store.PeriodEnd)
                    result = true;
                else
                    result = false;
            }
            else
                result = true;

            return result;
        }
    }
}
