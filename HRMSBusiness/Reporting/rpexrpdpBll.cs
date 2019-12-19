using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Reflection;
using System.Xml;
using System.Data;
using System.Data.Common;

namespace GotWell.HRMS.HRMSBusiness.Reporting
{
    public class rpexrpdpBll : BaseBll
    {
        #region personal report
        public DataSet GetPersonalData(string rpcd, List<trpexrpdd> lstDtl, List<ColumnInfo> lstParameters)
        {
            var tables = lstDtl.Select(p => p.tbnm).Distinct().ToList();

            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbTable = new StringBuilder();

            #region Build Select
            var columns = lstDtl.OrderBy(p => p.sqno).ToList();
            sbSql.Append("select ");
            for (int i = 0; i < columns.Count; i++)
            {
                if (i > 0)
                    sbSql.Append(",");

                sbSql.Append(columns[i].tbnm + "." + columns[i].itfd);
            }
            #endregion

            #region Build From
            sbSql.Append(" " + "from" + " ");
            for (int i = 0; i < tables.Count; i++)
            {
                sbSql.Append(tables[i].ToString());

                if (i < tables.Count - 1)
                    sbSql.Append(",");
            }
            #endregion

            #region Build Where
            sbSql.Append(" " + "where 1=1" + " ");
            for (int i = 0; i < lstParameters.Count; i++)
            {
                if (i == 0)
                    sbSql.Append(" " + "and" + " ");

                var q = (from p in lstDtl
                        where p.itfd == lstParameters[i].ColumnName
                        select p).ToList();

                if (q.Count > 0)
                {
                    switch (q[0].itty)
                    {
                        case "string":
                        case "datetime":
                            sbSql.Append("(" + q[0].tbnm + "." + q[0].itfd + "='" + lstParameters[i].ColumnValue.ToString() + "')");
                            break;
                        case "float":
                            sbSql.Append("(" + q[0].tbnm + "." + q[0].itfd + "=" + lstParameters[i].ColumnValue + ")");
                            break;
                    }
                }
            }
            #endregion

            #region Build Order
            sbSql.Append(" ");
            var orders = lstDtl.Where(p => p.isor == "Y").ToList();
            for (int i = 0; i < orders.Count; i++)
            {
                if (i == 0)
                    sbSql.Append(" " + "order by" + " ");

                sbSql.Append(orders[i].tbnm + "." + orders[i].itfd);
                if (i < orders.Count - 1)
                    sbSql.Append(",");
            }
            #endregion

            BaseBll bll = new BaseBll();
            //List< lstObj = bll.ExecuteRawSQLQueryy<object>(sbSql.ToString());
            DataSet ds = bll.ExecuteRawSQLQuery(sbSql.ToString());

            string strXML = ConvertToXML_Personal(ds, lstDtl);

            DataSet dsFin = new DataSet();

            XmlReader reader = XmlReader.Create(new System.IO.StringReader(strXML));
            dsFin.ReadXml(reader);

            return dsFin;
        }

        private string ConvertToXML_Personal(DataSet ds, List<trpexrpdd> lstDef)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Data>");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                sb.Append("<Row>");
                for (int j = 0; j < lstDef.Count; j++)
                {
                    string finm = lstDef[j].ittx == null ? "" : lstDef[j].ittx.Replace(" ", "");
                    finm = finm == "" ? lstDef[j].itnm.Replace(" ", "") : finm;

                    sb.Append("<" + finm + ">")
                        .Append(ds.Tables[0].Rows[i][lstDef[j].itfd].ToString())
                        .Append("</" + finm + ">");
                }
                sb.Append("</Row>");
            }
            sb.Append("</Data>");

