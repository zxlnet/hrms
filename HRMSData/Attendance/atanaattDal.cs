using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSData.Attendance
{
    public class atanaattDal : BaseDal
    {
        public atanaattDal()
        {
        }

        public List<object> AnalyzeAttendance(List<ColumnInfo> _atdtParameters,
                                    List<ColumnInfo> _personalParameters)
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
    }
}
