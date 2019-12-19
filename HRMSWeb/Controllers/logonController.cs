using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Common;
using GotWell.Utility;
using System.Collections;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSBusiness.Authorization;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class logonController : BaseController
    {
        public ActionResult Index()
        {
            ViewData["contextPath"] = Parameter.VIRTURE_DIRECT;
            string language = UtilRequest.getCookieValue(Request, "UserSettings", "language");
            if (language == null)
            {
                language = "zh-cn";
            }
            ViewData["lang"] = language;
            return this.View("logon");
        }

        public void logon()
        {
            string message = "";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                string msg = new logonBll().UserValidation(ht["urid"].ToString(), ht["pswd"].ToString(),ConfigReader.getAppName());

                if (msg.Substring(0, 7) == "success")
                {
                    tscuser user = JavaScriptConvert.DeserializeObject<tscuser>(msg.Substring(8, msg.Length - 8));
                    this.HttpContext.Session[Constant.SESSION_CURRENT_USER] = user.urid;
                    this.HttpContext.Session[Constant.SESSION_CURRENT_STAFF] = user.sfid;//存放登录用户对应的Staff ID

                    message = "{status:'success',msg:'Verify success.'}";
                }
                else
                {
                    message = "{status:'failure',msg:'" + msg + "'}";
                }
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            
            Response.Write(message);

        }

        public ActionResult logout()
        {
            this.HttpContext.Session[Constant.SESSION_AUTHORIZATION] = null;
            return RedirectToAction("Index");
        }
    }
}
