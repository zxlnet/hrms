using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pseductnController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("pseductn");
        }

        public void list()
        {
            getList<pseductnBll, object>();
        }

        public void New()
        {
            getNew<tpseductn>();
        }

        public void Edit()
        {
            getEdit<tpseductn>();
        }

        public void Delete()
        {
            getDelete<tpseductn>();
        }

        public void exportExcel()
        {
            getExportExcel<pseductnBll,object>();
        }


    }
}
