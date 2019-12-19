using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.Common;
using System.Collections;
using Newtonsoft.Json;
using System.Text;
using GotWell.Model.Authorization;
using GotWell.HRMS.HRMSBusiness.Overtime;
using GotWell.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class otttlvController : BaseAttendanceController
    {
        public override void SetAuthorization(string tabId)
        {
            StringBuilder auth = new StringBuilder();
            object obj = HttpContext.Session[Constant.SESSION_AUTHORIZATION];
            if (obj != null)
            {
                AuthorizationMdl authorization = (AuthorizationMdl)obj;
                string pageName = this.GetType().Name.GetPageName();

                string funId = pageName + "_" + tabId + "_transfer";
                bool isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_transfer:\"").Append(isValid).Append("\",");

                //funId = pageName + "_" + tabId + "_edit";
                //isValid = authorization.checkPermissionByFuncUrl(funId);
                //auth.Append(tabId).Append("_edit:\"").Append(isValid).Append("\",");

                //funId = pageName + "_" + tabId + "_delete";
                //isValid = authorization.checkPermissionByFuncUrl(funId);
                //auth.Append(tabId).Append("_delete:\"").Append(isValid).Append("\",");

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
            return getIndex("otttlv");
        }

        public void list()
        {
            getList<otttlvBll, object>();
        }

        //public void New()
        //{
        //    getNew<tlvcryfwd>();
        //}

        //public void Edit()
        //{
        //    getEdit<tlvcryfwd>();
        //}

        //public void Delete()
        //{
        //    getDelete<tlvcryfwd>();
        //}

        public void exportExcel()
        {
            getExportExcel<otttlvBll, object>();
        }

        public void transfer()
        {
            string message = "{}";
            List<ColumnInfo> transferParameters = null;
            List<ColumnInfo> personalParameters = null;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                try
                {
                    transferParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["otttlvparams"].ToString());
                    personalParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["personalparams"].ToString());

                }
                catch (Exception ex)
                {
                    message = "{status:'failure',msg:'Transfer parameters error.'}";
                    Response.Write(message);
                    return;
                }

                otttlvBll bll = new otttlvBll();
                bll.BatchTransferToLeave(transferParameters, personalParameters);

                message = "{status:'success',msg:'Transfering success.'}";

            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

    }
}