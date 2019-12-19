using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Syst;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using System.Text;
using GotWell.Common;
using GotWell.Model.Authorization;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class stpubmsgController : BaseAttendanceController
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

                funId = pageName + "_" + tabId + "_publish";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_publish:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_exportexcel";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_exportexcel:\"").Append(isValid).Append("\"");
            }

            ViewData["authorization"] = "{" + auth.ToString() + "}";
        }


        public ActionResult Index()
        {
            return getIndex("stpubmsg");
        }

        public void list()
        {
            getList<stpubmsgBll, tstpubmsg>();
        }

        public void New()
        {
            getNew<tstpubmsg>();
        }

        public void Edit()
        {
            getEdit<tstpubmsg>();
        }

        public void Delete()
        {
            getDelete<tstpubmsg>();
        }

        public void exportExcel()
        {
            getExportExcel<stpubmsgBll, tstpubmsg>();
        }

        public void GetNewSMID()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];

                stpubmsgBll bll = new stpubmsgBll();

                string msg = bll.GetNewSMID();

                message = "{status:'success',msg:'" + msg + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void Publish()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                stpubmsgBll bll = new stpubmsgBll();

                bll.Publish(parameters);

                message = "{status:'success',msg:'Publish success.'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }
    }
}

