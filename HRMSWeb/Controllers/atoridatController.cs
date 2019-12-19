using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Model.HRMS;
using System.Collections;
using Newtonsoft.Json;
using GotWell.HRMS.HRMSCore;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.LanguageResources;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class atoridatController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            string atdt = this.Request["atdt"];
            string info = "atdt:\"" + atdt + "\"";
            return getIndex("atoridat", info);
        }

        public void list()
        {
            getList<atoridatBll, object>();
        }

        public void New()
        {
            getNew<tatoridat>();
        }

        public void Edit()
        {
            getEdit<atoridatBll,tatoridat>();
        }

        public void Delete()
        {
            getDelete<tatoridat>();
        }

        public void exportExcel()
        {
            getExportExcel<atoridatBll, object>();
        }
    }
}
