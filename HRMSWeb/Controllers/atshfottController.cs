using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;
using GotWell.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atshfottController : BaseAttendanceController
    {
        public override ActionResult getIndex(string viewName)
        {
            string tabId = this.Request["menuId"];
            string sfcd = this.Request["sfcd"];
            string tableName = this.GetType().Name.GetPageName();
            string currentUser = Function.GetCurrentUser();

            if (sfcd == null) sfcd = "";

            #region MUF
            var q = (from p in (List<string>)HttpContext.Session[Constant.SESSION_CURRENT_MUF]
                     where p == tabId.Substring(1, tabId.Length - 1)
                     select p).ToList();

            bool muf = q.Count > 0 ? true : false;
            #endregion


            ViewData["config"] = "{tabId:\"" + tabId + "\",sfcd:\"" + sfcd + "\",tableName:\"t" + tableName + "\",currentUser:\"" + currentUser + "\",muf:\"" + muf + "\"}";

            SetAuthorization(tabId);

            return this.View(viewName);

        }

        public ActionResult Index()
        {
            return getIndex("atshfott");
        }

        public void list()
        {
            getList<atshfottBll, object>();
        }

        public void New()
        {
            getNew<tatshfott>();
        }

        public void Edit()
        {
            getEdit<tatshfott>();
        }

        public void Delete()
        {
            getDelete<tatshfott>();
        }

        public void exportExcel()
        {
            getExportExcel<atshfottBll, object>();
        }
    }
}
