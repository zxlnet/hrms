using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Model.Authorization;
using System.Text;
using GotWell.Common;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pspersonController : BaseController
    {
        public ActionResult Index()
        {

            string tabId = this.Request["menuId"];
            string emno = this.Request["emno"];
            string tableName = this.GetType().Name.GetPageName();
            string currentUser = Function.GetCurrentUser();

            if (emno == null) emno = "";

            #region MUF
            var q = (from p in (List<string>)HttpContext.Session[Constant.SESSION_CURRENT_MUF]
                     where p == tabId.Substring(1, tabId.Length - 1)
                     select p).ToList();

            bool muf = q.Count > 0 ? true : false;
            #endregion

            ViewData["config"] = "{tabId:\"" + tabId + "\",emno:\"" + emno + "\",tableName:\"t" + tableName + "\",currentUser:\"" + currentUser + "\",muf:\"" + muf + "\"}";

            SetAuthorization(tabId);

            return this.View("psperson");
        }


        public void list()
        {
            getList<pspersonBll,object>();
        }

        public void New()
        {
            getNew<tpsperson>();
        }

        public void Edit()
        {
            getEdit<tpsperson>();
        }

        public void Delete()
        {
            getDelete<tpsperson>();
        }

        public void exportExcel()
        {
            getExportExcel<pspersonBll, object>();
        }

        public void GetAutoStaffId()
        {
            string message=string.Empty;
            try
            {
                string emno = string.Empty;
                string sfid = string.Empty;
                pspersonBll bll = new pspersonBll();

                bll.GetAutoStaffId(ref emno, ref sfid);

                message = "{status:'success',emno:'" + emno + "',sfid:'" + sfid + "'}";
            }
            catch (Exception ex)
            {
                message = "{status:'failure',msg:'" + ExceptionPaser.Parse(ex.Message, ex, true) + "'}";
            }

            Response.Output.Write(message);
        }
    }
}
