using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Common;

namespace GotWell.Model.HRMS
{
    public class TransactionLogMdl
    {

        #region Model
        public string TxLog_SysID { get; set; }
        public string TxLog_Source { get; set; }
        public Transaction_OperationCategory Opertion_Category { get; set; }
        public string Operation_Desc { get; set; }
        public string Reason { get; set; }
        public string Event_User { get; set; }
        public DateTime Event_Time { get; set; }
        public string Remark { get; set; }
        #endregion

        #region Construct
        public TransactionLogMdl()
        {
            TxLog_SysID = string.Empty;
            TxLog_Source = string.Empty;
            Opertion_Category = Transaction_OperationCategory.Add;//
            Reason = string.Empty;
            Event_User = string.Empty;
            Event_Time = Function.GetNullDateTime();
            Remark = string.Empty;
        }
        #endregion
    }
}
