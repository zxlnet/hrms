using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;


namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class psclubController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("psclub");
        }

        public void list()
        {
            getList<psclubBll, object>();
        }

        public void New()
        {
            getNew<tpsclub>();
        }

        public void Edit()
        {
            getEdit<tpsclub>();
        }

        public void Delete()
        {
            getDelete<tpsclub>();
        }

        public void exportExcel()
        {
            getExportExcel<psclubBll,object>();
        }
    }
}
