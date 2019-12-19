using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Payroll;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prvarbleController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("prvarble");
        }

        public void list()
        {
            getList<prvarbleBll, object>();
        }

        public void New()
        {
            getNew<tprvarble>();
        }

        public void Edit()
        {
            getEdit<prvarbleBll, tprvarble>();
        }

        public void Delete()
        {
            getDelete<tprvarble>();
        }

        public void exportExcel()
        {
            getExportExcel<prvarbleBll, object>();
        }
    }
}
