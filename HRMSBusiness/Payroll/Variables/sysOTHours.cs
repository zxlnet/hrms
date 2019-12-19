using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Overtime;
using GotWell.Common;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysOTHours : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            return 0;
        }

        public double GetLimit(PrCalculationStore store, string _otcd)
        {
            otlimitDal limitDal = new otlimitDal();
            double monthLimit = limitDal.GetMonthlmbyEmp(store.CurrentEmployment, _otcd, HRMS_Limit_Type.OvertimeHours);

            if (monthLimit == null)
                monthLimit = 1000;
            if ((monthLimit == -1) || (monthLimit < 0))
                monthLimit = 10000;

            return monthLimit;
        }

    }
}
