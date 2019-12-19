using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.Model.Authorization
{
    public class SysUserMdl
    {
        #region Model
        //public string employee_id { set; get; }
        //public string full_name { set; get; }
        //public string user_id { set; get; }
        //public bool is_checked { set; get; }

        public string emid { set; get; }
        public string urnm { set; get; }
        public string urid { set; get; }
        public bool isck { set; get; }
        public string sfid { get; set; }

        #endregion Model
    }
}
