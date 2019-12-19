using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Recruitment;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class rcapplicController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("rcapplic");
        }

        public void list()
        {
            getList<rcapplicBll, object>();
        }

        public void New()
        {
            getNew<trcapplic>();
        }

        public void Edit()
        {
            getEdit<trcapplic>();
        }

        public void Delete()
        {
            getDelete<trcapplic>();
        }

        public void exportExcel()
        {
            getExportExcel<rcapplicBll, object>();
        }

        public void GetActiveApplication()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                rcapplicBll bll = new rcapplicBll();

                int total = 0;

                List<object> dataList = bll.GetActiveApplication(list, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(BuildAnonymousObject(dataList));
                Response.Write("{results:" + total + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }
    }
}
