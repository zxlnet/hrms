using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;

namespace GotWell.HRMS.HRMSData.Syst
{
    public class stpubmsgDal : BaseDal
    {
        public string GetNewSMID()
        {
            string newSMID = string.Empty;
            try
            {
                var q = (from p in gDB.tstpubmsgs
                         where p.smid.Contains(DateTime.Now.ToString("yyyyMM"))
                         select p.smid).Max();

                if (q == null)
                    newSMID = DateTime.Now.ToString("yyyyMM") + "0001";
                else
                    newSMID = (Convert.ToDouble(q) + 1).ToString();
            }
            catch (Exception ex)
            {
                newSMID = "Error";
            }

            return newSMID;
        }
    }
}
