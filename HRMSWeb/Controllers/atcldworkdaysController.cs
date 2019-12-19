using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atcldwkdsController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("atcldwkds");
        }

        public void list()
        {
            getList<atcldwkdsBll, object>();
        }

        public void New()
        {
            getNew<tatcldwkd>();
        }

        public void Edit()
        {
            getEdit<tatcldwkd>();
        }

        public void Delete()
        {
            getDelete<tatcldwkd>();
        }

        public void exportExcel()
        {
            getExportExcel<atcldwkdsBll, object>();
        }
    }
}
