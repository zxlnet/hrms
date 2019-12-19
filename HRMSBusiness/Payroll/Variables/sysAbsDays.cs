﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysAbsDays : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            try
            {
                double result = 0;

                var q = store.lstAbsenceDetails.Where(p=>p.emno == store.CurrentEmployment.emno).Select(p => p.ahrs).Sum();
                result += q.HasValue ? q.Value : 0;

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
