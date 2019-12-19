using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GotWell.HRMS.HRMSWeb.Views.Error
{
    public partial class Error : ViewPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string lang = ViewData["lang"] == null ? "zh-cn" : ViewData["lang"] as string;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
        }
    }
}
