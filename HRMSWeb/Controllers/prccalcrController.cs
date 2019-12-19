using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GotWell.HRMS.HRMSBusiness.Payroll;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class prccalcrController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("prccalcr");
        }

        public void list()
        {
            getList<prccalcrBll, object>();
        }

        public void New()
        {
            getNew<tprccalcr>();
        }

        public void Edit()
        {
            getEdit<tprccalcr>();
        }

        public void Delete()
        {
            getDelete<tprccalcr>();
        }

        public void exportExcel()
        {
            getExportExcel<prccalcrBll, object>();
        }

    }
}
