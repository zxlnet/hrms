using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using GotWell.Common;
using GotWell.Model.Authorization;
using GotWell.Utility;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using System.Collections;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSBusiness.Common;
using System.Data;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class tracerController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                string tabId = this.Request["menuId"];
                string fileName = this.Request["fileName"];
                if (fileName == null)
                {
                    fileName = string.Empty;
                }
                ViewData["config"] = "{tabId:\"" + tabId + "\",fileName:\"" + fileName + "\"}";
                return this.View("tracer");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void showContent()
        { 
            string message = string.Empty;
            try
            {
                string fileName = this.Request["fileName"];
                if (fileName != null && !fileName.Equals(string.Empty))
                {
                    HttpContext.Session["traceJson"] = null;
                }
                object obj = HttpContext.Session["traceJson"];
                List<Dictionary<string, string>> list = null;
                if (obj == null)
                {
                    if (fileName.StartsWith("SP_"))
                    {
                        list = this.readFromDatabase(fileName);
                    }
                    else
                    {
                        list = this.readFromFile(fileName);
                    }
                }
                else
                {
                    list = obj as List<Dictionary<string, string>>;
                }                
                int start = 0;
                int limit = list.Count;

                string strStart = this.Request["start"];
                string strLimit = this.Request["limit"];
                if (strStart != null)
                {
                    start = Convert.ToInt16(strStart);
                    limit = Convert.ToInt16(strLimit);
                }

                List<Dictionary<string, string>> pagingList = list.GetRange(start,Math.Min(limit,list.Count-start));
                message = "{results:" + list.Count + ",rows:" + JavaScriptConvert.SerializeObject(pagingList) + "}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";                
            }
            Response.Output.Write(message);
        }

        public void exportExcel()
        {
            try
            {
                string header = this.Request["header"];
                List<ColumnInfo> headers = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(header);
                object obj = HttpContext.Session["traceJson"];
                if (obj != null)
                {
                    List<Dictionary<string, string>> list = obj as List<Dictionary<string,string>>;
                    UtilExcel.ExportToExcel(Response, this.GetType().Name.GetPageName(), headers, list);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void getFileList()
        {
            string message = "{}";
            try
            {
                string fileName = this.Request["fileName"];
                if (fileName != null && !fileName.Equals(string.Empty))
                {
                    HttpContext.Session["traceJson"] = null;
                }
                object obj = HttpContext.Session["traceJson"];
                List<Dictionary<string, string>> list = null;
                if (obj == null)
                {
                    list = UtilFile.GetFiles("");
                    //get file name from database,for stored procedure trace
                    //DataSet dsTraceNames = new tracerBll().GetTraceName();
                    //list.AddRange(this.convertDataSet(dsTraceNames));
                    HttpContext.Session["traceJson"] = list;
                }
                else
                {
                    list = obj as List<Dictionary<string, string>>;
                }
                int start = 0;
                int limit = list.Count;

                string strStart = this.Request["start"];
                string strLimit = this.Request["limit"];
                if (strStart != null)
                {
                    start = Convert.ToInt16(strStart);
                    limit = Convert.ToInt16(strLimit);
                }

                List<Dictionary<string, string>> pagingList = list.GetRange(start, Math.Min(limit, list.Count - start));
                message = "{results:" + list.Count + ",rows:" + JavaScriptConvert.SerializeObject(pagingList) + "}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";                 
            }
            Response.Output.Write(message);
        }

        private List<Dictionary<string, string>> readFromDatabase(string traceName)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string,string>>();

            //DataTable dt = new tracerBll().GetTraceData(traceName).Tables[0];
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    DataRow row = dt.Rows[i];
            //    string field = row["trace_field"].ToString();
            //    string value = row["trace_value"].ToString();
            //    string[] fieldNames = field.Split(new string[] { Constant.SPLITDATAFIELD }, StringSplitOptions.None);
            //    string[] fieldValues = value.Split(new string[] { Constant.SPLITDATAFIELD }, StringSplitOptions.None);
            //    Dictionary<string, string> dic = new Dictionary<string, string>();
            //    for (int j = 0; j < fieldNames.Length;j++ )
            //    {
            //        dic.Add(fieldNames[j], fieldValues[j]);
            //    }
            //    list.Add(dic);
            //}
            //HttpContext.Session["traceJson"] = list;
            return list;
        }

        private List<Dictionary<string, string>> readFromFile(string fileName)
        {
            List<Dictionary<string, string>> list = UtilFile.GetJsonFromFile("", fileName);
            HttpContext.Session["traceJson"] = list;
            return list;
        }

        private List<Dictionary<string, string>> convertDataSet(DataSet ds)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];                
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    DataRow row = dt.Rows[i];
                    dic.Add("fileName", row["trace_name"].ToString());
                    dic.Add("description", row["trace_desc"].ToString());
                    list.Add(dic);
                }
            }
            return list;
        }
    }
}
