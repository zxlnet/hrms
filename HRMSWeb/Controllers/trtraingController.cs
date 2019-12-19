using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Training;
using GotWell.Model.HRMS;
using Newtonsoft.Json;
using System.Collections;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class trtraingController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("trtraing");
        }

        public void list()
        {
            getList<trtraingBll, object>();
        }

        public void New()
        {
            getNew<ttrtraing>();
        }

        public void Edit()
        {
            getEdit<trtraingBll, ttrtraing>();
        }

        public void Delete()
        {
            getDelete<ttrtraing>();
        }

        public void exportExcel()
        {
            getExportExcel<trtraingBll, object>();
        }

        public void publish()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<vw_employment> empParameters = JavaScriptConvert.DeserializeObject<List<vw_employment>>(ht["empparams"].ToString());
                string trcd = ht["objparams"].ToString();

                trtraingBll bll = new trtraingBll();

                bll.Publish(empParameters, trcd, "Y", "Y");

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_AddWell + "'}";

                Response.Write(message);
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }
    }
}
