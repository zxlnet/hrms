using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GotWell.Utility
{
    public class UtilString
    {
        public static string EscapeHtml(string html)
        {
            return Regex.Escape(html).Replace("'", "\\'").Replace("\r\n", "\\r\\n");
        }
    }
}
