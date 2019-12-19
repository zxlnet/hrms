using System;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace GotWell.Utility
{
    public sealed class UtilCulture
    {
        #region Definition

        private static ResourceManager rm;

        #endregion Definition

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        private UtilCulture()
        {
        }

        #endregion

        #region GetString

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Resource Id</param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            string s1;
            try
            {
                s1 = rm.GetString(key);
            }
            catch (Exception ex)
            {
                s1 = "---";
            }
            return s1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Resource Id</param>
        /// <param name="p1">Replace String 1</param>
        /// <returns></returns>
        public static string GetString(string key, string p1)
        {
            string s1;
            try
            {
                s1 = rm.GetString(key);
                s1 = s1.Replace("%s1", p1);
            }
            catch (Exception)
            {
                s1 = "---";
            }
            return s1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Resource Id</param>
        /// <param name="p1">Replace string 1</param>
        /// <param name="p2">Replace string 2</param>
        /// <returns></returns>
        public static string GetString(string key, string p1,string p2)
        {
            string s1;
            try
            {
                s1 = rm.GetString(key);
                s1 = s1.Replace("%s1", p1);
                s1 = s1.Replace("%s2", p2);
            }
            catch (Exception)
            {
                s1 = "---";
            }
            return s1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Resource Id</param>
        /// <param name="p1">Replace string 1</param>
        /// <param name="p2">Replace string 2</param>
        /// <param name="p3">Replace string 3</param>
        /// <returns></returns>
        public static string GetString(string key, string p1, string p2,string p3)
        {
            string s1;
            try
            {
                s1 = rm.GetString(key);
                s1 = s1.Replace("%s1", p1);
                s1 = s1.Replace("%s2", p2);
                s1 = s1.Replace("%s3", p3);
            }
            catch (Exception)
            {
                s1 = "---";
            }
            return s1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Resource Id</param>
        /// <param name="p1">Replace string 1</param>
        /// <param name="p2">Replace string 2</param>
        /// <param name="p3">Replace string 3</param>
        /// <param name="p4">Replace string 4</param>
        /// <returns></returns>
        public static string GetString(string key, string p1, string p2,string p3,string p4)
        {
            string s1;
            try
            {
                s1 = rm.GetString(key);
                s1 = s1.Replace("%s1", p1);
                s1 = s1.Replace("%s2", p2);
                s1 = s1.Replace("%s3", p3);
                s1 = s1.Replace("%s4", p4);
            }
            catch (Exception)
            {
                s1 = "---";
            }
            return s1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Resource Id</param>
        /// <param name="p1">Replace string 1</param>
        /// <param name="p2">Replace string 2</param>
        /// <param name="p3">Replace string 3</param>
        /// <param name="p4">Replace string 4</param>
        /// <param name="p5">Replace string 5</param>
        /// <returns></returns>
        public static string GetString(string key, string p1, string p2, string p3, string p4,string p5)
        {
            string s1;
            try
            {
                s1 = rm.GetString(key);
                s1 = s1.Replace("%s1", p1);
                s1 = s1.Replace("%s2", p2);
                s1 = s1.Replace("%s3", p3);
                s1 = s1.Replace("%s4", p4);
                s1 = s1.Replace("%s4", p5);
            }
            catch (Exception)
            {
                s1 = "---";
            }
            return s1;
        }

        #endregion

        #region InitialResource
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName">Resource/Language Name</param>
        /// <param name="resourcePath"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static bool InitialResource(string resourceName, string resourcePath, string culture)
        {
            bool b1;
            try
            {
                rm = ResourceManager.CreateFileBasedResourceManager(resourceName + "." + culture, resourcePath,null);
                //rm = new ResourceManager(resourceName + "." + culture,Assembly.GetExecutingAssembly(),null);
                //b1 = SetCultureInfo(culture);

                b1 = true;
            }
            catch (UtilException)
            {
                b1 = false;
            }
            return b1;
        }
        #endregion

        #region SetCultureInfo
        /// <summary>
        /// 
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static bool SetCultureInfo(string culture)
        {
            bool b1;
            try
            {
                CultureInfo cultureInfo = new CultureInfo(culture);
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                b1 = true;
            }
            catch (UtilException)
            {
                b1 = false;
            }
            return b1;
        }
        #endregion

    }
}
