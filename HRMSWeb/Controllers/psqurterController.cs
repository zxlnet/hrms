using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;


namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psqurterController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psqurter");
        }

        public void list()
        {
            getList<psqurterBll, object>();
        }

        public void New()
        {
            getNew<tpsqurter>();
        }

        public void Edit()
        {
            getEdit<tpsqurter>();
        }

        public void Delete()
        {
            getDelete<tpsqurter>();
        }

        public void exportExcel()
        {
            getExportExcel<psqurterBll, object>();
        }
    }
}
