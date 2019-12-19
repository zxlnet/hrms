using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Overtime;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class otaplctnController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            //return getIndex("otaplctn");
            string atdt = this.Request["atdt"];
            string info = "otdt:\"" + atdt + "\"";
            return getIndex("otaplctn", info);
        }

        public void list()
        {
            getList<otaplctnBll, object>();
        }

        public void New()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                totaplctn obj = JavaScriptConvert.DeserializeObject<totaplctn>(ht["params"].ToString());

                otaplctnBll bll = new otaplctnBll();
                bll.SaveOTApplication(obj);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void Edit()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                totaplctn obj = JavaScriptConvert.DeserializeObject<totaplctn>(ht["params"].ToString());

                //new BaseBll().DoInsert<T>(obj);

                otaplctnBll bll = new otaplctnBll();
                bll.UpdateOTApplication(obj);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void Delete()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                otaplctnBll bll = new otaplctnBll();
                bll.DeleteOTApplication(parameters);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void exportExcel()
        {
            getExportExcel<otaplctnBll, object>();
        }

        public void getNewAppNo()
        {

            string message = string.Empty;
            try
            {

                otaplctnBll bll = new otaplctnBll();
                string nextNo = bll.getNewAppNo();

                message = "{status:'success',msg:'" + nextNo + "'}";
                
            }
            catch (Exception ex)
            {
                message = "{status:'fail',msg:'" + ex.Message + "'}"; ;
            }

            //System.Threading.Thread.Sleep(5000);

            Response.Output.Write(message);
        }

        public void getEmpOTSettings()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                otaplctnBll bll = new otaplctnBll();

                LvSettingInfo lstSettings = bll.GetEmpOTSettings(ht["emno"].ToString(), ht["otcd"].ToString(), Convert.ToDateTime(ht["fromdate"].ToString()));

                string json = JavaScriptConvert.SerializeObject(lstSettings);

                message = "{status:'success',msg:'" + json + "'}";

                Response.Write(message);
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void CalcOTTime()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                totaplctn obj = new totaplctn();
                obj.emno = ht["emno"].ToString();
                obj.frtm = Convert.ToDateTime(ht["fromdate"].ToString());
                obj.totm = Convert.ToDateTime(ht["todate"].ToString());

                otaplctnBll bll = new otaplctnBll();
                string json = bll.CalcOTTime(obj);

                message = "{status:'success'," + json + "}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }
    }
}
