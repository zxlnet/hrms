using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GotWell.Utility;
using log4net.Config;
using GotWell.HRMS.HRMSWeb.Common;
using System.Configuration;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Authorization;
using GotWell.Model.Authorization;

namespace GotWell.HRMS.HRMSWeb
{
    public class GlobalApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Note: Change the URL to "{controller}.mvc/{action}/{id}" to enable
            //       automatic support on IIS6 and IIS7 classic mode

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                            "Default",                                              // Route name
                            "{controller}.mvc/{action}/{id}",                       // URL with parameters
                            new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
                        );

            routes.MapRoute(
                "Error",                                              // Route name
                "{controller}.mvc/{action}/{msg}",                       // URL with parameters
                new { controller = "Error", action = "Index", msg = "" }  // Parameter defaults
            );

        }

        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                XmlConfigurator.Configure();
                RegisterRoutes(RouteTable.Routes);

                Parameter.HRMS_CONNECTION_STRING = ConfigReader.getDBConnectionString_HRMS();
                Parameter.LOGGING_FILE_NAME = ConfigReader.getLogFileName();
                Parameter.LOGGING_LEVEL = (Log_LoggingLevel)Enum.Parse(typeof(Log_LoggingLevel), ConfigReader.getLogLevel().ToString());
                Parameter.LOGGING_FILE_SIZE = ConfigReader.getLogFileSize();
                Parameter.APPLICATION_NAME = ConfigReader.getAppName();
         
            }
            catch (Exception ex)
            {
                
            }           

        }

        public void Session_OnEnd()
        {
            //this.Response.Redirect("Error.aspx");
        }


        void Session_Start(object sender, EventArgs e)
        {
            string sessionId = Session.SessionID;
        }

    }
}