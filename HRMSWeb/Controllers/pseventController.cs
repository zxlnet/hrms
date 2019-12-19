using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;


namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pseventController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psevent");
        }

        public void list()
        {
            getList<pseventBll, object>();
        }

        public void New()
        {
            getNew<tpsevent>();
        }

        public void Edit()
        {
            getEdit<tpsevent>();
        }

        public void Delete()
        {
            getDelete<tpsevent>();
        }

        public void exportExcel()
        {
            getExportExcel<pseventBll,object>();
        }
    }
}
