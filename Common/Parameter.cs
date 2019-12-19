/**********************************************************************/
/*
 * Define system variables or parameters in this class
 * 
/**********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;


namespace GotWell.Common
{
    public class Parameter
    {
        #region General
        //定义系统的标题
        public static string SYSTEM_TITLE;
        public static string VIRTURE_DIRECT;
        
        //定义系统输出Log的级别
        //目前有4种级别，依次为Admin,Normal,None
        public static string LOGGING_FILE_NAME =string.Empty;
        public static Log_LoggingLevel LOGGING_LEVEL;
        public static string LOGGING_FILE_SIZE=string.Empty;

        //是否禁用行级数据安全控制
        public static bool IS_RSL_DISABLED = false;  

        //系统代号
        public static string apnm = "HRMS";
        #endregion

        #region HRMS

        public static string APPLICATION_NAME = string.Empty;
        public static string HRMS_CONNECTION_STRING = string.Empty;
        public static string ALARM_CONNECTION_STRING = string.Empty;
        public static string DAEMON_CONNECTION_STRING = string.Empty;

        //show import status
        public static Dictionary<string, ImportReportInfo> HRMS_IMPORT_REPORTS = new Dictionary<string, ImportReportInfo>();

        //system config
        public static object CURRENT_SYSTEM_CONFIG = new object();
        public static string CURRENT_USER_OPEN_MODE = string.Empty;  //default为Normal，表示只有HR人员才能进行操作
        #endregion

        public static object TABLE_DEFINITION = null;
    }
}