            return sb.ToString();
        }
        #endregion

        #region Payroll report
        public DataSet GetPayrollData(string rpcd, List<trpexrpdd> lstDtl, List<ColumnInfo> lstParameters)
        {
            string sSql = BuildSql(rpcd, lstDtl, lstParameters);

            BaseBll bll = new BaseBll();
            DataSet ds = bll.ExecuteRawSQLQuery(sSql);

            string strXML = ConvertToXML_Payroll(ds, lstDtl);

            DataSet dsFin = new DataSet();

            XmlReader reader = XmlReader.Create(new System.IO.StringReader(strXML));
            dsFin.ReadXml(reader);

            return dsFin;
        }

        private string ConvertToXML_Payroll(DataSet ds, List<trpexrpdd> lstDef)
        {
            StringBuilder sb = new StringBuilder();

            #region Salary Report
            var q1 = (from p in ds.Tables[0].AsEnumerable()
                      select new
                      {
                          emno = p.Field<string>("emno")
                      }).Distinct().ToList();

            sb.Append("<Data>");

            for (int n = 0; n < q1.Count; n++)
            {
                sb.Append("<Row>");
                DataRow[] drs = ds.Tables[0].Select("emno='" + q1[n].emno + "' ");

                for (int j = 0; j < lstDef.Count; j++)
                {
                    string finm = lstDef[j].ittx == null ? "" : lstDef[j].ittx.Replace(" ", "");
                    finm = finm == "" ? lstDef[j].itnm.Replace(" ", "") : finm;

                    if (lstDef[j].tbnm != "salitm")
                    {
                        sb.Append("<" + finm + ">")
                            .Append(drs[0][lstDef[j].itfd].ToString())
                            .Append("</" + finm + ">");
                    }

                    if (lstDef[j].tbnm == "salitm")
                    {
                        DataRow[] drs1 = ds.Tables[0].Select("emno='" + q1[n].emno + "' and itcd='" + lstDef[j].itfd + "'");
                        if (drs1.Length > 0)
                        {
                            sb.Append("<" + finm + ">")
                                .Append(drs1[0]["valu"].ToString())
                                .Append("</" + finm + ">");
                        }
                        else
                        {
                            sb.Append("<" + finm + ">")
                                .Append("0.00")
                                .Append("</" + finm + ">");
                        }
                    }
                }

                sb.Append("</Row>");
            }
            sb.Append("</Data>");
            #endregion

            return sb.ToString();
        }

        #endregion

        #region public func
        private string BuildSql(string rpcd, List<trpexrpdd> lstDtl, List<ColumnInfo> lstParameters)
        {
            var tables = lstDtl.Where(p => p.tbnm != "salitm").Select(p => p.tbnm).Distinct().ToList();

            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbTable = new StringBuilder();

            #region Build Select
            var columns = lstDtl.Where(p => p.tbnm == "vw_employment_rpt").OrderBy(p => p.sqno).ToList();
            sbSql.Append("select ");
            for (int i = 0; i < columns.Count; i++)
            {
                if (i > 0)
                    sbSql.Append(",");

                sbSql.Append(columns[i].tbnm + "." + columns[i].itfd);
            }

            for (int i = 0; i < tables.Count; i++)
            {
                if (tables[i] != "vw_employment_rpt")
                {
                    sbSql.Append("," + tables[i] + ".* ");
                }
            }
            #endregion

            #region Build From
            sbSql.Append(" " + "from" + " ");
            for (int i = 0; i < tables.Count; i++)
            {
                sbSql.Append(tables[i].ToString());

                if (i < tables.Count - 1)
                    sbSql.Append(",");
            }
            #endregion

            #region Build Where
            sbSql.Append(" " + "where 1=1" + " ");
            if (tables.Count > 1)
            {
                //如果是多表，根据emno进行关联
                for (int i = 1; i < tables.Count; i++)
                {
                    sbSql.Append(" and (" + tables[0] + ".emno=" + tables[i] + ".emno" + ")");
                }
            }

            for (int i = 0; i < lstParameters.Count; i++)
            {
                if (i == 0)
                    sbSql.Append(" " + "and" + " ");

                var q = (from p in lstDtl
                         where p.itfd == lstParameters[i].ColumnName
                         select p).ToList();

                if (q.Count > 0)
                {
                    switch (q[0].itty)
                    {
                        case "string":
                        case "datetime":
                            sbSql.Append("(" + q[0].tbnm + "." + q[0].itfd + "='" + lstParameters[i].ColumnValue.ToString() + "')");
                            break;
                        case "float":
                            sbSql.Append("(" + q[0].tbnm + "." + q[0].itfd + "=" + lstParameters[i].ColumnValue + ")");
                            break;
                    }
                }
            }
            #endregion

            #region Build Order
            sbSql.Append(" ");
            var orders = lstDtl.Where(p => p.isor == "Y").ToList();
            for (int i = 0; i < orders.Count; i++)
            {
                if (i == 0)
                    sbSql.Append(" " + "order by" + " ");

                sbSql.Append(orders[i].tbnm + "." + orders[i].itfd);
                if (i < orders.Count - 1)
                    sbSql.Append(",");
            }
            #endregion

            return sbSql.ToString();
        }
        
        private string ConvertToXML<T>(List<T> lstObj, List<trpexrpdd> lstDef) where T : class
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Data>");
            for (int i = 0; i < lstObj.Count; i++)
            {
                sb.Append("<Row>");
                for (int j = 0; j < lstDef.Count; j++)
                {
                    sb.Append("<" + lstDef[j].itnm  + ">")
                        .Append(typeof(T).GetType().GetProperty(lstDef[j].itfd).GetValue(lstObj[i],null).ToString())
                        .Append("</" + lstDef[j].itnm + ">");
                }
                sb.Append("</Row>");
            }
            sb.Append("</Data>");

            return sb.ToString();
        }

        #endregion


        #region Payroll Bank Alloc report
        public DataSet GetPayrollBankAllocData(string rpcd, List<trpexrpdd> lstDtl, List<ColumnInfo> lstParameters)
        {

            string sSql = BuildSql(rpcd, lstDtl, lstParameters);

            BaseBll bll = new BaseBll();
            DataSet ds = bll.ExecuteRawSQLQuery(sSql);

            string strXML = ConvertToXML_Personal(ds, lstDtl);

            DataSet dsFin = new DataSet();

            XmlReader reader = XmlReader.Create(new System.IO.StringReader(strXML));
            dsFin.ReadXml(reader);

            return dsFin;
        }
        #endregion

    }
}
