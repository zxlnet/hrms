using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysIsInProbation : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            bool result = false;
            if (!store.CurrentEmployment.prdt.HasValue) result = false;

            //判断加入日期和期末是否在同一个月，如果在，则认为true，否则则为false
            if (store.CurrentEmployment.prdt.Value.Month == store.PeriodEnd.Month)
                result = true;
            else
                result = false;

            return result;
        }
    }
}
