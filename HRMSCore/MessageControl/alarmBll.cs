using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSCore.AlarmService;
using GotWell.Common;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSCore.MessageControl
{
    public class alarmBll : BaseBll
    {
        AlarmServiceClient client = new AlarmServiceClient();

        public tstalarm BuildAlarmMdl(Alarm_AlarmType _alty, string _subj, string _bdtx, string _reci, string _cc, string _mtyp,DateTime _extm,string _remk)
        {
            tstalarm obj = new tstalarm();
            obj.alty = _alty.ToString();
            obj.alid = Function.GetGUID();
            obj.alst = Alarm_AlarmStatus.Unhandled.ToString();
            obj.apnm = Parameter.APPLICATION_NAME;
            obj.bdtx = _bdtx;
            obj.cc = _cc==null?string.Empty:_cc;
            obj.crtm = DateTime.Now;
            obj.crur = Function.GetCurrentUser();
            obj.reci = _reci==null?"Unknown":_reci;
            obj.remk = _remk;
            obj.subj = _subj;
            obj.mtyp = _mtyp;
            obj.extm = _extm;

            return obj;
        }

        public void CreateAlarmForBoard(tstalarm obj)
        {
            try
            {
                string sText = @"<Alarms>
                               <Alarm>
                                   <AlarmType>" + obj.alty + @"</AlarmType>             
                                   <Subject>" + obj.subj + @"</Subject>
                                   <Recipients>" + obj.reci + @"</Recipients>           
                                   <CC>" + obj.cc + @"</CC>                           
                                   <BodyText>" + obj.bdtx + @"</BodyText>
                                   <Comments>" + obj.remk + @"</Comments>
                                   <Application>" + obj.apnm + @"</Application>
                                   <Creator>" + obj.crur + @"</Creator>
                                   <CreatedTime>" + obj.crtm.Value.ToString("yyyy-MM-dd HH:mm:ss") + @"</CreatedTime>
                                </Alarm>
                             </Alarms>";
                client.SendAlarm(sText);
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }

        public string BuildTrainingAlarmBody(ttrtraing tra,string emno)
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append("<>"

            string registerURL = "ContextInfo.contextPath+\'/trtratt.mvc/";
            sb.Append("Click <a href='http://" + registerURL + "register?trcd=" + tra.trcd + "&emno=" + emno + "'>here</a> to register the training.");
            sb.Append("<BR/>");

            return sb.ToString();
        }
    }
}
