using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GotWell.Common;
using GotWell.Model.Authorization;
using GotWell.Utility;
using System.Collections;
using GotWell.HRMS.HRMSBusiness.AuthorizationService;

namespace GotWell.HRMS.HRMSBusiness.Authorization
{
    public class AuthorizationBll
    {
        
        #region FOR GET AUTHORIZATION LIST
        #region GetAuthorization
        /// <summary>
        /// Purpose: 获取用户Authorization信息
        //                              <Authorization>
        //                                    <Action>ReturnAuthorizationList</Action>
        //                                    <User>EM01</User>
        //                                    <Application url='http://hrms' name='HRMS'>
        //                                        <Roles>
        //                                            <Role>F01</Role>
        //                                            <Role>F05</Role>
        //                                            <Role>F03</Role>
        //                                            <Role>F04</Role>
        //                                        </Roles>
        //                                        <Modules>
        //                                            <Module name='BasicInfo'>
        //                                                <Functions>
        //                                                    <Function id='F0101' name='' url=''>Allowed</Function>
        //                                                    <Function id='F0102' name='' url=''>Allowed</Function>
        //                                                </Functions>
        //                                            </Module>  
        //                                            <Module name='Interface'>
        //                                                <Functions>
        //                                                    <Function id='F0201' name='' url=''>Allowed</Function>
        //                                                    <Function id='F04' name='' url=''>Denied</Function>
        //                                                    <Function id='F05' name='' url=''>Allowed</Function>
        //                                                </Functions>
        //                                            </Module>   
        //                                        </Modules>   
        //                                    </Application>
        //                                </Authorization>";
        /// </summary>
        /// <param name="_userId"></param>
        /// <param name="_application"></param>
        /// <returns></returns>
        public AuthorizationMdl GetAuthorization(string _userId,string _application)
        {
            try
            {
                GotWell.HRMS.HRMSBusiness.AuthorizationService.AuthorizationServiceClient client = new GotWell.HRMS.HRMSBusiness.AuthorizationService.AuthorizationServiceClient();
                //AuthorizationService service = new AuthorizationService();
                string _ResultXmlText = client.GetAuthorizationXml(CreateParamter_GetAuthorization(_userId, _application));

                return ParseAuthorizationXml(_ResultXmlText);

            }catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
        }
        #endregion

        #region CreateAuthorizationParamter
        private string CreateParamter_GetAuthorization(string _userId, string _application)
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
        private AuthorizationMdl ParseAuthorizationXml(string _AuthXmlText)
        {
            AuthorizationMdl result = new AuthorizationMdl();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_AuthXmlText);
                XmlNodeList nodes;
                result.Action= doc.SelectSingleNode("/Authorization/Action").InnerText.ToString();

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

        #region FOR LOGON

