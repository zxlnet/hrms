using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Utility;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSBusiness.Payroll.Variables
{
    public class sysWorkHours : ISysVariables
    {
        public object GetValue(PrCalculationStore store)
        {
            try
            {
                //用户变量,则直接抓去值
                object valu = null;
                string vari = this.GetType().Name;

                var q = store.lstVariables.Where(p => p.vacd == vari).ToList();

                if (q.Count > 0)
                {
                    tprvarble mdl = q.First();
                    valu = double.Parse(mdl.vlva);
                }
                else
                {
                    valu = 0;
                }

                return valu;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }
    }
}
