using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Leave;
using GotWell.Model.HRMS;
using Newtonsoft.Json;
using System.Collections;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class lvlealmtController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("lvlealmt");
        }

        public void list()
        {
            getList<lvlealmtBll, object>();
        }

        public void New()
        {
            getNew<tlvlealmt>();
        }

        public void Edit()
        {
            getEdit<tlvlealmt>();
        }

        public void Delete()
        {
            getDelete<tlvlealmt>();
        }

        public void exportExcel()
        {
            getExportExcel<lvlealmtBll, object>();
        }

        public void applyto()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<vw_employment> empParameters = JavaScriptConvert.DeserializeObject<List<vw_employment>>(ht["empparams"].ToString());
                tlvlealmt obj = JavaScriptConvert.DeserializeObject<tlvlealmt>(ht["objparams"].ToString());

                lvlealmtBll bll = new lvlealmtBll();

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