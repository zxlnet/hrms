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
    public class pshltrecController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("pshltrec");
        }

        public void list()
        {
            getList<pshltrecBll, object>();
        }

        public void New()
        {
            getNew<tpshltrec>();
        }

        public void Edit()
        {
            getEdit<tpshltrec>();
        }

        public void Delete()
        {
            getDelete<tpshltrec>();
        }

        public void exportExcel()
        {
            getExportExcel<pshltrecBll,object>();
        }


    }
}