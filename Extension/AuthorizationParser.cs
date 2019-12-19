//======================================================================
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GotWell.Common;
using GotWell.Model.Authorization;
using GotWell.Utility;

namespace GotWell.Extension
{
    public class AuthorizationParser
    {
        #region 获取Authorization信息

        #region createAuthorizationParamter
        private string createAuthorizationParamter(string _userId, string _application)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlNode node;
                XmlText textNode;
                XmlAttribute attribute;

                XmlNode root = doc.CreateNode(XmlNodeType.Element, "Authorization", "");

                node = doc.CreateNode(XmlNodeType.Element, "Action", "");
                textNode = doc.CreateTextNode("GetAuthorizationList");
                node.AppendChild(textNode);
                root.AppendChild(node);

                XmlNode paramtersNode = doc.CreateNode(XmlNodeType.Element, "Parameters", "");

                node = doc.CreateNode(XmlNodeType.Element, "Parameter", "");
                attribute = doc.CreateAttribute("name");
                attribute.Value = "UserId";
                node.Attributes.Append(attribute);
                textNode = doc.CreateTextNode(_userId);
                node.AppendChild(textNode);
                paramtersNode.AppendChild(node);

                node = doc.CreateNode(XmlNodeType.Element, "Parameter", "");
                attribute = doc.CreateAttribute("name");
                attribute.Value = "Application";
                node.Attributes.Append(attribute);
                textNode = doc.CreateTextNode(_application);
                node.AppendChild(textNode);
                paramtersNode.AppendChild(node);

                root.AppendChild(paramtersNode);

                doc.AppendChild(root);
                return doc.OuterXml;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }
        #endregion

        #region ParseAuthorizationXml
        /// <summary>
        /// Purpose: 将Authorization Xml String转换成Authorization对象
        /// </summary>
        /// <param name="_AuthXmlText"></param>
        /// <returns></returns>
        public AuthorizationMdl ParseAuthorizationXml(string _AuthXmlText)
        {
            AuthorizationMdl result = new AuthorizationMdl();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_AuthXmlText);
                XmlNodeList nodes;
                result.Action = doc.SelectSingleNode("/Authorization/Action").InnerText.ToString();

                //result.User = doc.SelectSingleNode("/Authorization/User").InnerText.ToString();
                XmlNode userNode = doc.SelectSingleNode("/Authorization/User");
                UserMdl user = new UserMdl();
                user.urid = userNode.Attributes["urid"].Value.ToString();
                user.urnm = userNode.Attributes["urnm"].Value.ToString();
                user.sfid = userNode.Attributes["sfid"].Value.ToString();
                result.User = user;

                XmlNodeList AppNodes = doc.SelectNodes("/Authorization/Application");
                List<AppMdl> applications = new List<AppMdl>();
                for (int i = 0; i < AppNodes.Count; i++)
                {
                    AppMdl appMdl = new AppMdl();
                    appMdl.apnm = AppNodes[i].Attributes["name"].Value;
                    appMdl.Web_Url = AppNodes[i].Attributes["url"].Value;
                    nodes = AppNodes[i].SelectNodes("Roles/Role");
                    List<RoleMdl> roles = new List<RoleMdl>();
                    for (int j = 0; j < nodes.Count; j++)
                    {
                        RoleMdl roleMdl = new RoleMdl();
                        roleMdl.roty = nodes[j].Attributes["roty"].Value;
                        roleMdl.Is_System_Role = (Public_Flag)Enum.Parse(typeof(Public_Flag), nodes[j].Attributes["issr"].Value);
                        roleMdl.Role_Id = nodes[j].Attributes["roid"].Value;
                        roleMdl.Role_Name = nodes[j].Attributes["ronm"].Value;
                        roleMdl.alep = nodes[j].Attributes["alep"].Value;
                        roles.Add(roleMdl);
                    }
                    appMdl.Roles = roles;
                    nodes = AppNodes[i].SelectNodes("Modules/Module");
                    List<ModuleMdl> modules = new List<ModuleMdl>();
                    for (int j = 0; j < nodes.Count; j++)
                    {
                        ModuleMdl module = new ModuleMdl();
                        module.Name = nodes[j].Attributes["name"].Value;
                        XmlNodeList fNodes = nodes[j].SelectNodes("Functions/Function");
                        List<FunctionMdl> functions = new List<FunctionMdl>();
                        for (int k = 0; k < fNodes.Count; k++)
                        {
                            FunctionMdl funtion = new FunctionMdl();
                            funtion.Id = fNodes[k].Attributes["id"].Value;
                            funtion.Name = fNodes[k].Attributes["name"].Value;
                            funtion.Url = fNodes[k].Attributes["url"].Value;
                            funtion.Permission = (Security_Permission_Type)Enum.Parse(typeof(Security_Permission_Type), fNodes[k].InnerText.ToString());
                            functions.Add(funtion);
                        }
                        module.Functions = functions;
                        modules.Add(module);
                    }
                    appMdl.Modules = modules;

                    applications.Add(appMdl);

                }
                result.Applications = applications;

                return result;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }

        }
        #endregion
        #endregion
    }
}


//XML Format
//<Authorization>
//    <Action>ReturnAuthorizationList</Action>
//    <User>EM01</User>
//    <Application url='http://hrms' name='HRMS'>
//        <Roles>
//            <Role>F01</Role>
//            <Role>F05</Role>
//            <Role>F03</Role>
//            <Role>F04</Role>
//        </Roles>
//        <Modules>
//            <Module name='BasicInfo'>
//                <Functions>
//                    <Function id='F0101' name='' url=''>Allowed</Function>
//                    <Function id='F0102' name='' url=''>Allowed</Function>
//                </Functions>
//            </Module>  
//            <Module name='Interface'>
//                <Functions>
//                    <Function id='F0201' name='' url=''>Allowed</Function>
//                    <Function id='F04' name='' url=''>Denied</Function>
//                    <Function id='F05' name='' url=''>Allowed</Function>
//                </Functions>
//            </Module>   
//        </Modules>   
//    </Application>
//</Authorization>";