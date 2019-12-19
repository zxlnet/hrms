using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Syst;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.Authorization;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using Newtonsoft.Json;


namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class stperiodController : Controller
    {
        public ActionResult index()
        {
            try
            {
                string tabId = this.Request["menuId"];
                ViewData["config"] = "{tabId:\"" + tabId + "\",pageid:\"" + this.GetType().Name.GetPageName() + "\"}";

                StringBuilder auth = new StringBuilder();
                object obj = HttpContext.Session[Constant.SESSION_AUTHORIZATION];
                if (obj != null)
                {
                    AuthorizationMdl authorization = (AuthorizationMdl)obj;
                    string pageName = this.GetType().Name.GetPageName();

                    string funId = pageName + "_" + tabId + "_update";
                    bool isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_update:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_openperiod";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_openperiod:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_closeperiod";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_closeperiod:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_query";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_query:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_exportexcel";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_exportexcel:\"").Append(isValid).Append("\"");
                }

                ViewData["authorization"] = "{" + auth.ToString() + "}";

                return this.View("stperiod");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void edit()
        {
            string msg = string.Empty;
            try
            {
                stperiodBll stperiodBll = new stperiodBll();
                string request = Request.Form["record"];
                tstperiod stperiodMdl = JavaScriptConvert.DeserializeObject<tstperiod>(request);
                Exception_ErrorMessage error = stperiodBll.UpdatePeriod(stperiodMdl);
                if (error == Exception_ErrorMessage.NoError)
                {
                    msg = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
                }
            }
            catch (Exception ex)
            {
                msg = "{status:'failure',msg:'" + HRMSRes.Public_Message_EditBad + "'}";
            }

            Response.Output.Write(msg);
        }

        //public void delete()
        //{
        //    string msg = string.Empty;
        //    try
        //    {
        //        PeriodBll periodBll = new PeriodBll();
        //        string year = Request.Form["Year"];
        //        string period = Request.Form["Period"];
        //        Exception_ErrorMessage error = periodBll.DeletePeriod(year, period);
        //        if (error == Exception_ErrorMessage.NoError)
        //        {
        //            msg = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = "{status:'failure',msg:'" + HRMSRes.Public_Message_DeleteBad + "'}";
        //    }
        //    Response.Output.Write(msg);
        //}

        public void list()
        {

            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                string startStr = this.Request["start"];
                string limitStr = this.Request["limit"];

                if ((record==null) || (record.Length<1))
                {
                    record="{\"year\":\"" + UtilDatetime.FormatDate3(DateTime.Now).Substring(0,4) + "\"}";
                }
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                int start = 0;
                if (startStr != null)
                {
                    start = Convert.ToInt32(startStr);
                }

                int limit = 0;
                if (limitStr != null)
                {
                    limit = Convert.ToInt32(limitStr);
                }

                stperiodBll bll = new stperiodBll();

                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="year",ColumnValue= ht["year"].ToString()}
                    };

                int total = 0;
                List<tstperiod> periodList = bll.GetSelectedRecords<tstperiod>(parameters, true, start, start + limit, ref total);

                string json = JavaScriptConvert.SerializeObject(periodList);
                json = "{results:" + total + ",rows:" + json + "}";
                Response.Write(json);
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void closePeriod()
        {
            string message = string.Empty;
            try
            {

                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                //string year = ht["year"].ToString();
                string period = ht["perd"].ToString();

                stperiodBll bll = new stperiodBll();
                tstperiod periodMdl = bll.GetPeriod(period);

                Exception_ErrorMessage result = bll.ClosePeriod(periodMdl);

                if (result.Equals(Exception_ErrorMessage.NoError))
                {
                    message = "{status:'success',msg:'" + HRMSRes.Public_Message_ClosePeriodSuccess + "'}";
                }
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_ClosePeriodFail, ex, true) + "'}";
            }
            Response.Output.Write(message);
        }

        public void openPeriod()
        {
            string message = string.Empty;
            try
            {

                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                //string year = ht["year"].ToString();
                string period = ht["perd"].ToString();

                stperiodBll bll = new stperiodBll();
                tstperiod periodMdl = bll.GetPeriod(period);

                Exception_ErrorMessage result = bll.OpenPeriod(periodMdl);

                if (result.Equals(Exception_ErrorMessage.NoError))
                {
                    message = "{status:'success',msg:'" + HRMSRes.Public_Message_OpenPeriodSuccess + "'}";
                }
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_OpenPeriodFail, ex, true) + "'}";
            }
            Response.Output.Write(message);
        }


        public void listPeriodYear()
        {
            try
            {
                stperiodBll bll = new stperiodBll();
                List<string> yearList = bll.GetYearList();
                StringBuilder json = new StringBuilder();
                for (int i = 0; i < yearList.Count; i++)
                {
                    if (!json.ToString().Equals(""))
                    {
                        json.Append(",");
                    }
                    json.Append("{year:'" + yearList[i] + "'}");
                }
                Response.Output.Write("{results:" + yearList.Count + ",rows:[" + json.ToString() + "]}");
            }
            catch (Exception ex)
            {
                throw ex;
            } 

        }

        public void exportExcel()
        {
            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                string header = this.Request["header"];

                if ((record == null) || (record.Length < 1))
                {
                    record = "{\"year\":\"" + UtilDatetime.FormatDate3(DateTime.Now).Substring(0, 4) + "\"}";
                }
                
                if (header == null || header.Equals(string.Empty))
                {
                    return;
                }
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> headers = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(header);
                stperiodBll bll = new stperiodBll();

                List<ColumnInfo> parameters = new List<ColumnInfo>(){
                        new ColumnInfo(){ColumnName="year",ColumnValue= ht["year"].ToString()}
                    };

                List<tstperiod> yearList = bll.GetSelectedRecords<tstperiod>(parameters);
                if (yearList != null)
                {
                    UtilExcel.ExportToExcel(Response, this.GetType().Name.GetPageName(), headers, yearList);
                }
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_ExportExcelFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

    }
}
