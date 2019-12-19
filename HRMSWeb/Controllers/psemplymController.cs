using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSBusiness.Common;
using System.Text;
using GotWell.Common;
using GotWell.Model.Authorization;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psemplymController : BaseController
    {
        public override void SetAuthorization(string tabId)
        {
            StringBuilder auth = new StringBuilder();
            object obj = HttpContext.Session[Constant.SESSION_AUTHORIZATION];
            if (obj != null)
            {
                AuthorizationMdl authorization = (AuthorizationMdl)obj;
                string pageName = this.GetType().Name.GetPageName();

                string funId = pageName + "_" + tabId + "_add";
                bool isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_add:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_edit";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_edit:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_delete";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_delete:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_query";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_query:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_rehire";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_rehire:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_terminate";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_terminate:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_change";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_change:\"").Append(isValid).Append("\",");
                
                funId = pageName + "_" + tabId + "_exportexcel";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_exportexcel:\"").Append(isValid).Append("\"");

            }

            ViewData["authorization"] = "{" + auth.ToString() + "}";
        }

        public ActionResult Index()
        {
            return getIndex("psemplym");
        }

        public void list()
        {
            getList<psemplymBll, object>();
        }

        public void New()
        {
            getNew();
        }

        public void Edit()
        {
            getEdit();
        }

        public void Delete()
        {
            getDelete();
        }

        public void exportExcel()
        {
            getExportExcel<psemplymBll, object>();
        }

        public void getNew()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                tpsemplym obj = JavaScriptConvert.DeserializeObject<tpsemplym>(record);

                psemplymBll bll = new psemplymBll();
                bll.InsertEmployment(obj);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_AddWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_AddBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getEdit() 
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                string action = this.Request["action"];

                tpsemplym obj = JavaScriptConvert.DeserializeObject<tpsemplym>(record);

                new psemplymBll().UpdateEmployment(obj, new List<ColumnInfo> { new ColumnInfo() { ColumnName = "emno", ColumnValue = obj.emno } }, action);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getDelete() 
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                new psemplymBll().DoDelete<tpsemplym>(new List<ColumnInfo> { new ColumnInfo() { ColumnName = "emno", ColumnValue = ht["emno"].ToString() } });
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getAdvQryEmployment()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                psemplymBll bll = new psemplymBll();

                int total = 0;

                List<object> dataList = bll.GetSelectedRecordsForAdvQry(list, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(dataList);
                Response.Write("{results:" + total + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

    }
}