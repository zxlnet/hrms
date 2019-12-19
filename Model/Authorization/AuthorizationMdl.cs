using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Common;

namespace GotWell.Model.Authorization
{
    [Serializable]
    public class AuthorizationMdl
    {
        public string Action { set; get; }
        public UserMdl User { set; get; }
        public List<AppMdl> Applications { set; get; }

        #region Public Functions
        /// <summary>
        /// Purpose: 根据Func_Id,判断用户是否有使用该Function的权限
        /// </summary>
        /// <param name="_func_id"></param>
        /// <returns></returns>
        public Boolean checkPermissionByFuncId(string _func_id)
        {
            for (int n = 0; n < Applications.Count; n++)
            {
                List<ModuleMdl> modules=Applications[n].Modules;
                for (int i = 0; i < modules.Count; i++)
                {
                    List<FunctionMdl> functions = modules[i].Functions;
                    for (int j = 0; j < functions.Count; j++)
                    {
                        if (functions[j].Id.Equals(_func_id) && functions[j].Permission.Equals(Security_Permission_Type.Allowed))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Purpose: 根据Func_Url,判断用户是否有使用该Function的权限
        /// </summary>
        /// <param name="_func_url"></param>
        /// <returns></returns>
        public Boolean checkPermissionByFuncUrl(string _func_url)
        {
            for (int n = 0; n < Applications.Count; n++)
            {
                List<ModuleMdl> modules = Applications[n].Modules;
                for (int i = 0; i < modules.Count; i++)
                {
                    List<FunctionMdl> functions = modules[i].Functions;
                    for (int j = 0; j < functions.Count; j++)
                    {
                        if (functions[j].Url.Equals(_func_url) && functions[j].Permission.Equals(Security_Permission_Type.Allowed))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion

    }
    [Serializable]
    public class AppMdl
    {
        public string apnm { set; get; }
        public string Web_Url { set; get; }
        public List<RoleMdl> Roles { set; get; }
        public List<ModuleMdl> Modules { set; get; }
    }
    [Serializable]
    public class ModuleMdl
    {
        public string Name { set; get; }
        public List<FunctionMdl> Functions { set; get; }
    }
    [Serializable]
    public class FunctionMdl
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string Url { set; get; }
        public Security_Permission_Type Permission { set; get; }
    }

    [Serializable]
    public class UserMdl
    {
        public string urid { get; set; }
        public string urnm { set; get; }
        public string sfid { get; set; }
    }
}
