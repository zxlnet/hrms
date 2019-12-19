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
    public class psrelshpController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psrelshp");
        }

        public void list()
        {
            getList<psrelshpBll, object>();
        }

        public void New()
        {
            getNew<tpsrelshp>();
        }

        public void Edit()
        {
            getEdit<tpsrelshp>();
        }

        public void Delete()
        {
            getDelete<tpsrelshp>();
        }

        public void exportExcel()
        {
            getExportExcel<psrelshpBll,object>();
        }
    }
}