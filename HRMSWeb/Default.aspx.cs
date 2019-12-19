using System;
using System.Collections.Generic;
using System.Web.UI;

namespace GotWell.HRMS.HRMSWeb
{
    public partial class _Default : Page
    {
        public void Page_Load(object sender, System.EventArgs e)
        {
            Response.Redirect("~/Home.mvc");
        }
    }
}
