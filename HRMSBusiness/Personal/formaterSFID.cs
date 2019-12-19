using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.Common;
using GotWell.Common;
using System.Text.RegularExpressions;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class formaterSFID
    {
        public string GetEmnoFormater()
        {
            StSystemConfig config = Parameter.CURRENT_SYSTEM_CONFIG as StSystemConfig;

            if (config.PsEMNOF.Equals(string.Empty))
                return "N(6)";

            return config.PsEMNOF;
        }

        public string Parse(string sfid, string formater)
        {
            string retval = string.Empty;

            /*Format 1: N(6)*/
            Regex reg = new Regex(@"N\([0-9]\)");
            if (reg.IsMatch(formater))
                retval = ParseFormat1(sfid, formater);

            /*Other formaters*/
            return retval;
        }

        public string ParseFormat1(string sfid, string formater)
        {
            try
            {
                string retval = string.Empty;
                int len = Int16.Parse(formater.Replace("N", "").Replace("(", "").Replace(")", ""));

                if (sfid.Trim() == string.Empty)
                    retval = "1".PadLeft(len, '0');
                else
                    retval = (Int64.Parse(sfid) + 1).ToString().PadLeft(len, '0');

                return retval;
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }
    }
}
