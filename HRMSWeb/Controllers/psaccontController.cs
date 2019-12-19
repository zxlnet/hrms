using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;


namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psaccontController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psaccont");
        }

        public void list()
        {
            getList<psaccontBll, object>();
        }

        public void New()
        {
            getNew<tpsaccont>();
        }

        public void Edit()
        {
            getEdit<tpsaccont>();
        }

        public void Delete()
        {
            getDelete<tpsaccont>();
        }

        public void exportExcel()
        {
            getExportExcel<psaccontBll,object>();
        }


    }
}
