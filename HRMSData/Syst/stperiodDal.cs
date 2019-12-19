using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using GotWell.Common;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSData.Syst
{
    public class stperiodDal : BaseDal
    {
        public stperiodDal()
        {

        }

        public tstperiod GetPreviousPeriod(tstperiod _periodMdl)
        {
            try
            {

                DateTime dt = (new DateTime(Int16.Parse(_periodMdl.year), Int16.Parse(_periodMdl.perd), 1)).AddMonths(-1);

                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="period",ColumnValue= (dt.Year.ToString()+ dt.Month.ToString().PadLeft(2, '0'))}
                    };

                tstperiod prevMdl = GetSelectedObject<tstperiod>(parameters);

                return prevMdl;
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