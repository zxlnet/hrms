using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Leave;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class lvleaappController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            //return getIndex("lvleaapp");
            string atdt = this.Request["atdt"];
            string info = "lvdt:\"" + atdt + "\"";
            return getIndex("lvleaapp", info);

        }

        public void list()
        {
            getList<lvleaappBll, object>();
        }

        public void New()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                tlvleaapp obj = JavaScriptConvert.DeserializeObject<tlvleaapp>(ht["params"].ToString());

                //new BaseBll().DoInsert<T>(obj);

                lvleaappBll bll = new lvleaappBll();
                bll.SaveLeaveApplication(obj);

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

                tlvleaapp obj = JavaScriptConvert.DeserializeObject<tlvleaapp>(ht["params"].ToString());

                //new BaseBll().DoInsert<T>(obj);

                lvleaappBll bll = new lvleaappBll();
                bll.UpdateLeaveApplication(obj);

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

                lvleaappBll bll = new lvleaappBll();
                bll.DeleteLeaveApplication(parameters);
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
            getExportExcel<lvleaappBll, object>();
        }

        public void getNewAppNo()
        {

            string message = string.Empty;
            try
            {

                lvleaappBll bll = new lvleaappBll();
                string nextNo = bll.getNewAppNo();

                message = "{status:'success',msg:'"+ nextNo + "'}";

            }
            catch (Exception ex)
            {
                message = "{status:'fail',msg:'" + ex.Message + "'}"; ;
            }

            Response.Output.Write(message);
        }

        public void getEmpLeaveSettings()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                lvleaappBll bll = new lvleaappBll();

                LvSettingInfo lstSettings = bll.GetEmpLeaveSettings(ht["emno"].ToString(), ht["ltcd"].ToString(), Convert.ToDateTime(ht["fromdate"].ToString()));

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

        public void CalcLeaveTime()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                tlvleaapp obj = new tlvleaapp();
                obj.emno = ht["emno"].ToString();
                obj.frtm = Convert.ToDateTime(ht["fromdate"].ToString());
                obj.totm = Convert.ToDateTime(ht["todate"].ToString());

                lvleaappBll bll = new lvleaappBll();
                string json = bll.CalcLeaveTime(obj);

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
