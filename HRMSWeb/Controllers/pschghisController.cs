using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;
using System.Collections;
using System.Text;
using GotWell.Common;
using GotWell.Model.Authorization;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pschghisController : BaseController
    {
        public override void SetAuthorization(string tabId)
        {
            StringBuilder auth = new StringBuilder();
            object obj = HttpContext.Session[Constant.SESSION_AUTHORIZATION];
            if (obj != null)
            {
                AuthorizationMdl authorization = (AuthorizationMdl)obj;
                string pageName = this.GetType().Name.GetPageName();

                string funId = pageName + "_" + tabId + "_checkout";
                bool isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_checkout:\"").Append(isValid).Append("\",");

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
            return getIndex("pschghis");
        }

        public void list()
        {
            getList<pschghisBll, object>();
        }

        public void exportExcel()
        {
            getExportExcel<pschghisBll, object>();
        }

    }
}