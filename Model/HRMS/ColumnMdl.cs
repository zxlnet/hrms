using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.Model.HRMS
{
    public class ColumnMdl
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int DataSize { get; set; }
        public int DataPrecision { get; set; }
        public string IsPrimaryKey { get; set; }
        public string IsRequired { get; set; }
        public string ResourceId { get; set; }
        public string Remark { get; set; }
        public string DefaultValue { get; set; }
        public string IsDisplay { get; set; }
        public string ControlType { get; set; }
    }
}
