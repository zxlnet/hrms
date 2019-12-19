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
    public class rcintschController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("rcintsch");
        }

        public void list()
        {
            getList<rcintschBll, object>();
        }

        public void New()
        {
            getNew();
        }

        public void Edit()
        {
            getEdit();
        }

        public void Delete()
        {
            getDelete<trcintsch>();
        }

        public void exportExcel()
        {
            getExportExcel<rcapplicBll, object>();
        }

        public void getScheduleList()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                rcintschBll bll = new rcintschBll();

                int total = 0;

                List<object> dataList = bll.GetSelectedRecords<object>(list, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(BuildAnonymousObject(dataList));
                Response.Write("{results:" + total + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void getNew()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                List<trcintsch> listDtl = JavaScriptConvert.DeserializeObject<List<trcintsch>>(ht["dtlparams"].ToString());

                rcintschBll bll = new rcintschBll();

                bll.CreateInterviewSchedule(listDtl);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getEdit()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                List<trcintsch> listDtl = JavaScriptConvert.DeserializeObject<List<trcintsch>>(ht["dtlparams"].ToString());

                rcintschBll bll = new rcintschBll();

                bll.UpdateInterviewSchedule(parameters, listDtl);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void exportExcelDtl()
        {
            getExportExcel<rcintschBll, object>();
        }

    }
}
