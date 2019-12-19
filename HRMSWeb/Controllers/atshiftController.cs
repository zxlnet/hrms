using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atshiftController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atshift");
        }

        public void list()
        {
            getList<atshiftBll, object>();
        }

        public void New()
        {
            getNew<tatshift>();
        }

        public void Edit()
        {
            getEdit<tatshift>();
        }

        public void Delete()
        {
            getDelete<tatshift>();
        }

        public void exportExcel()
        {
            getExportExcel<atshiftBll, object>();
        }
    }
}
