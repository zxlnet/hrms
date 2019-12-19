using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.HRMS;

namespace GotWell.Model.HRMS
{
    public class MasterDataMdl
    {
        public string TableName { get; set; }
        public List<ColumnMdl> columns { get; set; }
    }
}
