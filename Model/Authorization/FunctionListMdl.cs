using System;
using System.Collections.Generic;
using System.Text;
using GotWell.Common;

namespace GotWell.Model.Authorization
{
    /// <summary>
    /// ʵ����T_SECFUNCTIONLIST ��(����˵���Զ���ȡ���ݿ��ֶε�������Ϣ)
    /// </summary>
    public class FunctionListMdl
    {       
		#region Model
        public string Func_Id { set; get; }
        public string apnm { set; get; }
        public string Func_Name { set; get; }
        public string Chinese_Desc { set; get; }
        public string English_Desc { set; get; }
        public string leno { set; get; }
        public Public_Status Func_Status { set; get; }
        public string Module { set; get; }
        public string Parent_Func_Id { set; get; }
        public string Func_Url { set; get; }
		#endregion Model
    }
}
