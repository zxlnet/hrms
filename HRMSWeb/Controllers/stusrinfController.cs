using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using GotWell.Common;
using System.Collections;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSBusiness.Syst;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class stusrinfController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("stusrinf");
        }

        public void getUserInfo()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                stusrinfBll bll = new stusrinfBll();
                string msg = bll.GetUserInformation(Function.GetCurrentUser(), ConfigReader.getAppName());

                Response.Write(msg);

            }
            catch (Exception ex)
            {
                string message = ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true);
                Response.Output.Write(message);
            }
        }

        public void changePassword()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                stusrinfBll bll = new stusrinfBll();
                string note = bll.ChangeUserPassword(Function.GetCurrentUser(), ht["opwd"].ToString(), ht["npwd"].ToString(), ConfigReader.getAppName());

                Response.Write("{status:'success',msg:'" + note.EscapeHtml() + "'}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

    }
}
