using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using GotWell.Utility;
using GotWell.Common;
using System.Text;
using GotWell.LanguageResources;

namespace GotWell.HRMS.HRMSWeb.Common
{
    public class ExceptionPaser
    {

        public static string Parse(object _ex)
        {
            return subParse(_ex, string.Empty, string.Empty, false);
        }

        public static string Parse(object _ex, bool _colorIt)
        {
            return subParse(_ex, string.Empty, string.Empty, _colorIt);
        }

        public static string Parse(object _ex, string _additionalMessage)
        {
            return subParse(_ex, string.Empty, _additionalMessage, false);
        }

        public static string Parse(string _additionalMessage, object _ex)
        {
            return subParse(_ex, _additionalMessage, string.Empty, false);
        }

        public static string Parse(object _ex, string _additionalMessage, bool _colorIt)
        {
            return subParse(_ex, string.Empty, _additionalMessage, _colorIt);
        }

        public static string Parse(string _additionalMessage, object _ex, bool _colorIt)
        {
            return subParse(_ex, _additionalMessage, string.Empty, _colorIt);
        }

        private static string subParse(object _ex, string _additionalMessageFront, string _additionalMessageBack, bool _colorIt)
        {
            string sMessage = string.Empty;

            if (_ex.GetType().FullName == "GotWell.Utility.UtilException")
                sMessage = subParseUtilException((UtilException)_ex, _additionalMessageFront, _additionalMessageBack);
            else
                sMessage = subParseException((Exception)_ex, _additionalMessageFront, _additionalMessageBack);



            if (!sMessage.Equals(string.Empty))
            {
                if (_colorIt)
                {
                    sMessage = "<font color=red>" + sMessage + "</font>";
                }
            }

            return sMessage;
        }

        private static string subParseException(Exception _ex, string _additionalMessageFront, string _additionalMessageBack)
        {
            string sReturn = _ex.Message.EscapeHtml();

            if (!_additionalMessageFront.Equals(string.Empty))
                sReturn = _additionalMessageFront + "[" + sReturn + "]";

            if (!_additionalMessageBack.Equals(string.Empty))
                sReturn = sReturn + "[" + _additionalMessageFront + "]";

            return sReturn;
        }

