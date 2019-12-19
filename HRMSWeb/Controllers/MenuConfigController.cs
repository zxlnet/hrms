using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSBusiness.Authorization;
using Newtonsoft.Json;
using System.Collections;
using GotWell.Model.Common;
using GotWell.LanguageResources;
using GotWell.Model.Authorization;
using GotWell.Common;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSWeb.Common;


namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class MenuConfigController : Controller
    {
        /// <summary>
        /// Purpose: 获取顶层菜单，过滤无权限
        /// </summary>
        public void listTopMenu()
        {
            try
            {
                AuthorizationMdl authMdl = (AuthorizationMdl)this.HttpContext.Session[Constant.SESSION_AUTHORIZATION];
                MenuConfigBll menuConfigBll = new MenuConfigBll();

                List<tstmnucfg> array = menuConfigBll.getTopMenu();
                ArrayList result = new ArrayList();
                for (int i = 0; i < array.Count; i++)
                {
                    tstmnucfg mdl = (tstmnucfg)array[i];

                    System.Globalization.CultureInfo culture = (System.Globalization.CultureInfo)this.HttpContext.Session[Constant.SESSION_CULTURE];
                    String text = HRMSRes.ResourceManager.GetString(mdl.rsid, culture);
                    if (text != null)
                    {
                        mdl.munm = text;
                    }

                    if (mdl.muid != "Y")
                    {
                        if (getSubMenu(mdl.muid).Count > 0)
                        {
                            result.Add(mdl);
                        }
                    }
                    else
                        result.Add(mdl);
                    
                }
                Response.Output.Write(JavaScriptConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        /// <summary>
        /// Purpose: 获取树状菜单
        /// </summary>
        public void getMenuTree()
        {
            ArrayList result = new ArrayList();
            try
            {               
                var pami = this.Request["pami"];

                if (pami=="Y")
                    Response.Output.Write(JavaScriptConvert.SerializeObject(getMUFTreeNode(pami)));
                else
                    Response.Output.Write(JavaScriptConvert.SerializeObject(getSubTreeNode(pami)));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Purpose: 获取下一层树，使用递归
        /// </summary>
        /// <param name="pami"></param>
        /// <returns></returns>
        private ArrayList getSubTreeNode(string pami)
        {
            ArrayList result = new ArrayList();
            try
            {                              
                ArrayList array = getSubMenu(pami);
                for (int i = 0; i < array.Count; i++)
                {
                    tstmnucfg mdl = (tstmnucfg)array[i];
                    Hashtable ht = new Hashtable();
                    ht.Add("id", "M" + mdl.muid);
                    ht.Add("iconCls", "user");
                    System.Globalization.CultureInfo culture = (System.Globalization.CultureInfo)this.HttpContext.Session[Constant.SESSION_CULTURE];
                    String text = HRMSRes.ResourceManager.GetString(mdl.rsid, culture);
                    if (text == null)
                    {
                        ht.Add("text", mdl.munm);
                    }
                    else
                    {
                        ht.Add("text", text);
                    }
                    if (getSubMenu(mdl.muid).Count > 0)
                    {
                        ht.Add("children", getSubTreeNode(mdl.muid));
                    }
                    else
                    {
                        ht.Add("leaf", "true");
                        ht.Add("href", mdl.murl);
                    }
                    result.Add(ht);
                    
                }
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Purpose: 获取下一层菜单，过滤无权限
        /// </summary>
        /// <param name="pami"></param>
        /// <returns></returns>
        private ArrayList getSubMenu(string pami)
        {
            ArrayList result = new ArrayList();
            try
            {
                AuthorizationMdl authMdl = (AuthorizationMdl)this.HttpContext.Session[Constant.SESSION_AUTHORIZATION]; 
                MenuConfigBll menuConfigBll = new MenuConfigBll();
                List<tstmnucfg> array = menuConfigBll.getSubMenu(pami);
                for (int i = 0; i < array.Count; i++)
                {
                    tstmnucfg mdl = (tstmnucfg)array[i];
                    if (authMdl.checkPermissionByFuncId(mdl.fnid))
                    {
                        result.Add(mdl);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ArrayList getMUFTreeNode(string pami)
        {
            ArrayList result = new ArrayList();
            try
            {
                MenuConfigBll menuConfigBll = new MenuConfigBll();
                List<tstmnucfg> lstMUF = menuConfigBll.getMUF();
                for (int i = 0; i < lstMUF.Count; i++)
                {
                    tstmnucfg mdl = lstMUF[i];
                    Hashtable ht = new Hashtable();
                    ht.Add("id", "M" + mdl.muid);
                    ht.Add("iconCls", "user");
                    System.Globalization.CultureInfo culture = (System.Globalization.CultureInfo)this.HttpContext.Session[Constant.SESSION_CULTURE];
                    String text = HRMSRes.ResourceManager.GetString(mdl.rsid, culture);
                    if (text == null)
                    {
                        ht.Add("text", mdl.munm);
                    }
                    else
                    {
                        ht.Add("text", text);
                    }
                    if (getSubMenu(mdl.muid).Count > 0)
                    {
                        ht.Add("children", getSubTreeNode(mdl.muid));
                    }
                    else
                    {
                        ht.Add("leaf", "true");
                        ht.Add("href", mdl.murl);
                    }
                    result.Add(ht);

                }
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void updateMUF()
        {
            string message = string.Empty;
            try
            {
                string record = this.Request["record"];
                Hashtable ht = JavaScriptConvert.DeserializeObject<Hashtable>(record);

                MenuConfigBll bll = new MenuConfigBll();

                if (ht["muid"].ToString().Equals(string.Empty))
                    message = "{status:'success',msg:''}";
                else
                {
                    bll.UpdateMUF(ht["muid"].ToString().Substring(1, ht["muid"].ToString().Length-1), ht["action"].ToString());

                    HttpContext.Session[Constant.SESSION_CURRENT_MUF] = bll.getMUFForSession();

                    if (ht["action"].ToString()=="add")
                        message = "{status:'success',msg:'Add into MUF successfully.'}";
                    else
                        message = "{status:'success',msg:'Delete from MUF successfully.'}";
                }
            }
            catch (Exception ex)
            {
                message = "{status:'fail',msg:'" + ExceptionPaser.Parse(ex) + "'}"; ;
            }

            Response.Output.Write(message);
        }
    }
}
