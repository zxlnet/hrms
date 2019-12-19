using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using GotWell.HRMS.HRMSWeb.Common;
using GotWell.Common;

namespace GotWell.HRMS.HRMSWeb.ExtJS.resources
{
    public class HelpController : Controller
    {
        public void Help()
        {
            try
            {
                string tabId = this.Request["record"];
                string helpFile = AppDomain.CurrentDomain.BaseDirectory + @"\Help\Help_" + tabId + ".xml";
                this.Response.Write(getHelpContent(helpFile));
            } 
            catch (Exception ex)
            {
                this.Response.Write(ExceptionPaser.Parse(ex,true));
            }            
        }

        private string getHelpContent(string xmlFile)
        {
            string content = string.Empty;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNode rootNode = xmlDoc.DocumentElement;
            object language =this.HttpContext.Session[Constant.SESSION_CULTURE];
            string lang = language != null ? language.ToString().ToLower() : "zh-cn";
            string langNode = lang.Equals("zh-cn") ? "Chinese" : "English";
            XmlNode contentNode = rootNode.SelectSingleNode(langNode);
            if (contentNode != null)
            {
                content = contentNode.InnerText;
            }
            return content;            
        }
    }
}
