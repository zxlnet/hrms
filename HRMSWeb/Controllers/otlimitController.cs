using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Overtime;
using Newtonsoft.Json;
using System.Collections;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class otlimitController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("otlimit");
        }

        public void list()
        {
            getList<otlimitBll, object>();
        }

        public void New()
        {
            getNew<totlimit>();
        }

        public void Edit()
        {
            getEdit<totlimit>();
        }

        public void Delete()
        {
            getDelete<totlimit>();
        }

        public void exportExcel()
        {
            getExportExcel<otlimitBll, object>();
        }

        public void applyto()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<vw_employment> empParameters = JavaScriptConvert.DeserializeObject<List<vw_employment>>(ht["empparams"].ToString());
                totlimit obj = JavaScriptConvert.DeserializeObject<totlimit>(ht["objparams"].ToString());

                otlimitBll bll = new otlimitBll();

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