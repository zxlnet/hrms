using System.Linq;
using System.Text;
using System.Web.Mvc;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Authorization;
using GotWell.HRMS.HRMSBusiness.Master;
using GotWell.Model.HRMS;
using GotWell.Utility;
using GotWell.HRMS.HRMSBusiness.Syst;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.Authorization;
using System.Collections.Generic;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            GetRootDirect();
            string language = UtilRequest.getCookieValue(Request, "UserSettings","language");
            if (language == null)
            {
                language = "zh-cn";                
            }

            StSystemConfig sysCfg = new stsyscfgBll().GetSystemSetting();
            Parameter.CURRENT_SYSTEM_CONFIG = sysCfg;
            this.ViewData["sysCfg"] = JavaScriptConvert.SerializeObject(Parameter.CURRENT_SYSTEM_CONFIG);
            Parameter.CURRENT_USER_OPEN_MODE = "Normal";

            if (sysCfg.ScSBAD == "Y")
            {
                if (User.Identity.Name.ToString().Equals(""))
                {
                    return this.RedirectToAction("Index", "Error");
                }

                if (this.HttpContext.Session[Constant.SESSION_CURRENT_USER] == null)
                {
                    this.HttpContext.Session[Constant.SESSION_CURRENT_USER] = User.Identity.Name.ToString(); //get current AD user
                }
            }
            else
            {
                if (this.HttpContext.Session[Constant.SESSION_CURRENT_USER] == null)
                {
                    return this.RedirectToAction("Index", "Logon");
                }
            }

            if (this.HttpContext.Session[Constant.SESSION_AUTHORIZATION] == null)
            {
                this.HttpContext.Session[Constant.SESSION_AUTHORIZATION] = new AuthorizationBll().GetAuthorization(Function.GetCurrentUser(), Parameter.APPLICATION_NAME); ;
                this.HttpContext.Session[Constant.SESSION_CURRENT_STAFF] = ((AuthorizationMdl)this.HttpContext.Session[Constant.SESSION_AUTHORIZATION]).User.sfid;
            }

            this.HttpContext.Session[Constant.SESSION_CURRENT_MUF] = new MenuConfigBll().getMUFForSession();

            ViewData["lang"] = language;
            ViewData["contextPath"] = Parameter.VIRTURE_DIRECT;
            ViewData["currentEnvironment"] = Common.ConfigReader.getEnvironmentName(); 

            this.render();
            return View("Index");
        }

        public ActionResult changeLang()
        {
            string lang = this.Request["lang"];
            if (lang != null && !lang.Equals(string.Empty))
            {
                ViewData["lang"] = lang;
                UtilRequest.saveCookie(Request, Response, "UserSettings","language",lang);
            }
            return RedirectToAction("Index", "Home");
        }

        private void render()
        {
            this.ViewData["currentUser"] = Function.GetCurrentUser();

            stperiodBll periodBll = new stperiodBll();
            List<tstperiod> lstPeriodMdl = periodBll.GetPeriodByStatus(HRMS_Period_Status.Open.ToString());
            StringBuilder builder = new StringBuilder();

            if (lstPeriodMdl.Count <=0)
            {
                string from = "19000101";
                string to = "19000101";
                builder.Append("{");
                builder.Append("fullPeriod:\"").Append("1900").Append("01").Append("\",");
                builder.Append("year:\"").Append("1900").Append("\",");
                builder.Append("period:\"").Append("01").Append("\",");
                builder.Append("start:\"").Append(from).Append("\",");
                builder.Append("end:\"").Append(to).Append("\"");
                builder.Append("}");
            }
            else
            {
                string from = lstPeriodMdl[0].pest.ToString("yyyyMMdd");
                string to = lstPeriodMdl[0].peen.ToString("yyyyMMdd");
                builder.Append("{");
                builder.Append("fullPeriod:\"").Append(lstPeriodMdl[0].year).Append(lstPeriodMdl[0].mnth).Append("\",");
                builder.Append("year:\"").Append(lstPeriodMdl[0].year).Append("\",");
                builder.Append("month:\"").Append(lstPeriodMdl[0].mnth).Append("\",");
                builder.Append("start:\"").Append(from).Append("\",");
                builder.Append("end:\"").Append(to).Append("\"");
                builder.Append("}");
            }
            this.ViewData["currentPeriod"] = builder.ToString();
        }

        private void GetRootDirect()
        {
            string contextPath = Request.ApplicationPath;
            if (contextPath.Equals("/"))
            {
                contextPath = string.Empty;
            }
            Parameter.VIRTURE_DIRECT = contextPath;
        }

        private string GetOpenMode()
        {
            var q1 = (from p in ((AuthorizationMdl)HttpContext.Session[Constant.SESSION_AUTHORIZATION]).Applications
                      select p.Roles).ToList();

            string openMode = string.Empty;
            StSystemConfig sysCfg = (StSystemConfig)Parameter.CURRENT_SYSTEM_CONFIG;
            if (sysCfg.PsASTESPI == "N")
            {
                openMode = "A";
            }
            else
            {
                for (int i = 0; i < q1.Count; i++)
                {
                    for (int j = 0; j < q1[i].Count; j++)
                    {
                        RoleMdl roleMdl = (RoleMdl)q1[i][j];
                        if (roleMdl.alep == "Y")
                            openMode = "B";

                        switch (roleMdl.roty)
                        {
                            case "HR Admin":
                                openMode = "A";
                                break;
                            case "HR Staff":
                                openMode = "A";
                                break;
                            case "General Staff":
                                if (openMode != "A")
                                    openMode = "B";
                                break;
                        }
                    }
                }
            }

            return openMode;
        }

    }
}
