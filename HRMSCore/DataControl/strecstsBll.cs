using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.HRMS;
using GotWell.Common;
using GotWell.HRMS.HRMSCore.MessageControl;
using GotWell.Model.Common;
using System.Transactions;
using System.Reflection;

namespace GotWell.HRMS.HRMSCore.DataControl
{
    public class strecstsBll : BaseBll
    {
        public void AddRecStatusToObject(object _obj, string _action,string _lgtx,out tstalarm alarmMdl)
        {
            tstrecst recst = new tstrecst();
            recst.actn = _action;
            recst.attm = DateTime.Now;
            recst.atur = Function.GetCurrentUser();
            recst.ownr = Function.GetCurrentUser();
            recst.rfid = _obj.GetType().GetProperty("rfid").GetValue(_obj, null).ToString();
            recst.sttm = DateTime.Now;
            recst.stus = "Unconfirmed";
            recst.cftm = null;
            recst.cfur = null;

            //_obj.GetType().GetProperty("rfid").SetValue(_obj, recst.rfid, null);
            _obj.GetType().GetProperty("tstrecst").SetValue(_obj, recst, null);

            string emno = _obj.GetType().GetProperty("emno").GetValue(_obj, null).ToString();
            alarmMdl = new alarmBll().BuildAlarmMdl(Alarm_AlarmType.Board,
                        "[" + _action + "]" + _obj.GetType().Name, _lgtx, 
                        new stalsubsBll().GetRecipicientsBoard(emno), 
                        string.Empty, "Adding",DateTime.Now.AddDays(1),string.Empty);
        }

        public void UpdateRecStatusToObject(object _obj, string _action,string _lgtx, out tstalarm alarmMdl)
        {
            PropertyInfo prop = _obj.GetType().GetProperty("tstrecst");
            tstrecst rec = prop.GetValue(_obj, null) as tstrecst;
            rec.actn = "Update";
            rec.attm = DateTime.Now;
            rec.atur = Function.GetCurrentUser();
            rec.stus = "Unconfirmed";

            string emno = _obj.GetType().GetProperty("emno").GetValue(_obj, null).ToString();
            alarmMdl = new alarmBll().BuildAlarmMdl(Alarm_AlarmType.Board,
                        "[" + _action + "] " + _obj.GetType().Name, _lgtx, 
                        new stalsubsBll().GetRecipicientsBoard(emno),
                        string.Empty,"Updating",DateTime.Now.AddDays(1), string.Empty);
        }

        public void ConfirmRecord(string rfid)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rfid", ColumnValue = rfid } };
                    tstrecst obj = GetSelectedObject<tstrecst>(parameters);

                    if (obj != null)
                    {
                        obj.cftm = DateTime.Now;
                        obj.cfur = Function.GetCurrentUser();
                        obj.stus = "Confirmed";

                        DoUpdate<tstrecst>(obj);
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
