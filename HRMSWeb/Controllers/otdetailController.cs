using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using System.Collections;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSBusiness.Overtime;
using GotWell.HRMS.HRMSBusiness.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class otdetailController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            //return getIndex("otdetail");
            string atdt = this.Request["atdt"];
            string info = "otdt:\"" + atdt + "\"";
            return getIndex("otdetail", info);

        }

        public void list()
        {
            getList<otdetailBll, object>();
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
            getExportExcel<otdetailBll, object>();
        }


        public void getNew()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                totdetail obj = JavaScriptConvert.DeserializeObject<totdetail>(ht["params"].ToString());

                otdetailBll bll = new otdetailBll();
                bll.SaveOTDetail(obj);
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
                totdetail obj = JavaScriptConvert.DeserializeObject<totdetail>(ht["params"].ToString());

                otdetailBll bll = new otdetailBll();
                //bll.DoUpdate<T>(obj, parameters);
                bll.UpdateOTDetail(obj, parameters);
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

                otdetailBll bll = new otdetailBll();
                //new BaseBll().DoDelete<totdetail>(parameters);
                bll.DeleteOTDetail(parameters);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void applyto()
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<vw_employment> empParameters = JavaScriptConvert.DeserializeObject<List<vw_employment>>(ht["empparams"].ToString());
                totdetail obj = JavaScriptConvert.DeserializeObject<totdetail>(ht["objparams"].ToString());

                otdetailBll bll = new otdetailBll();

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
