using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Training;
using GotWell.Model.HRMS;
using GotWell.Common;
using System.Web.Routing;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class trtraattController : BaseAttendanceController
    {
        protected ActionResult getIndex(string viewName)
        {
            string tabId = this.Request["menuId"];
            string trcd = this.Request["trcd"];
            string tableName = this.GetType().Name.GetPageName();
            string currentUser = Function.GetCurrentUser();

            if (trcd == null) trcd = "";

            #region MUF
            var q = (from p in (List<string>)HttpContext.Session[Constant.SESSION_CURRENT_MUF]
                     where p == tabId.Substring(1, tabId.Length - 1)
                     select p).ToList();

            bool muf = q.Count > 0 ? true : false;
            #endregion

            ViewData["config"] = "{tabId:\"" + tabId + "\",trcd:\"" + trcd + "\",tableName:\"t" + tableName + "\",currentUser:\"" + currentUser + "\",muf:\"" + muf + "\"}";

            SetAuthorization(tabId);

            return this.View(viewName);

        }

        public ActionResult Index()
        {
            return getIndex("trtraatt");
        }

        public void list()
        {
            getList<trtraattBll, object>();
        }

        public void New()
        {
            getNew<ttrtraatt>();
        }

        public void Edit()
        {
            getEdit<trtraattBll, ttrtraatt>();
        }

        public void Delete()
        {
            getDelete<ttrtraatt>();
        }

        public void exportExcel()
        {
            getExportExcel<trtraattBll, object>();
        }

        public ActionResult register()
        {
            string msg = string.Empty;
            try
            {
                string trcd = this.Request["trcd"];
                string emno = this.Request["emno"];

                if (trcd == null || trcd.Trim() == string.Empty || emno == null || emno.Trim() == string.Empty)
                {
                    msg = "Fail.Incorrect parameters to register.";
                }
                else
                {

                    trtraattBll bll = new trtraattBll();
                    bll.Register(trcd, emno);

                    msg = "Register success.";
                }
            }
            catch (Exception ex)
            {
                msg = "Fail." + ex.Message;
            }

            return this.RedirectToAction("Index", "Error", new RouteValueDictionary { { "msg", msg } });
        }
    }
}
