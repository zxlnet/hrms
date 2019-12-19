using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Reporting;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using System.Text;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class rpexrpdpController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("rpexrpdp");
        }

        private string columnsJson(List<trpexrpdd> lstDtl)
        {
            StringBuilder json = new StringBuilder();

            for (int i = 0; i < lstDtl.Count; i++)
            {
                trpexrpdd col = lstDtl[i];
                string colName = col.itrs == null ? col.itnm : (col.itrs.Trim() == string.Empty ? col.itnm : HRMSRes.ResourceManager.GetString(col.itrs));
                string align = "left";
                switch (col.itty)
                {
                    case "datetime":
                        align = "center";
                        break;
                    case "float":
                    case "double":
                    case "int":
                    case "decimal":
                        align = "right";
                        break;
                    default:
                        align = "left";
                        break;
                }

                if (col.itty == "datetime" || col.itty == "date" || col.itty == "time")
                {
                    //from
                    json.Append("{");
                    json.Append("header:'").Append(colName).Append(HRMSRes.ResourceManager.GetString("Public_Label_From")).Append("',")
                        .Append("dataIndex:'from|").Append(col.itfd).Append("',")
                        .Append("align:'").Append(align).Append("',")
                        .Append("type:'").Append(col.itty).Append("',")
                        .Append("table:'").Append(col.tbnm).Append("',")
                        .Append("sortable:true").Append("");
                    json.Append("},");

                    //to
                    json.Append("{");
                    json.Append("header:'").Append(colName).Append(HRMSRes.ResourceManager.GetString("Public_Label_To")).Append("',")
                        .Append("dataIndex:'to|").Append(col.itfd).Append("',")
                        .Append("align:'").Append(align).Append("',")
                        .Append("type:'").Append(col.itty).Append("',")
                        .Append("table:'").Append(col.tbnm).Append("',")
                        .Append("sortable:true").Append("");
                    json.Append("}");
                }
                else
                {
                    json.Append("{");
                    json.Append("header:'").Append(colName).Append("',")
                        .Append("dataIndex:'").Append(col.itfd).Append("',")
                        .Append("align:'").Append(align).Append("',")
                        .Append("type:'").Append(col.itty).Append("',")
                        .Append("table:'").Append(col.tbnm).Append("',")
                        .Append("sortable:true").Append("");
                    json.Append("}");
                }

                if (i < (lstDtl.Count - 1))
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

        private string responseJson(trpexrpdf def, List<trpexrpdd> lstDtl)
        {
            StringBuilder json = new StringBuilder();

            string columnCfg = columnsJson(lstDtl);

            json.Append("{")
                .Append("columns:").Append(columnCfg).Append(",")
                .Append("rpcd:\"").Append(def.rpcd).Append("\"").Append("")
                .Append("}");

            return json.ToString();
        }

        public void loadReportDef()
        {

            string record = this.Request["record"]; //rpcd
            rpexrpdpBll bll = new rpexrpdpBll();

            trpexrpdf def = bll.GetSelectedObject<trpexrpdf>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "rpcd", ColumnValue = record } 
            });

            List<trpexrpdd> lstDtl = bll.GetSelectedRecords<trpexrpdd>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "rpcd", ColumnValue = record }, 
                new ColumnInfo() { ColumnName = "isqr", ColumnValue = "Y" }
            });

            string message = "{status:'success',msg:'" + responseJson(def, lstDtl).Replace("'", "\\'") + "'}";

            Response.Write(message);
        }


    }
}
