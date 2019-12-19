using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atanarstController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atanarst");
        }

        public void list()
        {
            getList<atanarstBll, object>();
        }

        public void New()
        {
            getNew<tatanarst>();
        }

        public void Edit()
        {
            getEdit<tatanarst>();
        }

        public void Delete()
        {
            getDelete<tatanarst>();
        }

        public void exportExcel()
        {
            getExportExcel<atanarstBll, object>();
        }
    }
}
