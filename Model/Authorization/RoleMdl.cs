using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Common;

namespace GotWell.Model.Authorization
{
    [Serializable]
    public class RoleMdl
    {
        #region Model
        public string Role_Id { set; get; }
        public string Role_Name { set; get; }
        public string Role_Desc { set; get; }
        public Public_Status Role_Status { set; get; }
        public Public_Flag Is_System_Role { set; get; }
        public string Last_Modified_User { set; get; }
        public DateTime Last_Modified_Time { set; get; }
        public string apnm { set; get; }
        public string roty { get; set; }
        public string alep { get; set; }
        #endregion Model
    }
}
