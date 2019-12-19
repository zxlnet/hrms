using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.Model.Common
{
    public class ColumnInfo
    {
        //Now, for exporting Excel
        public string ColumnDisplayName { set; get; }
        public string ColumnName { set; get; }        
        public string ColumnValue { get; set; }
        public string IsPrimaryKey { get; set; }
        public string ColumnOldValue { get; set; }
        public string ColumnType { get; set; }
        public string ColumnCategory { get; set; }

        public ColumnInfo()
        {
            ColumnType = "string";
        }
    }
}
