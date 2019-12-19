using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;
using System.Collections;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psinsurnController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psinsurn");
        }

        public void list()
        {
            getList<psinsurnBll, object>();
        }

        public void New()
        {
            getNew<tpsinsurn>();
        }

        public void Edit()
        {
            getEdit<tpsinsurn>();
        }

        public void Delete()
        {
            getDelete<tpsinsurn>();
        }

        public void exportExcel()
        {
            getExportExcel<psinsurnBll, object>();
        }
    }
}