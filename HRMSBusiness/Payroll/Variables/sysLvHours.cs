using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.Common;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysLvHours : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            try
            {
                double result = 0;
                for (int i = 0; i < store.ParametersForValue.Count; i++)
                {
                    var ltcd = store.ParametersForValue[i].ToString().Replace("\"", "");
                    var q = store.lstLeaveDetails.Where(p => p.ltcd == store.ParametersForValue[i].ToString() && p.emno == store.CurrentEmployment.emno).Select(p => p.hurs).Sum();
                    double limit = GetLimit(store, store.ParametersForValue[i].ToString());

                    result += (q <= limit ? q : limit);
                }

                if (store.ParametersForValue.Count == 0)
                {
                    var q = store.lstLeaveDetails.Where(p => p.emno == store.CurrentEmployment.emno).Select(p => p.hurs).Sum();
                    double limit = GetLimit(store, "All");
                    result = (q <= limit ? q : limit);
                }

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public double GetLimit(PrCalculationStore store,string _ltcd)
        {
            lvlealmtDal limitDal = new lvlealmtDal();
            double monthLimit = limitDal.GetMonthlmbyEmp(store.CurrentEmployment, _ltcd, HRMS_Limit_Type.LeaveHours);

            if (monthLimit == null)
                monthLimit = 1000;
            if ((monthLimit == -1) || (monthLimit < 0))
                monthLimit = 10000;

            return monthLimit;
        }
    }
}
