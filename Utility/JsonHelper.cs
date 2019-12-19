using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.IO;

namespace GotWell.Utility
{
    public class JsonHelper
    {
        public static string toJson(DataTable table)
        {
            StringBuilder json = new StringBuilder();
            int rowCount = 0;
            foreach (DataRow row in table.Rows)
            {
                int columnCount = 0;
                json.Append("{");
                foreach (DataColumn column in table.Columns)
                {
                    string columnName = column.ColumnName.ToLower();
                    object value = row[column];
                    if (column.DataType.FullName.Equals("System.DateTime") && !value.ToString().Equals(string.Empty))
                    {
                        value = ((DateTime)value).ToString("yyyyMMdd");
                        json.Append(columnName).Append(":'").Append(value).Append("'");
                    }
                    else if (column.DataType.FullName.Equals("System.Int32") && !value.ToString().Equals(string.Empty))
                    {
                        value = value.ToString();
                        json.Append(columnName).Append(":'").Append(Regex.Escape(value.ToString()).Replace("'", "\\'")).Append("'");
                    }
                    else
                    {
                        int x = value.ToString().IndexOf('.')>0?value.ToString().IndexOf('.'):0;
                        x = x==0?0:value.ToString().Length - x -1 ;
                        
                        json.Append(columnName).Append(":'").Append(Regex.Escape(string.Format("{0:N" + x.ToString() + "}", value))).Append("'");
                    }                    
                    if (columnCount++ < table.Columns.Count - 1)
                    {
                        json.Append(",");
                    }
                }
                json.Append("}");
                if (rowCount++ < table.Rows.Count - 1)
                {
                    json.Append(",");
                }

            }
            return "[" + json.ToString() + "]";
        }

        public static string toJson(DataTable table,int start,int end)
        {
            StringBuilder json = new StringBuilder();           
            int startIndex = Math.Min(start,table.Rows.Count);
            int endIndex = Math.Max(startIndex, Math.Min(end, table.Rows.Count));
            int total = endIndex - startIndex;

            for (int i = startIndex; i < endIndex; i++)
            {
                DataRow row = table.Rows[i];
                int columnCount = 0;
                json.Append("{");
                foreach (DataColumn column in table.Columns)
                {
                    string columnName = column.ColumnName.ToLower();
                    object value = row[column];
                    if (column.DataType.FullName.Equals("System.DateTime") && !value.ToString().Equals(string.Empty))
                    {
                        value = ((DateTime)value).ToString("yyyyMMdd");
                        json.Append(columnName).Append(":'").Append(value).Append("'");
                    }
                    else if (column.DataType.FullName.Equals("System.Int32") && !value.ToString().Equals(string.Empty))
                    {
                        value = value.ToString();
                        json.Append(columnName).Append(":'").Append(Regex.Escape(value.ToString()).Replace("'", "\\'")).Append("'");
                    }
                    else
                    {
                        //json.Append(columnName).Append(":'").Append(Regex.Escape(value.ToString()).Replace("'", "\\'")).Append("'");
                        //json.Append(columnName).Append(":'").Append(Regex.Escape(string.Format("{0:N}", value))).Append("'");
                        int x = value.ToString().IndexOf('.') > 0 ? value.ToString().IndexOf('.') : 0;
                        x = x == 0 ? 0 : value.ToString().Length - x - 1;

                        json.Append(columnName).Append(":'").Append(Regex.Escape(string.Format("{0:N" + x.ToString() + "}", value))).Append("'");
                    }
                    if (columnCount++ < table.Columns.Count - 1)
                    {
                        json.Append(",");
                    }
                }
                json.Append("}");
                if (i < endIndex - 1)
                {
                    json.Append(",");
                }
            }
            return "{results:" + total + ",rows:[" + json.ToString() + "]}";
        }

        public static string toJsonWithTime(DataTable table, int total)
        {
            StringBuilder json = new StringBuilder();
            int rowCount = 0;
            foreach (DataRow row in table.Rows)
            {
                int columnCount = 0;
                json.Append("{");
                foreach (DataColumn column in table.Columns)
                {
                    string columnName = column.ColumnName.ToLower();
                    object value = row[column];
                    if (column.DataType.FullName.Equals("System.DateTime") && !value.ToString().Equals(string.Empty))
                    {
                        value = ((DateTime)value).ToString("yyyyMMdd H:mm:ss");
                        json.Append(columnName).Append(":'").Append(value).Append("'");
                    }
                    else if (column.DataType.FullName.Equals("System.Decimal") && !value.ToString().Equals(string.Empty))
                    {                       
                        //json.Append(columnName).Append(":'").Append(Regex.Escape(string.Format("{0:N}", value))).Append("'");
                        int x = value.ToString().IndexOf('.') > 0 ? value.ToString().IndexOf('.') : 0;
                        x = x == 0 ? 0 : value.ToString().Length - x - 1;

                        json.Append(columnName).Append(":'").Append(Regex.Escape(string.Format("{0:N" + x.ToString() + "}", value))).Append("'");
                    }
                    else
                    {
                        json.Append(columnName).Append(":'").Append(value.ToString()).Append("'");
                    }
                    if (columnCount++ < table.Columns.Count - 1)
                    {
                        json.Append(",");
                    }
                }
                json.Append("}");
                if (rowCount++ < table.Rows.Count - 1)
                {
                    json.Append(",");
                }

            }
            return "{results:" + total + ",rows:[" + json.ToString() + "]}";
        }
    }
}
