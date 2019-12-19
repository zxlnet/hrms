using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GotWell.Model.Authorization
{
    public class ApplicationMdl
    {
        #region Model
        public string apnm { set; get; }
        public string Web_Url { set; get; }
        public string Chinese_Desc { set; get; }
        public string English_Desc { set; get; }
        public string Application_Version { set; get; }
        public string Platform { set; get; }
        public DateTime Released_Time { set; get; }
        public string Released_By { set; get; }
        public string New_Features { set; get; }
        public string Status { set; get; }
        #endregion Model
    }
}
