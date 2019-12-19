using System;
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
    public class prpubrstController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("prpubrst");
        }

        public void list()
        {
            getList<prpubrstBll, tprpubrst>();
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
            getExportExcel<prpubrstBll, tprpubrst>();
        }

        public void exportexceldtl()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> records = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());
                List<ColumnInfo> headers = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["headers"].ToString());

                prpubrstBll bll = new prpubrstBll();

                int total = 0;
                List<object> obj = bll.GetPubRuleSetDtl(records, false, 0, 0, ref total);


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

        public void getPubRuleSetDtl()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                prpubrstBll bll = new prpubrstBll();

                int total = 0;

                List<object> dataList = bll.GetPubRuleSetDtl(list, true, start, start + limit, ref total);
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

                tprpubrst obj = JavaScriptConvert.DeserializeObject<tprpubrst>(ht["params"].ToString());
                List<tprprsdtl> listDtl = JavaScriptConvert.DeserializeObject<List<tprprsdtl>>(ht["dtlparams"].ToString());

                prpubrstBll bll = new prpubrstBll();

                bll.InsertPubRuleSet(obj, listDtl);

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

                tprpubrst obj = JavaScriptConvert.DeserializeObject<tprpubrst>(ht["params"].ToString());
                List<tprprsdtl> listDtl = JavaScriptConvert.DeserializeObject<List<tprprsdtl>>(ht["dtlparams"].ToString());

                prpubrstBll bll = new prpubrstBll();

                bll.UpdatePubRuleSet(obj, listDtl);
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

                new prpubrstBll().DeletePubRuleSet(parameters);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getRelItem()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                prpubrstBll bll = new prpubrstBll();

                List<object> dataList = bll.GetRelItem(parameters[0].ColumnValue, parameters[1].ColumnValue);
                string json = JavaScriptConvert.SerializeObject(dataList);
                message = "{results:" + dataList.Count + ",rows:" + json + "}";
                
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
            }
            Response.Write(message);
        }

    }
}