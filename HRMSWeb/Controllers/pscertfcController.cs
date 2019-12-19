using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Personal;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class pscertfcController : BaseController
    {

        public ActionResult Index()
        {
            return getIndex("pscertfc");
        }

        public void list()
        {
            getList<pscertfcBll, object>();
        }

        public void New()
        {
            getNew<tpscertfc>();
        }

        public void Edit()
        {
            getEdit<tpscertfc>();
        }

        public void Delete()
        {
            getDelete<tpscertfc>();
        }

        public void exportExcel()
        {
            getExportExcel<pscertfcBll, object>();
        }


    }
}
