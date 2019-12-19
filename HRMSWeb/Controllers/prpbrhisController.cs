using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Payroll;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prpbrhisController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("prpbrhis");
        }

        public void list()
        {
            getList<prpbrhisBll, object>();
        }

        public void New()
        {
            getNew<tprpbrhi>();
        }

        public void Edit()
        {
            getEdit<tprpbrhi>();
        }

        public void Delete()
        {
            getDelete<tprpbrhi>();
        }

        public void exportExcel()
        {
            getExportExcel<prpbrhisBll, object>();
        }

        public void applyto()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<vw_employment> empParameters = JavaScriptConvert.DeserializeObject<List<vw_employment>>(ht["empparams"].ToString());
                tprpbrhi obj = JavaScriptConvert.DeserializeObject<tprpbrhi>(ht["objparams"].ToString());

                prpbrhisBll bll = new prpbrhisBll();

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
