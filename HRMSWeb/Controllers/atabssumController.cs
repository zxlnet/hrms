using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atabssumController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atabssum");
        }

        public void list()
        {
            getList<atabssumBll, object>();
        }

        public void New()
        {
            getNew<tatabssum>();
        }

        public void Edit()
        {
            getEdit<tatabssum>();
        }

        public void Delete()
        {
            getDelete<tatabssum>();
        }

        public void exportExcel()
        {
            getExportExcel<atabssumBll, object>();
        }

        public void Calculate()
        {
            string message = string.Empty;


        }
    }
}
