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
    public class pscontrtController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("pscontrt");
        }

        public void list()
        {
            getList<pscontrtBll, object>();
        }

        public void New()
        {
            getNew<tpscontrt>();
        }

        public void Edit()
        {
            getEdit<tpscontrt>();
        }

        public void Delete()
        {
            getDelete<tpscontrt>();
        }

        public void exportExcel()
        {
            getExportExcel<pscontrtBll,object>();
        }


    }
}