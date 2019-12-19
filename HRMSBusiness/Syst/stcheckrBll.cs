using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Common;
using System.Data;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Syst;
using GotWell.Utility;
using System.Reflection;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class stcheckrBll  : BaseBll
    {
        DataTable dtResult = new DataTable();
        UtilLog log = new UtilLog();
        stcheckrDal dal = null;

        #region stcheckrBll
        public stcheckrBll()
        {
            dal = new stcheckrDal();
        }
        #endregion

        public DataTable Check(List<object> _parameters)
        {
            try
            {
                BuildResultDT();

                int checknum = 1;

                //CheckData(checknum++, "检查员工轮班设定情况", "GetStep01Data");

                //CheckData(checknum++, "检查员工日历设定情况", "GetStep01Data");

                //CheckData(checknum++, "检查员工日历设定情况", "GetStep01Data");

                //CheckData(checknum++, "检查员工工资计算规则情况", "GetStep01Data");

                //CheckData(checknum++, "检查员工工资计算规则情况", "GetStep01Data");

                //CheckData(checknum++, "检查员工有没有设定默认的银行账号", "GetStep01Data");

                //CheckData(checknum++, "检查员工有没有设定默认的成本中心", "GetStep01Data");
 
                return dtResult;
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


        public Exception_ErrorMessage CheckData(int rownum, string checkDesc, String fun)
        {
            log.LogInfoWithLevel("Start: " + MethodBase.GetCurrentMethod().Name + ".", Log_LoggingLevel.Admin);

            try
            {
                Type t = dal.GetType();
                MethodInfo method = t.GetMethod(fun);
                DataSet ds = (DataSet)method.Invoke(dal, new object[] { });

                if (ds.Tables[0].Rows.Count > 0)
                {
                    //string errDetails = JsonHelper.toJson(ds.Tables[0], ds.Tables[0].Rows.Count);
                    //log.LogInfoWithLevel(checkDesc + " fail. ", Log_LoggingLevel.Admin);
                    //log.LogInfoWithLevel("Total " + ds.Tables[0].Rows.Count.ToString() + " record(s) fail.", Log_LoggingLevel.Admin);
                    //string fileName = (new UtilLog()).LogInfoToNewFile(errDetails, CurrentPageInfo.PageId, "[" + CurrentPageInfo.PageName + "]Data check fail at " + UtilDatetime.FormatDateTime3(Function.GetCurrentTime()) + " by " + Function.GetCurrentUser());
                    //log.LogInfoWithLevel("Please check error details in  " + fileName + ".", Log_LoggingLevel.Admin);

                    string fileName = string.Empty;
                    AddIntoResultDT(rownum.ToString(), checkDesc, "Fail.Please check " + fileName, fileName);
                }
                else
                {
                    AddIntoResultDT(rownum.ToString(), checkDesc, "Success", string.Empty);
                    log.LogInfoWithLevel("End: " + MethodBase.GetCurrentMethod().Name + "[successfully].", Log_LoggingLevel.Admin);
                }
                return Exception_ErrorMessage.NoError;
            }
            catch (UtilException ex)
            {
                AddIntoResultDT(rownum.ToString(), checkDesc, "Error." + ex.Message, string.Empty);
                log.LogInfoWithLevel("End: " + MethodBase.GetCurrentMethod().Name + "[fail]. ", Log_LoggingLevel.Admin);
                //throw ex;
            }
            catch (Exception ex)
            {
                AddIntoResultDT(rownum.ToString(), checkDesc, "Error." + ex.Message, string.Empty);
                log.LogInfoWithLevel("End: " + MethodBase.GetCurrentMethod().Name + "[fail]. Error:" + ex.Message + ".", Log_LoggingLevel.Admin);
            }

            return Exception_ErrorMessage.NoError;

        }

        #region BuildResultDT
        private void BuildResultDT()
        {
            dtResult.Columns.Add("sqno", typeof(System.String));
            dtResult.Columns.Add("cknm", typeof(System.String));
            dtResult.Columns.Add("ckrs", typeof(System.String));
            dtResult.Columns.Add("finm", typeof(System.String));
        }
        #endregion

        private void AddIntoResultDT(string _sequence, string _checkName, string _checkResult, string _fileName)
        {
            DataRow row = dtResult.NewRow();
            row["sqno"] = _sequence;
            row["cknm"] = _checkName;
            row["ckrs"] = _checkResult;
            row["finm"] = _fileName;

            dtResult.Rows.Add(row);
        }
    }
}
