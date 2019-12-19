using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Training;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Common;
using System.Transactions;
using GotWell.HRMS.HRMSCore.MessageControl;

namespace GotWell.HRMS.HRMSBusiness.Training
{
    public class trtraingBll : BaseBll
    {
        trtraingDal dal = null;

        public trtraingBll()
        {
            dal = new trtraingDal();
            baseDal = dal;
        }

        public void Publish(List<vw_employment> lstEmp, string trcd,string isEmail,string isBoard)
        {
            try
            {
                List<ColumnInfo> lstParameters = new List<ColumnInfo>()
            {
                new ColumnInfo(){ColumnName="trcd",ColumnValue=trcd}
            };

                ttrtraing tra = new BaseBll().GetSelectedObject<ttrtraing>(lstParameters);

                if (tra == null) return;

                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < lstEmp.Count; i++)
                    {
                        vw_employment emp = lstEmp[i];

                        ttrtraatt att = new ttrtraatt();
                        att.emno = emp.emno;
                        att.isat = "N";
                        att.isrg = "N";
                        att.lmtm = DateTime.Now;
                        att.lmur = Function.GetCurrentUser();
                        att.trcd = trcd;

                        DoInsert<ttrtraatt>(att);

                        alarmBll alaBll = new alarmBll();
                        if (isEmail == "Y")
                        {
                            //发Email
                            tstalarm alarmMdl = alaBll.BuildAlarmMdl(Alarm_AlarmType.Email, "Training: " + tra.trnm,
                                alaBll.BuildTrainingAlarmBody(tra,att.emno), emp.emno, string.Empty,
                                "Training", DateTime.Now.AddDays(1), string.Empty);

                            DoInsert<tstalarm>(alarmMdl);
                        }

                        if (isBoard == "Y")
                        {
                            //发送Board
                            tstalarm alarmMdl = alaBll.BuildAlarmMdl(Alarm_AlarmType.Board, "Training: " + tra.trnm,
                                alaBll.BuildTrainingAlarmBody(tra,att.emno), emp.emno, string.Empty,
                                "Training", DateTime.Now.AddDays(1), string.Empty);

                            DoInsert<tstalarm>(alarmMdl);
                        }

                        //update ispb标识
                        tra.ispb = "Y";

                        DoUpdate<ttrtraing>(tra,lstParameters);
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
