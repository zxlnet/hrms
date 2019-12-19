﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Payroll;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prprirstController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("prprirst");
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

        public void exportexceldtl()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> records = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());
                List<ColumnInfo> headers = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["headers"].ToString());

                prprirstBll bll = new prprirstBll();

                int total = 0;
                List<object> obj = bll.GetSelectedRecords<object>(records, false, 0, 0, ref total);


                if ((obj != null) && (total > 0))
                {
                    List<object> finalObj = obj.Cast<object>().ToList();
                    UtilExcel.ExportToExcel(finalObj[0].GetType(), Response, this.GetType().Name.GetPageName(), headers, finalObj);
                }
            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void getPriRuleSetDtl()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                prprirstBll bll = new prprirstBll();

                int total = 0;

                List<object> dataList = bll.GetSelectedRecords<object>(list, true, start, start + limit, ref total);
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

                //tprpubrst obj = JavaScriptConvert.DeserializeObject<tprpubrst>(ht["params"].ToString());
                List<tprprirst> listDtl = JavaScriptConvert.DeserializeObject<List<tprprirst>>(ht["dtlparams"].ToString());

                prprirstBll bll = new prprirstBll();

                bll.InsertPriRuleSet(listDtl);

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

                string emno = ht["params"].ToString();//JavaScriptConvert.DeserializeObject<string>(ht["params"].ToString());
                List<tprprirst> listDtl = JavaScriptConvert.DeserializeObject<List<tprprirst>>(ht["dtlparams"].ToString());

                prprirstBll bll = new prprirstBll();

                bll.UpdatePriRuleSet(emno,listDtl);
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

                new prprirstBll().DeletePriRuleSet(parameters);
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
                List<tprprirst> obj = JavaScriptConvert.DeserializeObject<List<tprprirst>>(ht["objparams"].ToString());

                prprirstBll bll = new prprirstBll();

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