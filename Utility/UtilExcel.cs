using System;
using System.Text;
using System.IO;
using System.Data;
using System.Linq;
using System.Web;
using GotWell.Model.Common;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI;


namespace GotWell.Utility
{

     public class UtilExcel
    {
        public static void ExportXLS(HttpResponseBase response, string myPageName, List<ColumnInfo> columns, DataSet ds)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\Excel\" + myPageName + ".xls";

            response.Clear();
            response.Buffer = true;
            response.Charset = "utf-8";
            response.AppendHeader("Content-Disposition", "attachment;filename=" + myPageName + ".xls");
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.ContentType = "application/ms-excel";

            ExcelWriter excel = new ExcelWriter(path);
            try
            {
                excel.BeginWrite();

                short row = 0;

                for (short k = 0; k < columns.Count; k++)
                {
                    excel.WriteString(row, k, columns[k].ColumnDisplayName);
                }

                DataTable dt = ds.Tables[0];

                for (short i = 0; i < dt.Rows.Count; i++)
                {
                    row++;
                    for (short j = 0; j < columns.Count; j++)
                    {
                        ColumnInfo column = columns[j];
                        string columnType = column.ColumnType;
                        string columnName = column.ColumnName;
                        object value = ds.Tables[0].Rows[i][columnName];

                        if (columnType != null && columnType.Equals("date"))
                        {
                            value = value.ToString().Split(new char[] { ' ' }, StringSplitOptions.None)[0];
                        }
                        excel.WriteString(row, j, value.ToString());
                    }
                }
            }
            finally
            {
                excel.EndWrite();
            }

            FileInfo file = new FileInfo(path);

            if (file.Exists)
            {
                response.WriteFile(path);
                response.Flush();
                file.Delete();
            }
        }

        public static void ExportToExcel<T>(HttpResponseBase response, string myPageName, List<ColumnInfo> columns, List<T> list)
        {
            string FileName = myPageName + ".xls";

            response.Clear();
            response.Buffer = true;
            response.Charset = "utf-8";
            response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            string ss = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.ContentType = "application/ms-excel";

            StringBuilder builder = new StringBuilder();
            builder.Append("<html><meta http-equiv='Content-Type' content='text/html; charset=utf-8'>")
                  .Append("<body><table width='100%' border='1'><tr bgcolor='gray' style='COLOR: white'>");

            for (int k = 0; k < columns.Count; k++)
            {
                builder.Append("<td align='center'>").Append(columns[k].ColumnDisplayName).Append("</td>");
            }
            builder.Append("</tr>");

            for (int i = 0; i < list.Count; i++)
            {
                //Dictionary<string, string> dic = list[i];
                T obj = list[i];
                builder.Append("<tr>");
                for (int j = 0; j < columns.Count; j++)
                {
                    ColumnInfo column = columns[j];
                    string columnType = column.ColumnType;
                    string columnName = column.ColumnName;
                    object tmpValue = typeof(T).GetProperty(columnName).GetValue(obj, null);
                    string value = tmpValue == null ? "" : tmpValue.ToString();

                    if (columnType != null && columnType.Equals("date"))
                    {
                        value = value.Split(new char[] { ' ' }, StringSplitOptions.None)[0];
                    }

                    builder.Append("<td>").Append(value.ToString()).Append("</td>");

                }
                builder.Append("</tr>");
            }
            builder.Append("</table></body></html>");
            response.Output.Write(builder.ToString());

        }

        public static void ExportToExcel(Type _type,HttpResponseBase response, string myPageName, List<ColumnInfo> columns, List<object> list)
        {
            string FileName = myPageName + ".xls";

            response.Clear();
            response.Buffer = true;
            response.Charset = "utf-8";
            response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            string ss = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.ContentType = "application/ms-excel";

            StringBuilder builder = new StringBuilder();
            builder.Append("<html><meta http-equiv='Content-Type' content='text/html; charset=utf-8'>")
                  .Append("<body><table width='100%' border='1'><tr bgcolor='gray' style='COLOR: white'>");

            for (int k = 0; k < columns.Count; k++)
            {
                builder.Append("<td align='center'>").Append(columns[k].ColumnDisplayName).Append("</td>");
            }
            builder.Append("</tr>");

            for (int i = 0; i < list.Count; i++)
            {
                //Dictionary<string, string> dic = list[i];
                object obj = list[i];
                builder.Append("<tr>");
                for (int j = 0; j < columns.Count; j++)
                {
                    ColumnInfo column = columns[j];
                    string columnType = column.ColumnType;
                    string columnName = column.ColumnName;
                    object tmpValue = _type.GetProperty(columnName).GetValue(obj, null);
                    string value = tmpValue == null ? "" : tmpValue.ToString();

                    if (columnType != null && columnType.Equals("date"))
                    {
                        value = value.Split(new char[] { ' ' }, StringSplitOptions.None)[0];
                    }

                    builder.Append("<td>").Append(value.ToString()).Append("</td>");

                }
                builder.Append("</tr>");
            }
            builder.Append("</table></body></html>");
            response.Output.Write(builder.ToString());
        }

