using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psdetainController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psdetain");
        }

        public void list()
        {
            getList<psdetainBll, object>();
        }

        public void New()
        {
            getNew<tpsdetain>();
        }

        public void Edit()
        {
            getEdit<tpsdetain>();
        }

        public void Delete()
        {
            getDelete<tpsdetain>();
        }

        public void exportExcel()
        {
            getExportExcel<psdetainBll,object>();
        }


    }
}