        #region GetUserValidation
        //<Authorization>
        //     <Action>ReturnUserValidation</Action>
        //     <UserId></UserId>
       //      <Application></Application>
       //      <Message></Message>   
       //</Authorization>";
        public Hashtable GetUserValidation(string _userId, string _passwd, string _application)
        {
            try
            {
                GotWell.HRMS.HRMSBusiness.AuthorizationService.AuthorizationServiceClient client = new GotWell.HRMS.HRMSBusiness.AuthorizationService.AuthorizationServiceClient();
                string _ResultXmlText = client.GetUserValidationXML(CreateParamter_GetUserValidation(_userId, _passwd, _application));

                //Parse
                XmlDocument doc = new XmlDocument();
                Hashtable result = new Hashtable();

                doc.LoadXml(_ResultXmlText);
                result["Action"] = doc.SelectSingleNode("/Authorization/Action").InnerText.ToString();
                result["UserId"] = doc.SelectSingleNode("/Authorization/UserId").InnerText.ToString();
                result["Application"] = doc.SelectSingleNode("/Authorization/Application").InnerText.ToString();
                result["Message"] = doc.SelectSingleNode("/Authorization/Message").InnerText.ToString();

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

        #region createUserValidationParamter
        //<Authorization>
        //      <Action>GetUserValidation</Action>
        //      <Parameters>
        //          <Parameter name="UserId"></Paramter>
        //          <Parameter name="Password"></Paramter>
        //          <Parameter name="Application"></Paramter>
        //      </Parameters>
        //</Authorization>
        private string CreateParamter_GetUserValidation(string _userId, string _passwd, string _application)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlNode node;
                XmlText textNode;
                XmlAttribute attribute;

                XmlNode root = doc.CreateNode(XmlNodeType.Element, "Authorization", "");

                node = doc.CreateNode(XmlNodeType.Element, "Action", "");
                textNode = doc.CreateTextNode("GetUserValidation");
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
                attribute.Value = "Password";
                node.Attributes.Append(attribute);
                textNode = doc.CreateTextNode(_passwd);
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

        #endregion

        #region FOR GET USER INFORMATION
        #region GetUserInformation
        //<Authorization>
        //     <Action>ReturnUserInformtion</Action>
        //     <UserId></UserId>
        //      <Application></Application>
        //      <Message></Message>   
        //</Authorization>";
        public Hashtable GetUserInformation(string _userId,string _application)
        {
            try
            {
                AuthorizationServiceClient client = new AuthorizationServiceClient();
                string _ResultXmlText = client.GetUserInfomation(CreateParamter_GetUserInformation(_userId, _application));

                //Parse
                XmlDocument doc = new XmlDocument();
                Hashtable result = new Hashtable();

                doc.LoadXml(_ResultXmlText);
                result["Action"] = doc.SelectSingleNode("/Authorization/Action").InnerText.ToString();
                result["UserId"] = doc.SelectSingleNode("/Authorization/UserId").InnerText.ToString();
                result["Application"] = doc.SelectSingleNode("/Authorization/Application").InnerText.ToString();
                result["Message"] = doc.SelectSingleNode("/Authorization/Message").InnerText.ToString();

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

        #region CreateParamter_GetUserInformation
        //<Authorization>
        //      <Action>GetUserInformation</Action>
        //      <Parameters>
        //          <Parameter name="UserId"></Paramter>
        //          <Parameter name="Password"></Paramter>
        //          <Parameter name="Application"></Paramter>
        //      </Parameters>
        //</Authorization>
        private string CreateParamter_GetUserInformation(string _userId, string _application)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlNode node;
                XmlText textNode;
                XmlAttribute attribute;

                XmlNode root = doc.CreateNode(XmlNodeType.Element, "Authorization", "");

                node = doc.CreateNode(XmlNodeType.Element, "Action", "");
                textNode = doc.CreateTextNode("GetUserValidation");
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
        #endregion

        #region FOR CHANGE USER PASSWORD
        #region ChangeUserPassword
        //<Authorization>
        //     <Action>ReturnChangeUserPassword</Action>
        //     <UserId></UserId>
        //      <Application></Application>
        //      <Message></Message>   
        //</Authorization>";
        public Hashtable ChangeUserPassword(string _userId,string _oldpassword,string _newpassword, string _application)
        {
            try
            {
                AuthorizationServiceClient client = new AuthorizationServiceClient();
                string _ResultXmlText = client.ChangeUserPassword(CreateParamter_ChangeUserPassword(_userId,_oldpassword,_newpassword, _application));

                //Parse
                XmlDocument doc = new XmlDocument();
                Hashtable result = new Hashtable();

                doc.LoadXml(_ResultXmlText);
                result["Action"] = doc.SelectSingleNode("/Authorization/Action").InnerText.ToString();
                result["UserId"] = doc.SelectSingleNode("/Authorization/UserId").InnerText.ToString();
                result["Application"] = doc.SelectSingleNode("/Authorization/Application").InnerText.ToString();
                result["Message"] = doc.SelectSingleNode("/Authorization/Message").InnerText.ToString();

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

        #region CreateParamter_ChangeUserPassword
        //<Authorization>
        //      <Action>ChangeUserPassword</Action>
        //      <Parameters>
        //          <Parameter name="UserId"></Paramter>
        //          <Parameter name="OldPassword"></Paramter>
        //          <Parameter name="NewPassword"></Paramter>
        //          <Parameter name="Application"></Paramter>
        //      </Parameters>
        //</Authorization>
        private string CreateParamter_ChangeUserPassword(string _userId,string _oldpassword,string _newpassword, string _application)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlNode node;
                XmlText textNode;
                XmlAttribute attribute;

                XmlNode root = doc.CreateNode(XmlNodeType.Element, "Authorization", "");

                node = doc.CreateNode(XmlNodeType.Element, "Action", "");
                textNode = doc.CreateTextNode("ChangeUserPassword");
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
                attribute.Value = "OldPassword";
                node.Attributes.Append(attribute);
                textNode = doc.CreateTextNode(_oldpassword);
                node.AppendChild(textNode);
                paramtersNode.AppendChild(node);

                node = doc.CreateNode(XmlNodeType.Element, "Parameter", "");
                attribute = doc.CreateAttribute("name");
                attribute.Value = "NewPassword";
                node.Attributes.Append(attribute);
                textNode = doc.CreateTextNode(_newpassword);
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
        #endregion

    }
        
}
