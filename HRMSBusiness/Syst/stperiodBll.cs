using System;
using System.Collections.Generic;
using System.Linq;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Syst;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class stperiodBll: BaseBll
    {
        stperiodDal localDal = null;

        public stperiodBll()
        {
            localDal = new stperiodDal();
            baseDal = localDal;
        }
 
        #region Update Period
        public Exception_ErrorMessage UpdatePeriod(tstperiod _periodMdl)
        {
            try
            {
                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="perd",ColumnValue= _periodMdl.perd}
                    };

                DoUpdate<tstperiod>(_periodMdl, parameters);

                return Exception_ErrorMessage.NoError;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
            finally
            {
            }
        }

        #endregion


        #region Get Periods
        public tstperiod GetPeriod(string _period)
        {
            try
            {
                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="perd",ColumnValue= _period}
                    };

                return GetSelectedObject<tstperiod>(parameters);
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

        public List<tstperiod> GetPeriodByStatus(string _status)
        {
            try
            {
                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="psts",ColumnValue= _status}
                    };

                return GetSelectedRecords<tstperiod>(parameters).ToList<tstperiod>();
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


        #region GetPeriodByStatus
        public List<tstperiod> GetPeriodWithoutClosed()
        {
            try
            {
                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="pest",ColumnValue= HRMS_Period_Status.Open.ToString()}
                    };

                return GetSelectedRecords<tstperiod>(parameters).ToList<tstperiod>();
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

         public List<tstperiod> GetAllPeriods()
        {
            try
            {
                List<ColumnInfo> parameters = new List<ColumnInfo>() { };

                return GetSelectedRecords<tstperiod>(parameters).ToList<tstperiod>();
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

         #region GetYearList
         public List<string> GetYearList()
         {
             try
             {
                 //return localDal.GetYearList();
                 List<ColumnInfo> parameters = new List<ColumnInfo>() { };

                 List<tstperiod> periodList = GetSelectedRecords<tstperiod>(parameters);

                 var yearList = (from p in periodList
                                select p.year).Distinct<string>().ToList<string>();

                 return yearList;
             }
             catch (Exception ex)
             {
                 throw new UtilException(ex.Message, ex);
             }
         }
         #endregion

        public Exception_ErrorMessage ClosePeriod(tstperiod _periodMdl)
        {
            try
            {

                _periodMdl.csby = Function.GetCurrentUser();
                _periodMdl.cstm = DateTime.Now;
                _periodMdl.psts = HRMS_Period_Status.Closed.ToString();
                _periodMdl.remk = " [Closed by " + _periodMdl.csby + " at " + UtilDatetime.FormateDateTime1(_periodMdl.cstm) + "]";

                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="perd",ColumnValue= _periodMdl.perd}
                    };

                DoUpdate<tstperiod>(_periodMdl, parameters);

                return Exception_ErrorMessage.NoError;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
            finally
            {
            }

        }

        public Exception_ErrorMessage OpenPeriod(tstperiod _periodMdl)
        {
            try
            {
                //check before open
                List<tstperiod> lstPeriod = GetPeriodByStatus(HRMS_Period_Status.Open.ToString());


                if (lstPeriod.Count>0)
                {
                    throw new UtilException(lstPeriod[0].perd, Exception_ErrorMessage.OpenPeriodFoundOpenDenied, null);
                }

                _periodMdl.csby = Function.GetCurrentUser();
                _periodMdl.cstm = DateTime.Now;
                _periodMdl.psts = HRMS_Period_Status.Open.ToString();
                _periodMdl.remk = " [Open by " + _periodMdl.csby + " at " + UtilDatetime.FormateDateTime1(_periodMdl.cstm) + "]";

                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="perd",ColumnValue= _periodMdl.perd}
                    };

                DoUpdate<tstperiod>(_periodMdl, parameters);

                return Exception_ErrorMessage.NoError;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
            finally
            {
            }

        }

        public void CheckSelectedPeriodStatus(tstperiod periodMdl)
        {
            HRMS_Period_Status value = (HRMS_Period_Status)Enum.Parse(typeof(HRMS_Period_Status), periodMdl.psts);
            switch (value)
            {
                case HRMS_Period_Status.Open:
                    break;
                case HRMS_Period_Status.Closed:
                    throw new UtilException("Period is Closed.", Exception_ErrorMessage.PeriodIsClosedImportDenied, null, Exception_ExceptionSeverity.High);
                    break;
                case HRMS_Period_Status.Unused:
                    throw new UtilException("Period is Unused.", Exception_ErrorMessage.PeriodIsUnused, null, Exception_ExceptionSeverity.Medium);
                    break;
                default:
                    break;
            }
        }


        public void CheckPeriodStatus(tstperiod periodMdl)
        {
            HRMS_Period_Status value = (HRMS_Period_Status)Enum.Parse(typeof(HRMS_Period_Status), periodMdl.psts);
            switch (value)
            {
                case HRMS_Period_Status.Open:
                    break;
                case HRMS_Period_Status.Closed:
                    throw new UtilException("Period is Closed.", Exception_ErrorMessage.PeriodIsClosed, null, Exception_ExceptionSeverity.High);
                case HRMS_Period_Status.Unused:
                    throw new UtilException("Period is Unused.", Exception_ErrorMessage.PeriodIsUnused, null, Exception_ExceptionSeverity.Medium);
                default:
                    break;
            }
        }
    }
}


