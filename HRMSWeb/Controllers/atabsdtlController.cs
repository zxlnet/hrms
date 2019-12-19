using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Attendance;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atabsdtlController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atabsdtl");
        }

        public void list()
        {
            getList<atabsdtlBll, object>();
        }

        public void New()
        {
            getNew<tatabsdtl>();
        }

        public void Edit()
        {
            getEdit<tatabsdtl>();
        }

        public void Delete()
        {
            getDelete<tatabsdtl>();
        }

        public void exportExcel()
        {
            getExportExcel<atabsdtlBll, object>();
        }
    }
}
