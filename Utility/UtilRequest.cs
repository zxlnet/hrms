using System;
using System.Web;

namespace GotWell.Utility
{
    public class UtilRequest
    {
        public static void saveCookie(HttpRequestBase request, HttpResponseBase response,string entry, string name, string value)
        {
            HttpCookie cookie = request.Cookies[entry];
            if (cookie == null)
            {
                cookie = new HttpCookie(entry);
                cookie.Expires = DateTime.Now.AddYears(10);                
            }           
            cookie[name] = value;
            response.Cookies.Add(cookie);
        }

        public static string getCookieValue(HttpRequestBase request,string entry, string name)
        {
            if (request.Cookies[entry] != null)
            {
                if (request.Cookies[entry][name] != null)
                {
                    return request.Cookies[entry][name];
                }                
            }
            return null;
        }

        public static void WriteExcelFile(HttpResponseBase response,string output,string fileName)
        {
            response.Clear();
            response.Buffer = true;
            response.Charset = "utf-8";
            response.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.ContentType = "application/ms-excel";
            response.Write(output);
        }
    }
}
