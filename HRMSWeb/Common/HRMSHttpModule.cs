using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using GotWell.Common;
using GotWell.Model.Common;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Common
{
    public class HRMSHttpModule : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {

        }

        public void Init(HttpApplication application)
        {
            application.AcquireRequestState += (new EventHandler(this.Application_AcquireRequestState));
        }

        private void Application_AcquireRequestState(Object source, EventArgs e)
        {
            HttpApplication Application = (HttpApplication)source;
            //string ErrorPage = "Error.mvc/Timeout?msg=" + HRMSRes.ResourceManager.GetString("Public_Message_SessionTimeOut");
            string ErrorPage = "Error.mvc/Timeout";
            string LogonPage = "Logon.mvc".ToUpper();
            string DefaultPage = "Default.aspx".ToUpper();
            string Home = "Home".ToUpper();
            string url = Application.Context.Request.Path.ToUpper();
            if (url.IndexOf("aspx".ToUpper()) >= 0 || url.IndexOf("mvc".ToUpper()) >= 0)
            {
                if ((url.IndexOf(LogonPage.ToUpper())) < 0 && (url.IndexOf(ErrorPage.ToUpper())) < 0 && (url.IndexOf(DefaultPage) < 0) && (url.IndexOf(Home) < 0))
                {
                    if (Application.Context.Session != null && Application.Context.Session[Constant.SESSION_AUTHORIZATION] == null)
                    {
                            Application.Context.Response.Output.Write("location.href='" + ErrorPage + "'@@@");

                    }
                }
            }
        }
        #endregion
    }
}
