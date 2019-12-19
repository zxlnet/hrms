using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Master;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.Authorization;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using Newtonsoft.Json;
using GotWell.Model.HRMS;
using System.Reflection;
using System.Linq;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class MasterDataController : Controller
    {
        private string table_name = string.Empty;
        public void exportExcel()
        {
            try
            {
                table_name = this.Request["tableName"];
                string record = this.Request["record"];
                string header = this.Request["header"];

                List<ColumnInfo> headers = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(header);

                List<object> dataList = null;
                List<ColumnInfo> parameters = new List<ColumnInfo>();

                MasterDataBll bll = new MasterDataBll();

                if (record != null)
                {
                    parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(record);
                }

                dataList = bll.GetSelectedRecords(table_name, parameters);

                if (dataList != null)
                {

                    UtilExcel.ExportToExcel(bll.GetDynamicType(table_name), Response, table_name, headers, dataList);
                }
            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
            
        }

        public void importExcel()
        {
            string message = "{}";
            try
            {
                table_name = this.Request["tableName"];
                string cols = this.Request["cols"];

                HttpPostedFileBase file = null;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    file = Request.Files[i];
                }

                if (file != null)
                {
                    string fileFullName = file.FileName;
                    string fileExtension = Path.GetExtension(fileFullName).ToLower();

                    if (fileExtension.ToLower().Equals(".xls"))
                    {
                        string tempPath = Path.GetTempPath();
                        string fileName = Path.GetFileName(fileFullName);
                        string savedFileFullName = tempPath + fileName;
                        file.SaveAs(savedFileFullName);

                        MasterDataBll bll = new MasterDataBll(table_name);
                        string excelDataSource = ConfigReader.getDBConnectionString_Excel().Replace("File.xls", savedFileFullName);
                        
                        List<ColumnInfo> list = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(cols);
                        bll.InsertFromExcel(table_name,excelDataSource, list);
                        
                        message = "{success:true, status:'success',msg:'" + HRMSRes.Public_Message_ImportWell + "'}";
                        FileInfo fileInfo = new FileInfo(savedFileFullName);
                        fileInfo.Delete();
                    }
                    else
                    {
                        throw new Exception(HRMSRes.Public_Message_FileNotEmpty);
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                message = "{errors:'',status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_ImportBad, ex, true) + ")'}";
            }
            Response.Output.Write(message);
        }

        public ActionResult index()
        {
            try
            {
                table_name = this.Request["tableName"];
                string tabId = this.Request["menuId"];

                List<ColumnMdl> columns = this.getColumns(table_name);

                MasterDataBll bll = new MasterDataBll(table_name);
                bool isLocked = false;

                #region MUF
                var q = (from p in (List<string>)HttpContext.Session[Constant.SESSION_CURRENT_MUF]
                         where p == tabId.Substring(1, tabId.Length - 1)
                         select p).ToList();

                bool muf = q.Count > 0 ? true : false;
                #endregion

                ViewData["config"] = responseJson(columns, tabId, table_name, isLocked, muf);

                StringBuilder auth = new StringBuilder();
                object obj = HttpContext.Session[Constant.SESSION_AUTHORIZATION];
                if (obj != null)
                {
                    AuthorizationMdl authorization = (AuthorizationMdl)obj;
                    string pageName = this.GetType().Name.GetPageName();

                    string funId = pageName + "_" + tabId + "_masterdata_add";
                    bool isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_masterdata_add:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_masterdata_edit";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_masterdata_edit:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_masterdata_delete";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_masterdata_delete:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_masterdata_export";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_masterdata_export:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_masterdata_import";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_masterdata_import:\"").Append(isValid).Append("\",");

                    funId = pageName + "_" + tabId + "_masterdata_query";
                    isValid = authorization.checkPermissionByFuncUrl(funId);
                    auth.Append(tabId).Append("_masterdata_query:\"").Append(isValid).Append("\"");
                }

                ViewData["authorization"] = "{" + auth.ToString() + "}";
            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
            return this.View("MasterData");
        }

        public void list()
        {
            try
            {
                string startStr = this.Request["start"];
                int start = 0;
                if (startStr != null)
                {
                    start=Convert.ToInt32(startStr);
                }

                string limitStr = this.Request["limit"];
                int limit = 0;
                if (limitStr != null)
                {
                    limit = Convert.ToInt32(limitStr);
                }

                table_name = this.Request["tableName"];
                string record = this.Request["record"];
                int total = 0;

                MasterDataBll bll = new MasterDataBll();

                object dataList = null;
                List<ColumnInfo> parameters = new List<ColumnInfo>();
                if (record != null)
                {
                    parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(record);
                }

                dataList = bll.GetSelectedRecords(table_name, parameters, true, start, start + limit, ref total);

                string json = JavaScriptConvert.SerializeObject(dataList);
                Response.Write("{results:" + total + ",rows:" + json + "}");
            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public void create()
        {
            string msg = "{}";
            try
            {
                table_name = this.Request["tableName"];
                string record = Request.Form["record"];

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(record);

                new MasterDataBll().InsertMasterData(table_name, parameters);
                
                msg = "{status:'success',msg:'" + HRMSRes.Public_Message_AddWell + "'}";
            }
            catch (UtilException ex)
            {
                if (ex.Code == -1001)
                {
                    msg = "{status:'failure',msg:'" + HRMSRes.Public_Message_AddBad + "',code:'" + Exception_ErrorMessage.IsDuplicate.ToString() + "'}";
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                msg = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_AddBad, ex, true) + "'}";
            }

            Response.Output.Write(msg);
        }

        public void edit()
        {
            string msg = "{}";
            try
            {
                table_name = this.Request["tableName"];
                string record = Request.Form["record"];

                List<ColumnInfo> parameters = JavaScriptConvert.DeserializeObject<List<ColumnInfo>>(record);
                
                new MasterDataBll().EditMasterData(table_name, parameters );

                msg = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                msg = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }

            Response.Output.Write(msg);
        }

        public void delete()
        {
            string msg = "{}";
            try
            {
                table_name = this.Request["tableName"];
                string record = this.Request["record"];

                List<List<ColumnInfo>> parameters = JavaScriptConvert.DeserializeObject<List<List<ColumnInfo>>>(record);
                
                new MasterDataBll().DeleteMasterData(table_name, parameters);
                msg = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                msg = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
            }

            Response.Output.Write(msg);
        }


        private string recordsJson(List<ColumnMdl> columns)
        {
            StringBuilder json = new StringBuilder();
            ColumnMdl mdl = null;

            for (int i = 0; i < columns.Count; i++)
            {
                mdl = columns[i];
                json.Append("{name:\"").Append(mdl.ColumnName).Append("\"").Append("},");
            }

            json.Append("{name:\"").Append("rw").Append("\"},");
            json.Append("{name:\"").Append("rfid").Append("\"}");
            if (json.Length > 0)
            {                
                json.Insert(0,"[");
                json.Append("]");
            }
            else
            {
                json.Append("[]");
            }

            return json.ToString();
        }

        private string columnsJson(List<ColumnMdl> columns)
        {
            StringBuilder json = new StringBuilder();
            for (int i = 0; i < columns.Count; i++)
            {
                ColumnMdl mdl = columns[i];
                string isRequired = mdl.IsRequired.Equals(string.Empty) ? "False" : mdl.IsRequired.Equals(Public_Flag.Y.ToString()).ToString();
                string isPrimaryKey = mdl.IsPrimaryKey.Equals(string.Empty) ? "False" : mdl.IsPrimaryKey.Equals(Public_Flag.Y.ToString()).ToString();
                string resourceId = mdl.ResourceId.Equals(string.Empty) ? mdl.ColumnName : "HRMSRes." + mdl.ResourceId;
                string display = mdl.IsDisplay.Equals(string.Empty) ? "False" : mdl.IsDisplay.Equals(Public_Flag.Y.ToString()).ToString();
                string defaultValue = mdl.DefaultValue;
                string defaultValueFormula = mdl.DefaultValue;
                if (!defaultValue.Equals(string.Empty))
                {
                    if (defaultValue.Equals(Public_DefaultValue.sysuser.ToString()))
                    {
                        defaultValue = "ContextInfo.currentUser";
                    }
                    else if (defaultValue.Equals(Public_DefaultValue.sysdate.ToString()))
                    {
                        defaultValue = "\"" + UtilDatetime.FormateDateTime1(DateTime.Now) + "\"";
                    }
                    else
                    {
                        defaultValue = "\"" + defaultValue + "\"";
                    }
                }
                else
                {
                    defaultValue = "\""+defaultValue+"\"";
                }
    
                json.Append("{");
                json.Append("header:typeof ").Append(resourceId).Append(" ==\"undefined\"").Append("?\"").Append(mdl.ColumnName).Append("\":").Append(resourceId).Append(",")
                    .Append("sortable:true").Append(",")
                    .Append("isPk:\"").Append(mdl.IsPrimaryKey.Equals(Public_Flag.Y.ToString())).Append("\",")
                    .Append("required:\"").Append(isRequired).Append("\",")
                    .Append("type:\"").Append(mdl.DataType).Append("\",")
                    .Append("size:").Append(mdl.DataSize).Append(",")
                    .Append("precision:\"").Append(mdl.DataPrecision).Append("\",")
                    .Append("defaultValue:").Append(defaultValue).Append(",")
                    .Append("defaultValueFormula:\"").Append(defaultValueFormula).Append("\",")
                    .Append("isDisplay:\"").Append(display).Append("\",")
                    .Append("align:").Append(GetPostionByType(mdl.DataType)).Append(",")
                    .Append("dataIndex:\"").Append(mdl.ColumnName).Append("\"").Append(",");

                if (mdl.DataType == "datetime")
                {
                    switch (mdl.DefaultValue.ToUpper())
                    {
                        case "SYSDATE":
                        case "DATE TIME":
                        case "":
                        default:
                            json.Append("renderer:formatDate").Append(",");
                            break;
                        case "DATE ONLY":
                            json.Append("renderer:formatDateNoTime").Append(",");
                            break;
                    }

                    //if (mdl.DefaultValue == "sysdate")
                    //    json.Append("renderer:formatDate").Append(",");
                    //else
                    //{
                    //    if (mdl.DefaultValue.ToUpper() == "DATE ONLY")
                    //    {
                    //        json.Append("renderer:formatDateNoTime").Append(",");
                    //    }
                    //    else
                    //    {
                    //        if (mdl.DefaultValue == null || mdl.DefaultValue.Trim() == string.Empty)
                    //            json.Append("renderer:formatDate").Append(",");
                    //        else
                    //            json.Append("renderer:formatDateNoTime").Append(",");
                    //    }
                    //}
                }

                json.Append("controlType:\"").Append(mdl.ControlType).Append("\"").Append("");

                json.Append("}");

                if (i < columns.Count - 1)
                {
                    json.Append(",");
                }
            }
            if (json.Length > 0)
            {
                json.Insert(0, "[");
                json.Append("]");
            }
            else
            {
                json.Append("[]");
            }
            return json.ToString();
        }

        private string GetPostionByType(string type)
        {
            if (type.Equals("datetime"))
            {
                return "\"center\"";
            }
            else if (type.Equals("int") || type.Equals("float"))
            {
                return "\"right\"";
            }
            else
            {
                return "\"left\"";
            }
        }

        private string responseJson(List<ColumnMdl> columns,string tabId,string tableName,bool isLocked,bool muf)
        {
            StringBuilder json = new StringBuilder();

            string recordCfg = recordsJson(columns);
            string columnCfg = columnsJson(columns);

            json.Append("{")
                .Append("records:").Append(recordCfg).Append(",")
                .Append("columns:").Append(columnCfg).Append(",")
                .Append("tabId:\"").Append(tabId).Append("\"").Append(",")
                .Append("tableName:\"").Append(tableName).Append("\",")
                .Append("muf:\"").Append(muf).Append("\",")
                .Append("locked:\"").Append(isLocked).Append("\"")
                .Append("}");

            return json.ToString();
        }

        private List<ColumnMdl> getColumns(string tableName)
        {
            MasterDataBll bll = new MasterDataBll(tableName);

            List<ColumnMdl> columns = bll.GetColumns();
            return columns;
        }

    }
}
