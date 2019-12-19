using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;
using System.Collections;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using Newtonsoft.Json;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atrosterController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atroster");
        }

        public void list()
        {
            getList<atrosterBll, tatroster>();
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
            getDelete<tatroster>();
        }

        public void exportExcel()
        {
            getExportExcel<atrosterBll, tatroster>();
        }

        public void getRosterDtl()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                atrosterBll bll = new atrosterBll();

                int total = 0;

                List<object> dataList = bll.GetRosterDetails(list, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(dataList);
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

                tatroster obj = JavaScriptConvert.DeserializeObject<tatroster>(ht["params"].ToString());
                List<tatrosdtl> listDtl = JavaScriptConvert.DeserializeObject<List<tatrosdtl>>(ht["dtlparams"].ToString());

                atrosterBll bll = new atrosterBll();

                bll.InsertRoster(obj, listDtl);

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

                tatroster obj = JavaScriptConvert.DeserializeObject<tatroster>(ht["params"].ToString());
                List<tatrosdtl> listDtl = JavaScriptConvert.DeserializeObject<List<tatrosdtl>>(ht["dtlparams"].ToString());

                atrosterBll bll = new atrosterBll();

                bll.UpdateRoster(obj, listDtl);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getDelete()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                new atrosterBll().DeleteRoster(parameters);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

    }
}
