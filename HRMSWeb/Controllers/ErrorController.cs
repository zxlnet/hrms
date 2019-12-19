using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Common;
using GotWell.Utility;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(string msg)
        {
            //string msg = this.ReadFromRequest("msg");

            ViewData["contextPath"] = Parameter.VIRTURE_DIRECT;
            string language = UtilRequest.getCookieValue(Request, "UserSettings", "language");
            if (language == null)
            {
                language = "zh-cn";
            }
            ViewData["lang"] = language;

            ViewData["config"] = "{msg:\"" + msg.EscapeHtml() + "\"}";

            return this.View("Error");
        }

        public ActionResult Timeout()
        {
            //string msg = this.ReadFromRequest("msg");
            string msg = HRMSRes.ResourceManager.GetString("Public_Message_SessionTimeOut");

            ViewData["contextPath"] = Parameter.VIRTURE_DIRECT;
            string language = UtilRequest.getCookieValue(Request, "UserSettings", "language");
            if (language == null)
            {
                language = "zh-cn";
            }
            ViewData["lang"] = language;

            ViewData["config"] = "{msg:\"" + msg.EscapeHtml() + "\"}";

            return this.View("Error");
        }

    }
}
