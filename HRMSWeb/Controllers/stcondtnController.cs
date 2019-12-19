using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Syst;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class stcondtnController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("stcondtn");
        }

        public void list()
        {
            getList<stcondtnBll, object>();
        }

        public void New()
        {
            getNew<tstcondtn>();
        }

        public void Edit()
        {
            getEdit<stcondtnBll, tstcondtn>();
        }

        public void Delete()
        {
            getDelete<tstcondtn>();
        }

        public void exportExcel()
        {
            getExportExcel<stcondtnBll, object>();
        }
    }
}
