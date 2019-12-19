using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psaddresController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psaddres");
        }

        public void list()
        {
            getList<psaddresBll, object>();
        }

        public void New()
        {
            getNew<tpsaddre>();
        }

        public void Edit()
        {
            getEdit<tpsaddre>();
        }

        public void Delete()
        {
            getDelete<tpsaddre>();
        }

        public void exportExcel()
        {
            getExportExcel<psaddresBll, object>();
        }

    }
}
