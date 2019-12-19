using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Overtime;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class otsummryController : BaseAttendanceController
    {

        public ActionResult Index()
        {
            return getIndex("otsummry");
        }

        public void list()
        {
            getList<otsummryBll, object>();
        }

        public void New()
        {
            getNew<totsummry>();
        }

        public void Edit()
        {
            getEdit<totsummry>();
        }

        public void Delete()
        {
            getDelete<totsummry>();
        }

        public void exportExcel()
        {
            getExportExcel<otsummryBll, object>();
        }
    }
}
