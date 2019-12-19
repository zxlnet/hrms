using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GotWell.HRMS.HRMSBusiness.Authorization;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSBusiness.Master;
using GotWell.Common;
using GotWell.LanguageResources;
using GotWell.Utility;

namespace GotWell.HRMSWeb.Views.Home
{
    public partial class Index : ViewPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string lang = ViewData["lang"] == null ? "zh-cn" : ViewData["lang"] as string;
                HRMSRes.Culture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
                System.Threading.Thread.CurrentThread.CurrentUICulture = HRMSRes.Culture;
                Session[Constant.SESSION_CULTURE] = HRMSRes.Culture;
            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }

        }
    }
}
