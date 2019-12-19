/**********************************************************************/
/*
 * Define public functions used in solution in this class
 * 
/**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;

namespace GotWell.Common
{
    public class Function
    {
        /// <summary>
        /// Get a new GUID
        /// </summary>
        /// <returns></returns>
        public static string GetGUID()
        {
            return System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Get MSSql sysdate, used in SQL string
        /// </summary>
        /// <returns></returns>
        public static string GetMSSqlSystemDate()
        {
            return "getdate()";
        }

        /// <summary>
        /// Get Oracle sysdate, used in SQL string
        /// </summary>
        /// <returns></returns>
        public static string GetOracleSystemDate()
        {
            return "sysdate";
        }

        /// <summary>
        /// Get system time, you can format it here
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// If the value of datatime field is empty, 
        /// replace it with '1900-01-01 00:00:00'
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNullDateTime()
        {
            return DateTime.Parse("1900-01-01 00:00:00");
        }

        public static DateTime GetMaxDateTime()
        {
            return DateTime.Parse("2050-01-01 00:00:00");
        }


        public static DateTime GetNullableDateTime()
        {
            Nullable<DateTime> nullDate = null;
            return Convert.ToDateTime(nullDate);
        }

        /// <summary>
        /// if the value of datatime field is '1900-01-01 00:00:00'
        /// replace it with null
        /// </summary>
        /// <param name="_ds"></param>
        public static void SetNullDateTime(DataSet _ds)
        {
            for (int i = 0; i < _ds.Tables[0].Rows.Count; i++)
            {
                for (int j = 0; j < _ds.Tables[0].Columns.Count; j++)
                {
                    object obj = _ds.Tables[0].Rows[i][j];
                    if (obj is DateTime && (obj.Equals(Function.GetNullDateTime()) || obj.Equals(DateTime.MinValue)))
                    {
                        _ds.Tables[0].Rows[i][j] = DBNull.Value;
                    }
                    
                }
            }
        }

        /// <summary>
        /// Return current login user, from AD
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUser()
        {
            string user = HttpContext.Current.Session[Constant.SESSION_CURRENT_USER].ToString();
            if (!user.Equals(string.Empty))
            {
                int index=user.IndexOf("\\");
                if (index>= 0)
                {
                    user = user.Substring(index + 1);
                }
                return user;
            }
            return "";
        }

        public static string GetCurrentStaff()
        {
            return HttpContext.Current.Session[Constant.SESSION_CURRENT_STAFF].ToString();
        }

        public static string GetExceptionMsg(string str)
        {
            string result = str;
            if (result.IndexOf("@@") >= 0)
            {
                result = Regex.Split(result,"@@")[1];
            }
            return result;
        }

        public static string GetCurrentApplication()
        {
            return Parameter.APPLICATION_NAME;
        }
    }
}
