/**********************************************************************/
/*
 * Define enum,structure,type in this class
 * 
/**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace GotWell.Common
{

    #region Parameter Item
    [Serializable]
    public class ParameterItem
    {
        public ParameterItem()
        {
        }

        public string ParameterName;
        public object ParameterValue;

        public ParameterItem(string parameter_name, object parameter_value)
        {
            ParameterName = parameter_name;
            ParameterValue = parameter_value;
        }
    }
    #endregion Parameter Item

    #region ReturnValue
    public class ReturnValue
    {
        private int _code;
        private string _text;

        public int Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public ReturnValue()
        {
        }

        public ReturnValue(int code)
        {
            _code = code;
            _text = "";
        }

        public ReturnValue(string text)
        {
            _code = -1;
            _text = text;
        }

        public ReturnValue(int code, string text)
        {
            _code = code;
            _text = text;
        }
    }

    #endregion Return Value
    
    #region Page
    public class PageInfo
    {
        public string PageId { get; set; }
        public string PageName { get; set; }
        public string CurrentUserId { get; set; }

        public PageInfo()
        {
        }

        public PageInfo(string _pageId)
        {
            PageId = _pageId;
            ////Update page name with resource id
            //PageName = "Public_Menu_" + PageId;
        }

        public PageInfo(string _pageId, string _pageName,string _currentUserId)
        {
            PageId = _pageId;
            PageName = _pageName;
            CurrentUserId = _currentUserId;

            ////Update page name with resource id
            //PageName = "Public_Menu_" + PageId;
        }
    }
    #endregion

    #region ImportReportInfo
    public class ImportReportInfo
    {
        public int TotalCount { get; set; }
        public int CurrentCount { get; set; }
        public HRMS_Import_Status Status { get; set; }

        public ImportReportInfo()
        {
            TotalCount = 0;
            CurrentCount = 0;
            Status = HRMS_Import_Status.Unknown;
        }
    }
    #endregion
}