        public static void ExportToExcel(HttpResponseBase response, string myPageName,List<List<ColumnInfo>> list)
        {
            string FileName = myPageName + ".xls";

            response.Clear();
            response.Buffer = true;
            response.Charset = "utf-8";
            response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            string ss = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.ContentType = "application/ms-excel";

            StringBuilder builder = new StringBuilder();
            builder.Append("<html><meta http-equiv='Content-Type' content='text/html; charset=utf-8'>")
                  .Append("<body><table width='100%' border='1'><tr bgcolor='gray' style='COLOR: white'>");

            for (int k = 0; k < list[0].Count; k++)
            {
                if (list[0][k].ColumnDisplayName != "Hidden")
                {
                    builder.Append("<td align='center'>").Append(list[0][k].ColumnName).Append("</td>");
                }
            }
            builder.Append("</tr>");

            for (int i = 0; i < list.Count; i++)
            {
                builder.Append("<tr>");
                for (int j = 0; j < list[i].Count; j++)
                {
                    ColumnInfo obj = list[i][j];

                    if (obj.ColumnDisplayName != "Hidden")
                    {
                        builder.Append("<td>").Append(obj.ColumnValue).Append("</td>");
                    }
                }
                builder.Append("</tr>");
            }
            builder.Append("</table></body></html>");
            response.Output.Write(builder.ToString());

        }

        public static void ExportToExcel(HttpResponse response, string myPageName, GridView grdQuery)
        {
            string FileName = myPageName + ".xls";

            response.Clear();
            response.Buffer = true;
            response.Charset = "utf-8";
            response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.ContentType = "application/ms-excel";

            string str;
            string strSub;
            str = @"<html><meta http-equiv='Content-Type' content='text/html; charset=utf-8'>
                    <body><table width='100%' border='1'><tr bgcolor='gray' style='COLOR: white'>";
            for (int k = 0; k < grdQuery.Columns.Count; k++)
            {
                str += "<td>" + grdQuery.Columns[k].HeaderText + "</td>";
            }
            str += "</tr>";
            for (int i = 0; i < grdQuery.Rows.Count; i++)
            {
                strSub = "<tr>";
                for (int j = 0; j < grdQuery.Columns.Count; j++)
                {
                    strSub += "<td>" + grdQuery.Rows[i].Cells[j].Text.ToString() + "</td>";

                }
                //strSub += "<td>" + this.grdQuery.Rows[i].FindControl("").Text.ToString() + "</td>";

                strSub += "</tr>";
                str += strSub;
            }
            str += "</table>";


            response.Write(str);
            response.End();
        }

        public static void ExportToExcelWithColor(HttpResponse Response,GridView grid,string fileName)
        {
            Response.Clear();

            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", fileName.Trim()));

            Response.Charset = "";

            //Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.ContentType = "application/vnd.xls";

            System.IO.StringWriter stringWrite = new System.IO.StringWriter();

            System.Web.UI.HtmlTextWriter htmlWrite =
            new HtmlTextWriter(stringWrite);

            grid.RenderControl(htmlWrite);

            Response.Write(stringWrite.ToString());

            Response.End();
        }

        public static void ExportToExcelDataOnly(HttpResponse Response,GridView gv,string fileName)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}.xls", fileName.Trim()));
            HttpContext.Current.Response.ContentType = "application/ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    //  Create a table to contain the grid
                    Table table = new Table();

                    //  include the gridline settings
                    table.GridLines = gv.GridLines;

                    //add caption to the table
                    if (gv.Caption.Trim() != string.Empty)
                    {
                        TableRow row = new TableRow();
                        TableCell cell = new TableCell()
                        {
                            Text = gv.Caption,
                            HorizontalAlign= HorizontalAlign.Center,
                            VerticalAlign = VerticalAlign.Middle,
                            ColumnSpan = gv.Rows[0].Cells.Count
                        };
                        row.Cells.Add(cell);
                        
                        table.Rows.Add(row);
                    }

                    //  add the header row to the table
                    if (gv.HeaderRow != null)
                    {
                        PrepareControlForExport(gv.HeaderRow);
                        table.Rows.Add(gv.HeaderRow);
                    }

                    //  add each of the data rows to the table
                    foreach (GridViewRow row in gv.Rows)
                    {
                        PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }

                    //  add the footer row to the table
                    if (gv.FooterRow != null)
                    {
                        PrepareControlForExport(gv.FooterRow);
                        table.Rows.Add(gv.FooterRow);
                    }

                    //  render the table into the htmlwriter
                    table.RenderControl(htw);

                    //  render the htmlwriter into the response
                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.End();
                }
            }
        }

        private static void PrepareControlForExport(Control control)
        {
            for (int i = 0; i < control.Controls.Count; i++)
            {
                Control current = control.Controls[i];
                if (current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
                }
                else if (current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if (current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if (current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if (current is CheckBox)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                }

                if (current.HasControls())
                {
                    PrepareControlForExport(current);
                }
            }
        }

    }
}
