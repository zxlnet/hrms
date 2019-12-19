using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysAtDays : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            //需要考虑本月新入职员工/和本月离职员工
            return 0;
        }
    }
}
