using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pshctcfgController : BaseAttendanceController
    {
        public ActionResult Index()
        {
            return getIndex("pshctcfg");
        }

        public void list()
        {
            getList<pshctcfgBll, object>();
        }

        public void New()
        {
            getNew<tpshctcfg>();
        }

        public void Edit()
        {
            getEdit<tpshctcfg>();
        }

        public void Delete()
        {
            getDelete<tpshctcfg>();
        }

        public void exportExcel()
        {
            getExportExcel<pshctcfgBll, object>();
        }
    }
}