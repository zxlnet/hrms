using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for ConfigReader
/// </summary>
/// 
namespace GotWell.HRMS.HRMSWeb.Common
{
    public class ConfigReader
    {
        public static string getDBConnectionString_HRMS()
        {
            //ConfigHandler config = (ConfigHandler)System.Configuration.ConfigurationManager.GetSection(
            //    "GotWell.HRMS.HRMSWeb/Connections/HRMS_ConnectionString");
            ConfigHandler config = (ConfigHandler)ConfigurationManager.GetSection("GotWell.HRMS.HRMSWeb/Connections/HRMS_ConnectionString");
            return config.MSSqlConnectionString;
        }

        public static string getDBConnectionString_Excel()
        {
            ConfigHandler config = (ConfigHandler)System.Configuration.ConfigurationManager.GetSection(
                "GotWell.HRMS.HRMSWeb/Connections/Excel_ConnectionString");
            return config.ExcelConnectionString;
        }

        public static string getLogLevel()
        {
            ConfigHandler config = (ConfigHandler)System.Configuration.ConfigurationManager.GetSection(
              "GotWell.HRMS.HRMSWeb/Logging");
            return config.Level;
        }

        public static string getLogFileSize()
        {
            ConfigHandler config = (ConfigHandler)System.Configuration.ConfigurationManager.GetSection(
              "GotWell.HRMS.HRMSWeb/Logging");
            return config.LogFileSize;
        }

        public static string getLogFileName()
        {
            ConfigHandler config = (ConfigHandler)System.Configuration.ConfigurationManager.GetSection(
              "GotWell.HRMS.HRMSWeb/Logging");
            return config.LogFileName;
        }

        public static string getReportinConfigPath()
        {
            ConfigHandler config = (ConfigHandler)System.Configuration.ConfigurationManager.GetSection(
              "GotWell.HRMS.HRMSWeb/Reporting");
            return config.ConfigPath;
        }

        public static string getEnvironmentName()
        {
            ConfigHandler config = (ConfigHandler)System.Configuration.ConfigurationManager.GetSection(
              "GotWell.HRMS.HRMSWeb/Environment");
            return config.EnvName;
        }

        public static string getAppName()
        {
            ConfigHandler config = (ConfigHandler)System.Configuration.ConfigurationManager.GetSection(
              "GotWell.HRMS.HRMSWeb/Application");
            return config.AppName;
        }

    }
}
