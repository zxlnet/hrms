using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using Newtonsoft.Json;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.HRMS;
using GotWell.Common;
using GotWell.Model.Authorization;
using System.Text;
using System.Collections;
using GotWell.HRMS.HRMSBusiness.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pshctvalController : BaseAttendanceController
    {
        public void exportExcel()
        {
            try
            {

                string record = this.Request["record"];
                pshctvalBll bll = new pshctvalBll();

                tpshctcfg cfg = bll.GetSelectedObject<tpshctcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "hccd", ColumnValue = record } });

                tstdefcfg xdef = bll.GetSelectedObject<tstdefcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "dfnm", ColumnValue = cfg.xdef } });

                tstdefcfg ydef = bll.GetSelectedObject<tstdefcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "dfnm", ColumnValue = cfg.ydef } });

                ValueInfoBll dd = new ValueInfoBll();
                List<ValueInfo> lstX = typeof(ValueInfoBll).GetMethod(xdef.dasc).Invoke(dd, new object[] { }) as List<ValueInfo>;
                List<ValueInfo> lstY = typeof(ValueInfoBll).GetMethod(ydef.dasc).Invoke(dd, new object[] { }) as List<ValueInfo>;

                List<List<ColumnInfo>> lstResult = bll.GetHeadCountCfgValue(record, cfg, lstX, lstY);

                if (lstResult != null)
                {
                    UtilExcel.ExportToExcel(Response, this.GetType().Name, lstResult);
                }
            }
            catch (Exception ex)
            {
                string message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_QueryFail, ex, true) + "'}";
                Response.Output.Write(message);
            }
        }

        public ActionResult Index()
        {
            return getIndex("pshctval");
        }

        public void Delete()
        {
            string message = "{}";
            try
            {
                string record = this.Request["record"];

                pshctvalBll bll = new pshctvalBll();
                bll.Delete(record);

                message = "{status:'success',msg:'" + HRMSRes.Public_Message_DeleteWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_DeleteBad, ex, true) + "'}";
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

                List<CoordinateInfo> listDtl = JavaScriptConvert.DeserializeObject<List<CoordinateInfo>>(ht["dtlparams"].ToString());

                pshctvalBll bll = new pshctvalBll();

                bll.Update(parameters, listDtl);
                message = "{status:'success',msg:'" + HRMSRes.Public_Message_EditWell + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(HRMSRes.Public_Message_EditBad, ex, true) + "'}";
            }
            Response.Write(message);
        }

        private string recordsJson(List<ValueInfo> lstX)
        {
            StringBuilder json = new StringBuilder();

            json.Append("{name:\"Item\"").Append("},");
            json.Append("{name:\"ItemValue\"").Append("},");
            for (int i = 0; i < lstX.Count; i++)
            {
                ValueInfo col = lstX[i];
                json.Append("{name:\"").Append(col.DisplayField).Append("\"").Append("},");
            }
            json.Append("{name:\"").Append("rfid").Append("\"}");
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

        private string columnsJson(List<ValueInfo> lstX)
        {
            StringBuilder json = new StringBuilder();

            //item
            json.Append("{").Append("header:'Item',")
                .Append("dataIndex:'Item',")
                .Append("sortable:true,").Append("width:200").Append("},");
            //item value
            json.Append("{").Append("header:'ItemValue',")
                .Append("dataIndex:'ItemValue',")
                .Append("hidden:true,")
                .Append("sortable:true").Append("},");

            for (int i = 0; i < lstX.Count; i++)
            {
                ValueInfo col = lstX[i];

                json.Append("{");
                json.Append("header:'").Append(col.DisplayField).Append("',")
                    .Append("dataIndex:'").Append(col.DisplayField).Append("',")
                    .Append("actualValue:'").Append(col.ValueField).Append("',")
                    .Append("align:'right',")
                    .Append("editor: new Ext.form.NumberField({allowBlank:false,allowNegative:false}),")
                    .Append("sortable:true").Append("");

                json.Append("}");

                if (i < (lstX.Count - 1))
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

        private string responseJson(string hccd,List<ValueInfo> lstX, List<ValueInfo> lstY)
        {
            StringBuilder json = new StringBuilder();

            string recordCfg = recordsJson(lstX);
            string columnCfg = columnsJson(lstX);

            json.Append("{")
                .Append("records:").Append(recordCfg).Append(",")
                .Append("columns:").Append(columnCfg).Append(",")
                .Append("hccd:\"").Append(hccd).Append("\"").Append("")
                .Append("}");

            return json.ToString();
        }

        public void load()
        {

            string record = this.Request["record"];
            pshctvalBll bll = new pshctvalBll();

            tpshctcfg cfg = bll.GetSelectedObject<tpshctcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "hccd", ColumnValue = record } 
            });

            tstdefcfg xdef = bll.GetSelectedObject<tstdefcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "dfnm", ColumnValue = cfg.xdef } 
            });

            tstdefcfg ydef = bll.GetSelectedObject<tstdefcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "dfnm", ColumnValue = cfg.ydef } 
            });

            ValueInfoBll dd = new ValueInfoBll();
            List<ValueInfo> lstX = typeof(ValueInfoBll).GetMethod(xdef.dasc).Invoke(dd, new object[] { }) as List<ValueInfo>;
            List<ValueInfo> lstY = typeof(ValueInfoBll).GetMethod(ydef.dasc).Invoke(dd, new object[] { }) as List<ValueInfo>;

            string message = "{status:'success',msg:'" + responseJson(record, lstX, lstY).Replace("'","\\'") + "'}";

            Response.Write(message);
        }

        public void list()
        {

            string record = this.Request["record"];
            pshctvalBll bll = new pshctvalBll();

            tpshctcfg cfg = bll.GetSelectedObject<tpshctcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "hccd", ColumnValue = record } 
            });

            tstdefcfg xdef = bll.GetSelectedObject<tstdefcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "dfnm", ColumnValue = cfg.xdef } 
            });

            tstdefcfg ydef = bll.GetSelectedObject<tstdefcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "dfnm", ColumnValue = cfg.ydef } 
            });

            ValueInfoBll dd = new ValueInfoBll();
            List<ValueInfo> lstX = typeof(ValueInfoBll).GetMethod(xdef.dasc).Invoke(dd, new object[] { }) as List<ValueInfo>;
            List<ValueInfo> lstY = typeof(ValueInfoBll).GetMethod(ydef.dasc).Invoke(dd, new object[] { }) as List<ValueInfo>;

            List<List<ColumnInfo>> lstResult = bll.GetHeadCountCfgValue(record, cfg, lstX, lstY);
            int total = lstResult.Count;

            string json = ConvertToJson(lstResult);
            Response.Write("{results:" + total + ",rows:" + json + "}");

            //ViewData["hctconfig"] = responseJson(lstResult,lstX,lstY);
        }

        private string ConvertToJson(List<List<ColumnInfo>> lstResult)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < lstResult.Count; i++)
            {
                if (i != 0) sb.Append(",");

                sb.Append("{");

                for (int j = 0; j < lstResult[i].Count; j++)
                {
                    if (j != 0) sb.Append(",");
                    sb.Append("\"" + lstResult[i][j].ColumnName + "\":\"" + lstResult[i][j].ColumnValue + "\"");
                }

                sb.Append("}");
            }

            return "[" + sb.ToString() + "]";
        }
    }
}
