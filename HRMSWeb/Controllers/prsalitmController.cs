using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Payroll;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prsalitmController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("prsalitm");
        }

        public void list()
        {
            getList<prsalitmBll, object>();
        }

        public void New()
        {
            getNew<tprsalitm>();
        }

        public void Edit()
        {
            getEdit<prsalitmBll, tprsalitm>();
        }

        public void Delete()
        {
            getDelete<tprsalitm>();
        }

        public void exportExcel()
        {
            getExportExcel<prsalitmBll, object>();
        }
    }
}
