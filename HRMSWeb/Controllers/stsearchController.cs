using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSBusiness.Syst;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class stsearchController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("stsearch");
        }

        public void search()
        {
            try
            {
                string record = this.Request["record"];
                string mode = this.Request["mode"];

                stsearchBll bll = new stsearchBll();
                List<StSearchResult> lstResult = bll.Search(record,mode);
                string json = JavaScriptConvert.SerializeObject(lstResult);
                Response.Write("{results:" + lstResult.Count + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }
    }
}
