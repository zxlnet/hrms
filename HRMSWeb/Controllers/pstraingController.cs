using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pstraingController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("pstraing");
        }

        public void list()
        {
            getList<pstraingBll, object>();
        }

        public void New()
        {
            getNew<tpstraing>();
        }

        public void Edit()
        {
            getEdit<tpstraing>();
        }

        public void Delete()
        {
            getDelete<tpstraing>();
        }

        public void exportExcel()
        {
            getExportExcel<pstraingBll,object>();
        }

    }
}
