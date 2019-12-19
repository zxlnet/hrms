using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Payroll;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prsalhisController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("prsalhis");
        }

        public void list()
        {
            getList<prsalhisBll, object>();
        }

        public void edit()
        {
            getEdit();
        }

        public void exportExcel()
        {
            getExportExcel<prsalhisBll, object>();
        }

        public void getEdit()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());
                string obj = JavaScriptConvert.DeserializeObject<string>(ht["params"].ToString());


                new prsalhisBll().UpdateAdjustedValue(parameters, Convert.ToDouble(obj));
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }
     }
}
