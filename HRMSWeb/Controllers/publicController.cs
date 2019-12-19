using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.MobileControls;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.HRMS.HRMSBusiness.Syst;
using GotWell.HRMS.HRMSCore.DataControl;
using GotWell.HRMS.HRMSCore.LogControl;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using Newtonsoft.Json;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class publicController : Controller
    {
        public void listPeriodWithoutClosed()
        {
            try
            {
                stperiodBll bll = new stperiodBll();
                List<tstperiod> periodList = bll.GetPeriodWithoutClosed();
                string json = JavaScriptConvert.SerializeObject(periodList);
                json = "{results:" + periodList.Count + ",rows:" + json + "}";

                Response.Output.Write(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void listAllPeriod()
        {
            try
            {
                stperiodBll bll = new stperiodBll();
                List<tstperiod> periodList = bll.GetAllPeriods();
                string json = JavaScriptConvert.SerializeObject(periodList);
                json = "{results:" + periodList.Count + ",rows:" + json + "}";

                Response.Output.Write(json); 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void checkPeriodStatus()
        {
            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                stperiodBll periodBll = new stperiodBll();
                tstperiod periodMdl = periodBll.GetPeriod(ht["period"].ToString());

                periodBll.CheckSelectedPeriodStatus(periodMdl);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_CheckPeriodStatusSuccess + "'}";
            }
            catch (UtilException ex)
            {
                if ((ex.Code == (int)Exception_ErrorMessage.PeriodIsUnused) ||
                    (ex.Code == (int)Exception_ErrorMessage.StepIsCompleted))
                {
                    message = "{status:'ask',msg:'" + ExceptionPaser.Parse(ex, true) + "'}";
                }
                else
                {
                    message = "{status:'fail',msg:'" + ExceptionPaser.Parse(ex, true) + "'}";
                }
            }
            catch (Exception ex)
            {
                message = "{status:'fail',msg:'" + ExceptionPaser.Parse(ex, true) + "'}";
            }

            Response.Output.Write(message);
        }

        public void checkPeriodStatusOnly()
        {
            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                stperiodBll periodBll = new stperiodBll();
                tstperiod periodMdl = periodBll.GetPeriod(ht["period"].ToString());

                periodBll.CheckSelectedPeriodStatus(periodMdl);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_CheckPeriodStatusSuccess + "'}";
            }
            catch (UtilException ex)
            {
                if ((ex.Code == (int)Exception_ErrorMessage.PeriodIsUnused) ||
                    (ex.Code == (int)Exception_ErrorMessage.StepIsCompleted))
                {
                    message = "{status:'ask',msg:'" + ExceptionPaser.Parse(ex, true) + "'}";
                }
                else
                {
                    message = "{status:'fail',msg:'" + ExceptionPaser.Parse(ex, true) + "'}";
                }
            }
            catch (Exception ex)
            {
                message = "{status:'fail',msg:'" + ExceptionPaser.Parse(ex, true) + "'}";
            }

            Response.Output.Write(message);
        }

        public void GetMaxemno()
        {
            string message = string.Empty;
            try
            {
                pspersonBll bll = new pspersonBll();
                string sMax = Convert.ToString(bll.GetMaxemno() + 1);
                message = "{status:'success',msg:'" + sMax + "'}"; ;
            }
            catch (Exception ex)
            {
                message = "{status:'fail',msg:''}"; ;
            }
            Response.Output.Write(message);
        }

        public void GetMaxsqno()
        {
            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                BaseBll bll = new BaseBll();

                if (ht["emno"].ToString().Equals(string.Empty))
                    message = "{status:'success',msg:''}"; 
                else
                {
                    int? list = bll.GetMaxsqno(ht["tablename"].ToString(), ht["emno"].ToString());

                    string sMax = string.Empty;

                    if (!list.HasValue) sMax = "1";
                    else sMax = Convert.ToString(list + 1);

                    message = "{status:'success',msg:'" + sMax + "'}";
                }
            }
            catch (Exception ex)
            {
                message = "{status:'fail',msg:''}"; ;
            }
            Response.Output.Write(message);
        }

        public void GetMaxNo()
        {
            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                BaseBll bll = new BaseBll();

                if (ht["field"].ToString().Equals(string.Empty))
                    message = "{status:'success',msg:''}";
                else
                {
                    int? list = bll.GetMaxNo(ht["tablename"].ToString(), ht["field"].ToString());

                    string sMax = string.Empty;

                    if (!list.HasValue) sMax = "1";
                    else sMax = Convert.ToString(list + 1);

                    message = "{status:'success',msg:'" + sMax + "'}";
                }
            }
            catch (Exception ex)
            {
                message = "{status:'fail',msg:''}"; ;
            }
            Response.Output.Write(message);
        }

        public void listValidPerson()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                int total = 0;

                psemplymBll bll = new psemplymBll();

                List<vw_employment> dataList = bll.GetValidEmployment(); //.GetSelectedRecords<vw_employment>(list, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(dataList);
                Response.Write("{results:" + total + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void getRecordLogHistory()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                //List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                List<ColumnInfo> lstParameter = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rfid", ColumnValue = ht["rfid"].ToString() } };

                stactlogBll bll = new stactlogBll();

                int total = 0;

                List<tstactlog> dataList = bll.GetSelectedRecords<tstactlog>(lstParameter, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(dataList);
                Response.Write("{results:" + total + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void confirmRecord()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                strecstsBll bll = new strecstsBll();
                bll.ConfirmRecord(ht["rfid"].ToString());

                Response.Write("{status:'success',msg:'Confirm successfully.'}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }
    }
}
