using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;
using Newtonsoft.Json;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using System.Collections;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atpvtcldController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atpvtcld");
        }

        public void list()
        {
            getList<atpvtcldBll, object>();
        }

        public void New()
        {
            getNew<tatpricld>();
        }

        public void Edit()
        {
            getEdit<tatpricld>();
        }

        public void Delete()
        {
            getDelete<tatpricld>();
        }

        public void exportExcel()
        {
            getExportExcel<atpvtcldBll, object>();
        }

        public void applyto()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<vw_employment> empParameters = JavaScriptConvert.DeserializeObject<List<vw_employment>>(ht["empparams"].ToString());
                tatpricld obj = JavaScriptConvert.DeserializeObject<tatpricld>(ht["objparams"].ToString());

                atpvtcldBll bll = new atpvtcldBll();

                bll.ApplyTo(empParameters, obj);

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
