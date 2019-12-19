using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Syst;
using GotWell.HRMS.HRMSWeb.Controllers;
using GotWell.Model.Common;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.HRMS;
using GotWell.Common;
using System.Collections;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class staldlybController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("staldlyb");
        }

        public void listBoardMessage()
        {
            try
            {
                string record = this.Request["record"];

                //Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                //List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());
                List<ColumnInfo> list = new List<ColumnInfo>() { new ColumnInfo(){ColumnName="reci",ColumnValue=Function.GetCurrentStaff()}};

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                int total = 0;

                staldlybBll bll = new staldlybBll();

                List<tstaldlyb> dataList = bll.GetSelectedRecords<tstaldlyb>(list, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(dataList);
                Response.Write("{results:" + total + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void handleMesssage()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                staldlybBll bll = new staldlybBll();
                bll.HandleMesssage(ht["adid"].ToString());

                Response.Write("{status:'success',msg:'Message handled success.'}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

    }
}
