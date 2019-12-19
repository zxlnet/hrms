using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysLvType : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            store.ParametersForValue.Add(store.CurrentExpressionRight);
            return store.CurrentExpressionRight.Replace("\"","");
        }
    }
}
