using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pswelfreController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("pswelfre");
        }

        public void list()
        {
            getList<pswelfreBll, object>();
        }

        public void New()
        {
            getNew<tpswelfre>();
        }

        public void Edit()
        {
            getEdit<tpswelfre>();
        }

        public void Delete()
        {
            getDelete<tpswelfre>();
        }

        public void exportExcel()
        {
            getExportExcel<pswelfreBll, object>();
        }

    }
}
