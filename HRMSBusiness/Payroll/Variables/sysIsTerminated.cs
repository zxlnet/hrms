using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysIsTerminated : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            bool result = false;

            if (store.CurrentEmployment.tmdt.HasValue)
            {
                if (store.CurrentEmployment.tmdt > store.PeriodEnd)
                    result = false;
                else
                    result = true;
            }
            else
                result = false;

            return result;
        }
    }
}
