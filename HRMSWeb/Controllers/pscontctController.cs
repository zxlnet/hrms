using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;


namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pscontctController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("pscontct");
        }

        public void list()
        {
            getList<pscontctBll, object>();
        }

        public void New()
        {
            getNew<tpscontct>();
        }

        public void Edit()
        {
            getEdit<tpscontct>();
        }

        public void Delete()
        {
            getDelete<tpscontct>();
        }

        public void exportExcel()
        {
            getExportExcel<pscontctBll,object>();
        }


    }
}
