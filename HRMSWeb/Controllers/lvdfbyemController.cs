using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using System.Collections;
using GotWell.HRMS.HRMSBusiness.Leave;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class lvdfbyemController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("lvdfbyem");
        }

        public void list()
        {
            getList<lvdfbyempBll, object>();
        }

        public void New()
        {
            getNew<tlvdfbyem>();
        }

        public void Edit()
        {
            getEdit<tlvdfbyem>();
        }

        public void Delete()
        {
            getDelete<tlvdfbyem>();
        }

        public void exportExcel()
        {
            getExportExcel<lvdfbyempBll, object>();
        }

        public void applyto()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<string> empParameters = JavaScriptConvert.DeserializeObject<List<string>>(ht["empparams"].ToString());
                tlvdfbyem obj = JavaScriptConvert.DeserializeObject<tlvdfbyem>(ht["objparams"].ToString());

                lvdfbyempBll bll = new lvdfbyempBll();

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
