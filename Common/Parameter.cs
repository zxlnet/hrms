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
        //����ϵͳ�ı���
        public static string SYSTEM_TITLE;
        public static string VIRTURE_DIRECT;
        
        //����ϵͳ���Log�ļ���
        //Ŀǰ��4�ּ�������ΪAdmin,Normal,None
        public static string LOGGING_FILE_NAME =string.Empty;
        public static Log_LoggingLevel LOGGING_LEVEL;
        public static string LOGGING_FILE_SIZE=string.Empty;

        //�Ƿ�����м����ݰ�ȫ����
        public static bool IS_RSL_DISABLED = false;  

        //ϵͳ����
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
        public static string CURRENT_USER_OPEN_MODE = string.Empty;  //defaultΪNormal����ʾֻ��HR��Ա���ܽ��в���
        #endregion

        public static object TABLE_DEFINITION = null;
    }
}
