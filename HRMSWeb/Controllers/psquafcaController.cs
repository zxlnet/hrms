using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psquafcaController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psquafca");
        }

        public void list()
        {
            getList<psquafcaBll, object>();
        }

        public void New()
        {
            getNew<tpsquafca>();
        }

        public void Edit()
        {
            getEdit<tpsquafca>();
        }

        public void Delete()
        {
            getDelete<tpsquafca>();
        }

        public void exportExcel()
        {
            getExportExcel<psquafcaBll,object>();
        }
    }
}
