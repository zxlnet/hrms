using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.Authorization;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Common;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class BaseController : Controller
    {
        public virtual ActionResult getIndex(string viewName)
        {
            return getIndex(viewName, string.Empty);
        }

        public virtual ActionResult getIndex(string viewName,string additionInfo)
        {
            string tabId = this.Request["menuId"];
            string emno = this.Request["emno"];
            string tableName = this.GetType().Name.GetPageName();
            string currentUser = Function.GetCurrentUser();

            if (emno == null) emno = "";

            #region MUF
            var q = (from p in (List<string>)HttpContext.Session[Constant.SESSION_CURRENT_MUF]
                     where p == tabId.Substring(1, tabId.Length - 1)
                     select p).ToList();

            bool muf = q.Count > 0 ? true : false;
            #endregion

            ViewData["config"] = "{tabId:\"" + tabId + "\",emno:\"" + emno + "\",tableName:\"t"
                                 + tableName + "\",currentUser:\"" + currentUser + "\",muf:\"" + muf + "\""
                                 + (additionInfo == string.Empty ? "" : ",") + additionInfo
                                 + "}";

            SetAuthorization(tabId);

            return this.View(viewName);

        }

        public virtual void SetAuthorization(string tabId)
        {
            StringBuilder auth = new StringBuilder();
            object obj = HttpContext.Session[Constant.SESSION_AUTHORIZATION];
            if (obj != null)
            {
                AuthorizationMdl authorization = (AuthorizationMdl)obj;
                string pageName = this.GetType().Name.GetPageName();

                string funId = pageName + "_" + tabId + "_add";
                bool isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_add:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_edit";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_edit:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_delete";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_delete:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_query";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_query:\"").Append(isValid).Append("\",");

                funId = pageName + "_" + tabId + "_exportexcel";
                isValid = authorization.checkPermissionByFuncUrl(funId);
                auth.Append(tabId).Append("_exportexcel:\"").Append(isValid).Append("\"");

            }

            ViewData["authorization"] = "{" + auth.ToString() + "}";
        }

        public virtual void getList<T, P>()
            where T : class
            where P : class
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                int start = Convert.ToInt32(this.Request["start"]);
                int limit = Convert.ToInt32(this.Request["limit"]);

                T bll = (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] {  });
                BaseBll baseBll = bll as BaseBll; 

                int total = 0;

                List<P> dataList = baseBll.GetSelectedRecords<P>(list, true, start, start + limit, ref total);
                string json = JavaScriptConvert.SerializeObject(BuildAnonymousObject(dataList));
                Response.Write("{results:" + total + ",rows:" + json + "}");

            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public virtual void getNew<T>() where T:class
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                T obj = JavaScriptConvert.DeserializeObject<T>(record);

                new BaseBll().DoInsert<T>(obj);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_AddWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_AddBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public virtual void getEdit<T>() where T : class
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                T obj = JavaScriptConvert.DeserializeObject<T>(record);

                List<ColumnInfo> lstParameters = new List<ColumnInfo>();
                lstParameters.Add(new ColumnInfo() { ColumnName = "emno", ColumnValue = typeof(T).GetProperty("emno").GetValue(obj, null).ToString() });

                if (typeof(T).GetProperty("sqno") != null)
                {
                    lstParameters.Add(new ColumnInfo() { ColumnName = "sqno", ColumnValue = typeof(T).GetProperty("sqno").GetValue(obj, null).ToString(), ColumnType = "int" });
                }

                new BaseBll().DoUpdate<T>(obj, lstParameters);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public virtual void getDelete<T>() where T : class
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                new BaseBll().DoDelete<T>(new List<ColumnInfo> { new ColumnInfo() { ColumnName = "emno", ColumnValue = ht["emno"].ToString() } ,
                                                                                new ColumnInfo() { ColumnName = "sqno", ColumnValue =  ht["sqno"].ToString(),ColumnType="int" }});
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        public void getDelete<T, P>()
            where T : class
            where P : class
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());

                T bll = (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
                BaseBll baseBll = bll as BaseBll;

                baseBll.DoDelete<T>(parameters);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }
            Response.Write(message);
        }


        public virtual void getExportExcel<T,P>() where T : class where P:class
        {
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);
                List<ColumnInfo> records = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["params"].ToString());
                List<ColumnInfo> headers = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(ht["headers"].ToString());

                T bll = (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
                BaseBll baseBll = bll as BaseBll;

                int total = 0;
                List<P> obj = baseBll.GetSelectedRecords<P>(records, false, 0, 0, ref total);


                if ((obj != null) && (total > 0))
                {
                    List<object> finalObj = BuildAnonymousObject(obj).Cast<object>().ToList();
                    UtilExcel.ExportToExcel(finalObj[0].GetType(), Response, this.GetType().Name.GetPageName(), headers, finalObj);
                }
            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }


        protected virtual IEnumerable BuildAnonymousObject<T>(List<T> _obj) where T : class
        {
            return _obj;
        }

        public virtual void getApplyTo<T, P>()
            where T : class
            where P : class
        {
            string message;
            try
            {
                string record = this.Request["record"];

                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                List<vw_employment> empParameters = JavaScriptConvert.DeserializeObject<List<vw_employment>>(ht["empparams"].ToString());
                P obj = JavaScriptConvert.DeserializeObject<P>(ht["objparams"].ToString());

                T bll = (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new object[] { });
                BaseBll baseBll = bll as BaseBll;

                baseBll.ApplyTo(empParameters, obj);

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
