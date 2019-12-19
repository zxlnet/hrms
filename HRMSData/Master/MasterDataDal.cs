using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Text;
using GotWell.Common;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GotWell.HRMS.HRMSData.Master
{
    public class MasterDataDal : BaseDal
    {
        private string tableName;
        private List<string> primaryKeys;
        private List<ColumnMdl> columns;
        private bool hasDflag;

        public MasterDataDal(string tableName)
        {
            this.tableName = tableName;
            this.columns = this.GetColumns();
            this.primaryKeys = this.GetPrimaryKey();
        }

        public List<ColumnMdl> GetColumns()
        {
            List<ColumnMdl> list = new List<ColumnMdl>();

            var mtdList = (from p in gDB.tsttabdefs
                           where p.tbnm == this.tableName
                           && p.isen =="Y"
                           orderby p.cono
                           select p).Cast<tsttabdef>().ToList();

            for (int i = 0; i < mtdList.Count; i++)
            {
                tsttabdef mtd = mtdList[i];

                ColumnMdl column = new ColumnMdl();
                column.ColumnName = mtd.conm;
                string precision = mtd.dapr;
                if (!precision.Equals(string.Empty))
                {
                    column.DataPrecision = Convert.ToInt32(precision);
                }
                column.DataType = mtd.daty;
                string dataSize = mtd.dasz.ToString();
                if (!dataSize.Equals(string.Empty))
                {
                    column.DataSize = Convert.ToInt32(dataSize);
                }
                column.IsPrimaryKey = mtd.ispk.ToString();
                column.ResourceId = mtd.rsid;
                column.IsRequired = mtd.isrq.ToString();
                column.IsDisplay = mtd.isdp.ToString();
                column.DefaultValue = mtd.dfva;
                column.ControlType = mtd.ctty;
                list.Add(column);
            }
            return list;
        }

        public List<object> GetRecordsFromExcel(Type _type,string connectionString, List<ColumnInfo> list) 
        {
            StringBuilder selectSql = new StringBuilder();
            selectSql.Append("select ");
            for (int i = 0; i < list.Count; i++)
            {
                ColumnInfo column = list[i];
                if (column.ColumnValue.Equals(string.Empty))
                {
                    selectSql.Append(column.ColumnName).Append(",");
                }
            }
            string result = selectSql.ToString();

            if (result.EndsWith(","))
            {
                result = result.Substring(0, result.Length - 1);
            }

            //result += " from [" + this.tableName.Replace("t_", "") + "$]";
            result += " from [Sheet1$]";
            if (this.primaryKeys != null && this.primaryKeys.Count > 0)
            {
                result += " where ";
                for (int i = 0; i < this.primaryKeys.Count; i++)
                {
                    result += this.primaryKeys[i] + " is not null";
                    if (i < this.primaryKeys.Count - 1)
                    {
                        result += " or ";
                    }
                }
            }

            ExcelHelper excelHepler = new ExcelHelper();
            excelHepler.SetConnectionString(connectionString);
            DataSet ds =  excelHepler.ExecuteQuery(result, null);

            string json = JsonHelper.toJson(ds.Tables[0]);
           
            //List<object> lstObj = (List<object>)(JavaScriptConvert.DeserializeObject(json,_type));
            JArray arr = (JArray)JavaScriptConvert.DeserializeObject(json);
            //for (int i =0;i<ds.Tables[0].r
            List<object> lstObj = new List<object>();
            for (int i = 0; i < arr.Count; i++)
            {
                lstObj.Add(JavaScriptConvert.DeserializeObject(arr[i].ToString(),_type));
            }

            return lstObj;
        }

        private List<string> GetPrimaryKey()
        {
            List<string> keys = new List<string>();
            for (int i = 0; i < this.columns.Count; i++)
            {
                ColumnMdl mdl = this.columns[i];

                if (mdl.IsPrimaryKey.Equals(Public_Flag.Y.ToString()))
                {
                    keys.Add(mdl.ColumnName);
                }

                if (!this.hasDflag)
                {
                    hasDflag = mdl.ColumnName.Equals("dflag");
                }
            }
            return keys;
        }

        private bool IsPrimaryKey(ColumnInfo cols)
        {
            return this.primaryKeys.Contains(cols.ColumnName);
        }

        private bool ExcludeNullValues(ColumnInfo col)
        {
            return !col.ColumnValue.Equals(string.Empty);
        }

 
    }
}
