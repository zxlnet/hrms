using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public interface ISysVariables
    {
        object GetValue(PrCalculationStore store);
    }
}
