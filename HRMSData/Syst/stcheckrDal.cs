using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Common;
using System.Data;

namespace GotWell.HRMS.HRMSData.Syst
{
    public class stcheckrDal : BaseDal
    {
        private UtilLog log = null;

        public stcheckrDal()
        {
        }

         #region GetStep01Data
        /// <summary>
        /// Check User's Roster History
        /// </summary>
        /// <returns></returns>
        public DataSet GetStep01Data()
        {
            try
            {
                return null;
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
        #endregion


    }
}