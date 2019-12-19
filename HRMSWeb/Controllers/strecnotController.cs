using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSBusiness.Syst;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class strecnotController : BaseController
    {
        public void getNote()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                strecnotBll bll = new strecnotBll();
                string note = bll.GetNote(ht["rfid"].ToString(),Function.GetCurrentUser());

                Response.Write("{status:'success',msg:'" + note.EscapeHtml() + "'}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void updateNote()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                strecnotBll bll = new strecnotBll();
                bll.UpdateNote(ht["rfid"].ToString(),Function.GetCurrentUser(), ht["note"].ToString());

                Response.Write("{status:'success',msg:'Update note success.'}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

    }
}
