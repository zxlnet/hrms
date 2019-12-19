using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psskillController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("psskill");
        }

        public void list()
        {
            getList<psskillBll, object>();
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
            getExportExcel<psskillBll, object>();
        }

        public void getSkillItems()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                psskillBll bll = new psskillBll();

                int total = 0;

                List<object> dataList = bll.GetSkillItems(list, true, start, start + limit, ref total);
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

                tpsskill obj = JavaScriptConvert.DeserializeObject<tpsskill>(ht["params"].ToString());
                List<tpsskiitm> listDtl = JavaScriptConvert.DeserializeObject<List<tpsskiitm>>(ht["dtlparams"].ToString());

                psskillBll bll = new psskillBll();

                bll.InsertSkill(obj, listDtl);

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

                tpsskill obj = JavaScriptConvert.DeserializeObject<tpsskill>(ht["params"].ToString());
                List<tpsskiitm> listDtl = JavaScriptConvert.DeserializeObject<List<tpsskiitm>>(ht["dtlparams"].ToString());

                psskillBll bll = new psskillBll();

                bll.UpdateSkill(obj, listDtl);
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

               List<ColumnInfo> parameters =new List<ColumnInfo> { new ColumnInfo() { ColumnName = "emno", ColumnValue = ht["emno"].ToString() } ,
                                                                                new ColumnInfo() { ColumnName = "sqno", ColumnValue =  ht["sqno"].ToString(),ColumnType="int" }};
                
                new psskillBll().DeleteSkill(parameters);
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