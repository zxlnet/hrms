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
    public class psexprncController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psexprnc");
        }

        public void list()
        {
            getList<psexprncBll, object>();
        }

        public void New()
        {
            getNew<tpsexprnc>();
        }

        public void Edit()
        {
            getEdit<tpsexprnc>();
        }

        public void Delete()
        {
            getDelete<tpsexprnc>();
        }

        public void exportExcel()
        {
            getExportExcel<psexprncBll,object>();
        }

    }
}