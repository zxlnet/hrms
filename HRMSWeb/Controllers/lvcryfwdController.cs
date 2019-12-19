using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Leave;
using GotWell.Model.HRMS;
using GotWell.Model.Authorization;
using System.Text;
using GotWell.Common;
using Newtonsoft.Json;
using System.Collections;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class lvcryfwdController : BaseAttendanceController
    {
        public override void SetAuthorization(string tabId)
        {
            StringBuilder auth = new StringBuilder();
            object obj = HttpContext.Session[Constant.SESSION_AUTHORIZATION];
            if (obj != null)
            {
                AuthorizationMdl authorization = (AuthorizationMdl)obj;
                string pageName = this.GetType().Name.GetPageName();

                string funId = pageName + "_" + tabId + "_carryforward";
                bool isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_carryforward:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_edit";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_edit:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_delete";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_delete:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_query";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_query:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_exportexcel";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_exportexcel:\"").Append(isValid).Append("\"");
            }

            ViewData["authorization"] = "{" + auth.ToString() + "}";
        }

        public ActionResult Index()
        {
            return getIndex("lvcryfwd");
        }

        public void list()
        {
            getList<lvcryfwdBll, object>();
        }

        public void New()
        {
            getNew<tlvcryfwd>();
        }

        public void Edit()
        {
            getEdit<tlvcryfwd>();
        }

        public void Delete()
        {
            getDelete<tlvcryfwd>();
        }

        public void exportExcel()
        {
            getExportExcel<lvcryfwdBll, object>();
        }

        public void carryforward()
        {
            string message = "{}";
            List<ColumnInfo> carryforwardParameters = null;
            List<ColumnInfo> personalParameters = null;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                try
                {
                    carryforwardParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["carryforwardparams"].ToString());
                    personalParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["personalparams"].ToString());

                }
                catch (Exception ex)
                {
                    message = "{status:'failure',msg:'Carryforward parameters error.'}";
                    Response.Write(message);
                    return;
                }

                lvcryfwdBll bll = new lvcryfwdBll();
                bll.CarryForward(carryforwardParameters, personalParameters);

                message = "{status:'success',msg:'Carryforward success.'}";

            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

    }
}