        private static string subParseUtilException(UtilException _ex, string _additionalMessageFront, string _additionalMessageBack)
        {
            string sReturn = string.Empty;
            switch (_ex.Code)
            {
                case (int)Exception_ErrorMessage.CheckP208ProcessFail:
                    //sReturn = HRMSRes.P207_Message_ProcessFail + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP208MaterialFail:
                    //sReturn = HRMSRes.P207_Message_MaterialFail + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP208BusinessFail:
                    //sReturn = HRMSRes.P207_Message_BusinessFail + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP207ProcessFail:
                    //sReturn = HRMSRes.P207_Message_ProcessFail + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP207MaterialFail:
                    //sReturn = HRMSRes.P207_Message_MaterialFail + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP207BusenessFail:
                    //sReturn = HRMSRes.P207_Message_BusinessFail + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP303FailByWeight:
                    //sReturn = HRMSRes.P303_Message_FailForWeight + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]"; 
                    break;
                case (int)Exception_ErrorMessage.CheckP303FailByAmount:
                    //sReturn = HRMSRes.P303_Message_FailForAmount + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]"; 
                    break;
                case (int)Exception_ErrorMessage.PeriodIsClosedImportDenied:
                    sReturn = HRMSRes.Public_Message_PeriodIsClosed + "," + HRMSRes.Public_Message_ImportDenied;
                    break;
                case (int)Exception_ErrorMessage.PeriodIsUnused:
                    sReturn = HRMSRes.Public_Message_PeriodIsUnused;
                    break;
                case (int)Exception_ErrorMessage.ImportDataFail:
                    sReturn = HRMSRes.Public_Message_ImportBad + "," + _ex.Message + "";
                    break;
                case (int)Exception_ErrorMessage.ParameterError:
                    //sbReturn.Append(HRMSRes.Public_Message_PeriodIsClosed.Replace("%s1", ""));
                    break;
                case (int)Exception_ErrorMessage.CheckPreconditionFail:
                    sReturn = HRMSRes.Public_Message_CheckPreconditionFail + ",[<font color='blue'>" + HRMSRes.ResourceManager.GetString(_ex.Message) + "</font>]需要先执行。";
                    break;
                case (int)Exception_ErrorMessage.CheckP209BusinessFail1:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "[labor=" + Constant.P209_SQL_CONSTANT_004 + " and (gi_menge-menge-scrquantity-baddies)<>0]," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.OpenPeriodFoundOpenDenied:
                    sReturn = HRMSRes.Public_Message_OpenPeriodFoundOpenDenied + "[" + _ex.Message + "]";
                    break;
                case (int)Exception_ErrorMessage.NoDataToCarryForward:
                    sReturn = HRMSRes.Public_Message_NoDataToCarryForward;
                    break;
                case (int)Exception_ErrorMessage.StepCompletedCarryForwardDenied:
                    sReturn = HRMSRes.Public_Message_StepCompletedCarryForwardDenied;
                    break;
                case (int)Exception_ErrorMessage.ResetIsUnnecessary:
                    sReturn = HRMSRes.Public_Message_ResetIsUnnecessary;
                    break;
                case (int)Exception_ErrorMessage.CheckP302BusinessFail1:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP302BusinessFail2:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP301BusinessFail:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP301BusinessFail1:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP304BusinessFail1:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP304BusinessFail2:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP306BusinessFail1:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP306BusinessFail2:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.P505SAPFeedBackError:
                    //sReturn = HRMSRes.Public_Message_SAPFeedBackError + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.TraceFileName) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP210BusinessFail1:
                    //sReturn = HRMSRes.Public_Message_DataCheckFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.StepIsCompleted:
                    sReturn = HRMSRes.Public_Message_StepCompleted;
                    break;
                case (int)Exception_ErrorMessage.StepIsLocked:
                    sReturn = HRMSRes.Public_Message_StepLocked;
                    break;
                case (int)Exception_ErrorMessage.CheckP205BusinessFail:
                    //sReturn = HRMSRes.P205_Message_BusinessFail + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP504BusinessFail:
                    //sReturn = HRMSRes.Public_Message_CheckWarning + "," + HRMSRes.Public_Message_CheckTraceFile + " [" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case (int)Exception_ErrorMessage.CheckP506ResetFail:
                    //sReturn = HRMSRes.Public_Message_ResetAllFail + "[" + HRMSRes.Public_Message_Revise + "]";
                    break;
                case (int)Exception_ErrorMessage.NoReportData:
                    sReturn = HRMSRes.Public_PagingToolbar_EmptyMsg;
                    break;
                case 20003:
                    //sReturn = HRMSRes.P301_Message_001 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20004:
                    //sReturn = HRMSRes.P301_Message_002 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20005:
                    //sReturn = HRMSRes.P301_Message_003 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20006:
                    //sReturn = HRMSRes.P301_Message_004 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20007:
                    //sReturn = HRMSRes.P301_Message_005 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20008:
                    //sReturn = HRMSRes.P301A_Message_005 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20009:
                    //sReturn = HRMSRes.P301A_Message_001 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20010:
                    //sReturn = HRMSRes.P301A_Message_006 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20011:
                    //sReturn = HRMSRes.P301A_Message_002 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20012:
                    //sReturn = HRMSRes.P301A_Message_007 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20013:
                    //sReturn = HRMSRes.P301A_Message_003 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 20015:
                    //sReturn = HRMSRes.P301A_Message_004 + HRMSRes.Public_Message_CheckTrace + "[" + AddLinkToTrace(_ex.Message) + "]";
                    break;
                case 2291: //Oracle Exception: FK violation
                    //sReturn = _ex.Message + "[" + AddLinkToTrace(_ex.TraceFileName) + "]";
                    break;
                case 1: //Oracle Exception: Unique violation
                    sReturn = _ex.Message + "[" + AddLinkToTrace(_ex.TraceFileName) + "]";
                    break;
                case 1400://Oracle Exception: Cannot insert null
                    sReturn = _ex.Message + "[" + AddLinkToTrace(_ex.TraceFileName) + "]";
                    break;
                default:
                    sReturn = _ex.Message;
                    break;
            } 

            if (!_additionalMessageFront.Equals(string.Empty))
                sReturn = _additionalMessageFront + "[" + sReturn + "]";

            if (!_additionalMessageBack.Equals(string.Empty))
                sReturn = sReturn + "[" + _additionalMessageFront + "]";

            return sReturn.ToString().EscapeHtml();
        }

        public static string AddLinkToTrace(string _fileName)
        {
            string menu = "M9030";
            string url = "ContextInfo.contextPath+\'/trace.mvc/index?menuId=" + menu + "&fileName=" + _fileName + "\'";
            string title = HRMSRes.Public_Menu_TraceViewer;
            string method = "javascript:mainPanel.loadClass(" + url + ",\'" + menu + "\',\'\',\'" + title + "\',\'\',true)";
            string s = "<a href=" + method + ">" + _fileName + "</a>";
            return s;
        }

    }
}
