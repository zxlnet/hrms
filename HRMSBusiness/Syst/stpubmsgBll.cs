using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Syst;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Common;
using GotWell.HRMS.HRMSCore.MessageControl;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Syst
{
    public class stpubmsgBll : BaseBll
    {
        stpubmsgDal dal = null;

        public stpubmsgBll()
        {
            dal = new stpubmsgDal();
            baseDal = dal;
        }

        public string GetNewSMID()
        {
            return dal.GetNewSMID();
        }

        public void Publish(List<ColumnInfo> lstParameters)
        {
            try
            {
                tstpubmsg msg = GetSelectedObject<tstpubmsg>(lstParameters);

                if (msg != null)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        msg.ispb = "Y";
                        msg.pbtm = DateTime.Now;
                        msg.pbur = Function.GetCurrentUser();

                        DoUpdate<tstpubmsg>(msg, lstParameters);

                        //generate a message to publish
                        tstalarm alarm = new tstalarm();
                        alarm.alid = Function.GetGUID();
                        alarm.alst = Alarm_AlarmStatus.Unhandled.ToString();
                        alarm.alty = Alarm_AlarmType.Board.ToString();
                        alarm.apnm = Function.GetCurrentApplication();
                        alarm.bdtx = msg.bdtx;
                        alarm.cc = string.Empty;
                        alarm.crtm = DateTime.Now;
                        alarm.crur = Function.GetCurrentUser();
                        alarm.extm = alarm.crtm.Value.AddDays(7);
                        alarm.mtyp = Alarm_MessageType.PublicMessage.ToString();
                        alarm.reci = "Public";
                        alarm.subj = msg.subj;

                        alarmBll bll = new alarmBll();
                        bll.CreateAlarmForBoard(alarm);

                        scope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
