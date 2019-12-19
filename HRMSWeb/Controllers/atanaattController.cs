using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using System.Collections;
using Newtonsoft.Json;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.Model.Common;
using GotWell.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atanaattController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atanaatt");
        }

        public void exportExcel()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                //List<ColumnInfo> records = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());
                List<ColumnInfo> headers = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["headers"].ToString());

                List<ColumnInfo> atdtParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["atdtparams"].ToString());
                List<ColumnInfo> personalParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["personalparams"].ToString());

                string scope = ht["scope"].ToString();

                List<ColumnInfo> fullParameters = new List<ColumnInfo>();

                for (int i = 0; i < personalParameters.Count; i++)
                {
                    fullParameters.Add(personalParameters[i]);
                }

                for (int i = 0; i < atdtParameters.Count; i++)
                {
                    fullParameters.Add(atdtParameters[i]);
                }

                fullParameters.Add(new ColumnInfo() { ColumnName = "atst", ColumnValue = (scope == "All" ? "" : "1") }); //1: abnormal

                atanarstBll bll = new atanarstBll();

                int total = 0;


                List<object> obj = bll.GetAnalyzeResult(fullParameters, false, 0, 0, ref total);


                if ((obj != null) && (total > 0))
                {
                    List<object> finalObj = BuildAnonymousObject(obj).Cast<object>().ToList();
                    UtilExcel.ExportToExcel(finalObj[0].GetType(), Response, this.GetType().Name.GetPageName(), headers, finalObj);
                }
            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void analyzeAttendance()
        {
            string message = "{}";
            List<ColumnInfo> atdtParameters = null;
            List<ColumnInfo> personalParameters = null;
            bool includeLeave;
            bool includeOvertime;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                try
                {
                    atdtParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["atdateparams"].ToString());
                    personalParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["personalparams"].ToString());
                    includeLeave = ht["includelv"].ToString()=="Y"?true:false;
                    includeOvertime = ht["includeot"].ToString() == "Y" ? true : false;
                }
                catch (Exception ex)
                {
                    //message = "{status:'failure',msg:'" + HRMSRes.public_message_ + "'}";
                    Response.Write(message);
                    return;
                }

                atanaattBll bll = new atanaattBll();

                int total = 0;
                bll.AnalyzeAttendance(atdtParameters, personalParameters, includeLeave, includeOvertime);

                message = "{status:'success',msg:'Analyze success.'}";

            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void listAnalResult()
        {
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> atdtParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["atdateparams"].ToString());
                List<ColumnInfo> personalParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["personalparams"].ToString());

                string scope = ht["scope"].ToString();

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                //list(atdtParameters, personalParameters, start, limit, scope);

                List<ColumnInfo> fullParameters = new List<ColumnInfo>();

                for (int i = 0; i < personalParameters.Count; i++)
                {
                    fullParameters.Add(personalParameters[i]);
                }

                for (int i = 0; i < atdtParameters.Count; i++)
                {
                    fullParameters.Add(atdtParameters[i]);
                }

                fullParameters.Add(new ColumnInfo() { ColumnName = "atst", ColumnValue = (scope == "All" ? "" : "1") }); //1: abnormal

                atanarstBll bll = new atanarstBll();

                int total = 0;


                List<object> dataList = bll.GetAnalyzeResult(fullParameters, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(dataList);

                Response.Write("{results:" + total + ",rows:" + json + "}");
            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }

        }

        public void save()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<tatdatmnu> obj = JavaScriptConvert.DeserializeObject<List<tatdatmnu>>(ht["params"].ToString());
                List<List<ColumnInfo>> lstIscf = JavaScriptConvert.DeserializeObject<List<List<ColumnInfo>>>(ht["paramsIscf"].ToString());

                atdatmnuBll bll = new atdatmnuBll();
                bll.SaveDataManu(obj, lstIscf);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

    }
}
