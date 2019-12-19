using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Training;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class trtrarstController : BaseAttendanceController
    {
        protected ActionResult getIndex(string viewName)
        {
            string tabId = this.Request["menuId"];
            string trcd = this.Request["trcd"];
            string tableName = this.GetType().Name.GetPageName();
            string currentUser = Function.GetCurrentUser();

            if (trcd == null) trcd = "";

            #region MUF
            var q = (from p in (List<string>)HttpContext.Session[Constant.SESSION_CURRENT_MUF]
                     where p == tabId.Substring(1, tabId.Length - 1)
                     select p).ToList();

            bool muf = q.Count > 0 ? true : false;
            #endregion

            ViewData["config"] = "{tabId:\"" + tabId + "\",trcd:\"" + trcd + "\",tableName:\"t" + tableName + "\",currentUser:\"" + currentUser + "\",muf:\"" + muf + "\"}";

            SetAuthorization(tabId);

            return this.View(viewName);

        }

        public ActionResult Index()
        {
            return getIndex("trtrarst");
        }

        public void list()
        {
            getList<trtrarstBll, object>();
        }

        public void New()
        {
            getEdit();
        }

        public void Edit()
        {
            getEdit();
        }

        public void Delete()
        {
            getDelete<ttrtrarst>();
        }

        public void exportExcel()
        {
            getExportExcel<trtrarstBll, object>();
        }

        public void getEdit()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                //tatroster obj = JavaScriptConvert.DeserializeObject<tatroster>(ht["params"].ToString());
                List<ttrtrarst> listDtl = JavaScriptConvert.DeserializeObject<List<ttrtrarst>>(ht["dtlparams"].ToString());

                trtrarstBll bll = new trtrarstBll();

                bll.UpdateTrainingResult(listDtl);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void clear()
        {
            string message = string.Empty;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["keycolumns"].ToString());

                trtrarstBll bll = new trtrarstBll();

                bll.Clear(list[0].ColumnValue);

                message = "{status:'success',msg:'Clear success.'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
            }
            Response.Output.Write(message);
        }

        public void getTrainingResult()
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                trtrarstBll bll = new trtrarstBll();

                List<object> dataList = bll.GetTrainingResult(list);
                string json = JavaScriptConvert.SerializeObject(BuildAnonymousObject(dataList));
                Response.Write("{results:" + dataList.Count.ToString() + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
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

                trtrarstBll bll = new trtrarstBll();

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

  
    }
}
