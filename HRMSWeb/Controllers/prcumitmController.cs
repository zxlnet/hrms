using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSBusiness.Payroll;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prcumitmController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("prcumitm");
        }

        public void calculation()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                string rnno = ht["rnno"].ToString();
                string cond = ht["cond"].ToString();
                List<ColumnInfo> personalParameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["custcond"].ToString());
                string perd = ht["perd"].ToString();

                prcumitmBll bll = new prcumitmBll();

                bll.Calculate(rnno, cond, personalParameters, perd);
                message = "{status:'success',msg:'" + "Calcualte successfully." + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse("Calcualte fail.", ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void listdetails()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                prcumitmBll bll = new prcumitmBll();

                int total = 0;

                List<object> dataList = bll.GetDetails(list, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(dataList);
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
