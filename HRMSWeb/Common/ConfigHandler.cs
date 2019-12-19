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
/// Summary description for ConfigHandler
/// </summary>
/// 
namespace GotWell.HRMS.HRMSWeb.Common
{
    public class ConfigHandler : ConfigurationSection
    {
        /// <summary>
        /// 得到当前的数据库类型
        /// </summary>
        [ConfigurationProperty("DB_Type")]
        public String DB_Type
        {
            get
            { return (String)this["DB_Type"]; }
            set
            { this["DB_Type"] = value; }
        }

        /// <summary>
        /// 得到MSSql数据库连接字符串
        /// </summary>
        [ConfigurationProperty("MSSql")]
        public String MSSqlConnectionString
        {
            get
            { return (String)this["MSSql"]; }
            set
            { this["MSSql"] = value; }
        }

        /// <summary>
        /// 得到Oracle数据库连接字符串
        /// </summary>
        [ConfigurationProperty("Oracle")]
        public String SqlConnectionString
        {
            get
            { return (String)this["Oracle"]; }
            set
            { this["Oracle"] = value; }
        }

        /// <summary>
        /// 得到RFC连接字符串
        /// </summary>
        [ConfigurationProperty("RFC")]
        public String RFCConnectionString
        {
            get
            { return (String)this["RFC"]; }
            set
            { this["RFC"] = value; }
        }

        /// <summary>
        /// 得到Excel数据库连接字符串
        /// </summary>
        [ConfigurationProperty("Excel")]
        public String ExcelConnectionString
        {
            get
            { return (String)this["Excel"]; }
            set
            { this["Excel"] = value; }
        }

        [ConfigurationProperty("level", DefaultValue = "0", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 20)]
        public String Level
        {
            get
            { return (String)this["level"]; }
            set
            { this["level"] = value; }
        }

        [ConfigurationProperty("logfilesize", DefaultValue = "0", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 20)]
        public String LogFileSize
        {
            get
            { return (String)this["logfilesize"]; }
            set
            { this["logfilesize"] = value; }
        }

        [ConfigurationProperty("logfilename", DefaultValue = "0", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 50)]
        public String LogFileName
        {
            get
            { return (String)this["logfilename"]; }
            set
            { this["logfilename"] = value; }
        }

        [ConfigurationProperty("xmlpath", DefaultValue = " ", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{};", MinLength = 1, MaxLength = 50)]
        public String ConfigPath
        {
            get
            { return (String)this["xmlpath"]; }
            set
            { this["xmlpath"] = value; }
        }

        [ConfigurationProperty("envname", DefaultValue = " ", IsRequired = false)]
        [StringValidator(InvalidCharacters = "", MinLength = 1, MaxLength = 50)]
        public String EnvName
        {
            get
            { return (String)this["envname"]; }
            set
            { this["envname"] = value; }
        }

        [ConfigurationProperty("appname", DefaultValue = " ", IsRequired = false)]
        [StringValidator(InvalidCharacters = "", MinLength = 1, MaxLength = 50)]
        public String AppName
        {
            get
            { return (String)this["appname"]; }
            set
            { this["appname"] = value; }
        }

    }
}




