using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using Newtonsoft.Json;
using System.Collections;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atdatmnuController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            string atdt = this.Request["atdt"];
            string info = "atdt:\"" + atdt + "\"";

            return getIndex("atdatmnu",info);
        }

        public void list()
        {
            getList<atdatmnuBll, object>();
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
            getDelete();
        }

        public void exportExcel()
        {
            getExportExcel<atdatmnuBll, object>();
        }

        public void getNew()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                tatdatmnu obj = JavaScriptConvert.DeserializeObject<tatdatmnu>(ht["params"].ToString());

                new atdatmnuBll().InsertDataManu(obj);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + ".You should do attendance analysis again.'}";
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
                tatdatmnu obj = JavaScriptConvert.DeserializeObject<tatdatmnu>(ht["params"].ToString());

                new atdatmnuBll().UpdateDataManu(obj);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + ".You should do attendance analysis again.'}";
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

                new atdatmnuBll().DeleteDataManu(parameters);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + ".You should do attendance analysis again.'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

    }
}
