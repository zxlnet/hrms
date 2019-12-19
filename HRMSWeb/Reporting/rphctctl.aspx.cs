using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSBusiness.Common;
using System.Collections.Generic;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.HRMS.HRMSBusiness.Reporting;
using System.Xml;
using GotWell.Utility;
using System.IO;

namespace GotWell.HRMS.HRMSWeb.Reporting
{
    public partial class rphctctl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = this.Request.QueryString["action"].ToString();
            string hccd = this.Request.QueryString["hccd"].ToString();

            if (action == "query")
            {
                LoadData(hccd);
            }
            else
            {
                ExportToExcel(hccd);                
            }
        }

        private void LoadData(string hccd)
        {
            pshctcfgBll bll = new pshctcfgBll();

            tpshctcfg cfg = bll.GetSelectedObject<tpshctcfg>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "hccd", ColumnValue = hccd } 
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

            //Build Grid Header
            //BuildGridHeader(lstX);

            rphctctlBll rpbll = new rphctctlBll();
            string strXML = rpbll.GetData(hccd, cfg, xdef, ydef, lstX, lstY);

            DataSet ds = new DataSet();

            XmlReader reader = XmlReader.Create(new System.IO.StringReader(strXML));
            ds.ReadXml(reader);
            this.GridView1.DataSource = ds;
            this.GridView1.DataBind();

            //GroupRows(this.GridView1, 0);
            MergeGridView mergeGV = new MergeGridView();
            mergeGV.Merge(this.GridView1, 0);

        }

        private void BuildGridHeader(List<ValueInfo> lstX)
        {
            //Item
            BoundField field = new BoundField() { DataField = "item", HeaderText = "Item" };
            this.GridView1.Columns.Add(field);

            //Type
            field = new BoundField() { DataField = "type", HeaderText = "Type" };
            this.GridView1.Columns.Add(field);

            //X
            for (int i = 0; i < lstX.Count; i++)
            {
                ValueInfo val = lstX[i];
                BoundField f = new BoundField() { HeaderText = val.DisplayField, DataField = "F" + val.ValueField };
            }

            //Total

        }

        private List<Int32> lstTotal = new List<int>();
        private int summaryStartIndex = 2; //from 0

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = summaryStartIndex; i < e.Row.Cells.Count; i++)
                {
                    if (lstTotal.Count < e.Row.Cells.Count - summaryStartIndex)
                    {
                        Int32 total = Convert.ToInt32(e.Row.Cells[i].Text);
                        lstTotal.Add(total);
                    }
                    else
                    {
                        lstTotal[i - summaryStartIndex] += Convert.ToInt32(e.Row.Cells[i].Text);
                    }

                    e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
                }
            }
            //else if (e.Row.RowType == DataControlRowType.Footer)
            //{
            //    e.Row.Cells[0].Text = "Total";

            //    for (int i = summaryStartIndex; i < e.Row.Cells.Count; i++)
            //    {
            //        e.Row.Cells[i].Text = lstTotal[i - summaryStartIndex].ToString();
            //        e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Right;
            //    }
            //}

        }

        private void ExportToExcel(string hccd)
        {
            LoadData(hccd);
            UtilExcel.ExportToExcelDataOnly(Response,this.GridView1, this.GetType().Name);
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //base.VerifyRenderingInServerForm(control);
        }
    }
}
