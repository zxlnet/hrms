using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using GotWell.Model.Authorization;
using GotWell.Common;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Utility;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.HRMS.HRMSBusiness.Syst;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class stcheckrController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("stsearch");
        }

        public void check()
        {
            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                stcheckrBll bll = new stcheckrBll();


                //message = JsonHelper.toJson(result, result.Rows.Count);

            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(ex, true) + "'}";
            }
            finally
            {
            }

            Response.Output.Write(message);
        }
    }
}
