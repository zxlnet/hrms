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
using System.Collections.Generic;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSCore;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Reporting;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSWeb.Reporting
{
    public partial class rpexrpdp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<ColumnInfo> lstParameter = new List<ColumnInfo>();

            string rpcd = Request.QueryString["rpcd"].ToString();
            string action = Request.QueryString["action"].ToString();

            for (int i = 2; i < Request.QueryString.Count; i++)
            {
                ColumnInfo col = new ColumnInfo() { ColumnName = Request.QueryString.Keys[i], ColumnValue = Request.QueryString[i].ToString() };
                lstParameter.Add(col);
            }

            if (action == "query")
                LoadData(rpcd, lstParameter);
            else
                ExportToExcel(rpcd, lstParameter);
        }

        private void LoadData(string rpcd,List<ColumnInfo> lstParameters)
        {
            BaseBll bll = new BaseBll();

            trpexrpdf def = bll.GetSelectedObject<trpexrpdf>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "rpcd", ColumnValue = rpcd } 
            });

            List<trpexrpdd> lstDtl = bll.GetSelectedRecords<trpexrpdd>(new List<ColumnInfo>() { 
                new ColumnInfo() { ColumnName = "rpcd", ColumnValue = rpcd } 
            });

            rpexrpdpBll rpbll = new rpexrpdpBll();
            DataSet ds = null;

            switch (def.rpty)
            {
                case "Personnel":
                    ds = rpbll.GetPersonalData(rpcd, lstDtl, lstParameters);
                    break;
                case "Payroll":
                    ds = rpbll.GetPayrollData(rpcd, lstDtl, lstParameters);
                    break;
                case "PayrollBankAlloc":
                    ds = rpbll.GetPayrollBankAllocData(rpcd, lstDtl, lstParameters);
                    break;
            }

            if (ds.Tables.Count>0 && ds.Tables[0].Rows.Count > 0)
            {
                //Title
                this.GridView1.Caption = "<B>" + def.rptt + "</B>";

                this.GridView1.DataSource = ds;
                this.GridView1.DataBind();

                //Merge Column
                MergeGridView mergeGV = new MergeGridView();
                for (int i = 0; i < lstDtl.Count; i++)
                {
                    if (lstDtl[i].isme == "Y")
                        mergeGV.Merge(this.GridView1, i);
                }

                //Summary
                var q = (from p in lstDtl
                         where p.issu == "Y"
                         select p).ToList();

                for (int i = 0; i < q.Count; i++)
                {
                    //GridViewSummary sum = new GridViewSummary(q[i].itnm, SummaryOperation.Sum);
                }
            }
            else
            {
                
            }

        }

        private void ExportToExcel(string rpcd, List<ColumnInfo> lstParameters)
        {
                LoadData(rpcd,lstParameters);
                UtilExcel.ExportToExcelDataOnly(Response, this.GridView1, this.GetType().Name);
        }
    }
}
