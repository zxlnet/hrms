/**********************************************************************/
/*
 * Define system constanct in this class
 * 
/**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace GotWell.Common
{
    public class Constant
    {

        //Session名称
        public const string SESSION_CULTURE = "Culture";
        public const string SESSION_AUTHORIZATION = "Authorization";
        public const string SESSION_CURRENT_USER = "CurrentUser";
        public const string SESSION_CURRENT_STAFF = "CurrentStaff";
        public const string SESSION_CURRENT_MUF = "CurrentMUF";

        //系统参数

        //系统内的值常量
        public const string APPLICATION_ALL = "ALL";

        public const string SPLITLABEL = "@@@";
        public const string SPLITDBERRORMSG = "@@";
        public const string SPLITDATAFIELD = "|";
        public const string NULLSTRING = "NA";



        /*******************Daemon Section************************/
        public static string JOB_DATAMAP_MODEL = "MDL";
        public static string JOB_DATAMAP_LOG = "LOG";

        /*******************HRMS Section************************/
        public const string SYSTEM_USER_ID = "System";
    }
}
