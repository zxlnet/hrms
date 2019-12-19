using System.Web.Mvc;

namespace GotWell.HRMS.HRMSWeb.Controllers
{
    public class rphctctlController : BaseController
    {
        public ActionResult Index()
        {
            return getIndex("rphctctl");
        }
    }
}
