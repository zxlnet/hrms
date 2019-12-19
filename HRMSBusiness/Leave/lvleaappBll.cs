using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.Utility;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using System.Transactions;
using GotWell.Common;
using GotWell.HRMS.HRMSCore.MessageControl;

namespace GotWell.HRMS.HRMSBusiness.Leave
{
    public class lvleaappBll : BaseBll
    {
        lvleaappDal dal = null;

        public lvleaappBll()
        {
            dal = new lvleaappDal();
            baseDal = dal;
        }

        public string getNewAppNo()
        {
            try
            {
                //得到最大请假单号
                string maxNo = dal.getMaxAppNo();
                string nextNo = string.Empty;

                if (maxNo.Equals(string.Empty))
                    nextNo = UtilDatetime.FormatDate3(DateTime.Now) + ("1").PadLeft(3, '0');
                else
                    nextNo = Convert.ToString(Convert.ToDouble(maxNo) + 1);

                return nextNo;
            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
        }

        public void SaveLeaveApplication(tlvleaapp _leaveApp)
        {
            try
            {
                //分析请假并保存
                lvanalevBll bll = new lvanalevBll();

                List<ColumnInfo> dateParameters = new List<ColumnInfo>();
                dateParameters.Add(new ColumnInfo() { ColumnName = "lvstart", ColumnValue = UtilDatetime.FormateDateTime1(_leaveApp.frtm) });
                dateParameters.Add(new ColumnInfo() { ColumnName = "lvend", ColumnValue = UtilDatetime.FormateDateTime1(_leaveApp.totm), ColumnType = "datetime" });

                List<ColumnInfo> personParameters = new List<ColumnInfo>();
                personParameters.Add(new ColumnInfo() { ColumnName = "emp.emno", ColumnValue = _leaveApp.emno });

                using (TransactionScope scope = new TransactionScope())
                {
                    DoInsert<tlvleaapp>(_leaveApp);

                    //如果审核过，则分析休假，并生成leave detail
                    if (_leaveApp.lvst == "Approved")  //Approved
                        bll.AnalyzeLeave(dateParameters, personParameters, _leaveApp, true);

                    //发送Alarm
                    CreateAlarmForLeave(_leaveApp);

                    scope.Complete();
                }
            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }

        }

        private void CreateAlarmForLeave(tlvleaapp _leaveApp)
        {
            vw_employment emp = GetEmploymee(_leaveApp.emno);

            tstalarm alarmMdl = new tstalarm();
            alarmMdl.alid = Function.GetGUID();
            alarmMdl.alst = "Unhandled";
            alarmMdl.alty = "Board";
            alarmMdl.apnm = Parameter.APPLICATION_NAME;
            alarmMdl.bdtx = BuildAlarmBodyForLeaveApplication(_leaveApp,emp);
            alarmMdl.cc = string.Empty;
            alarmMdl.crtm = DateTime.Now;
            alarmMdl.crur = Function.GetCurrentUser();
            alarmMdl.extm = DateTime.Now.AddDays(30); //will expire in 30 days.
            alarmMdl.mtyp = "Leave Application";
            alarmMdl.reci = new stalsubsBll().GetRecipicientsBoard(emp.emno);
            alarmMdl.remk = _leaveApp.remk;
            alarmMdl.subj = emp.ntnm.Trim() + "'s Leave Application [" + _leaveApp.apno + "]";

            if (alarmMdl.reci == string.Empty) alarmMdl.reci = "Unknown";

            DoInsert<tstalarm>(alarmMdl);

        }

        private string BuildAlarmBodyForLeaveApplication(tlvleaapp _leaveApp, vw_employment emp)
        {
            string bdtx = string.Empty;
            bdtx += "&lt;LeaveApplication&gt;";  //<LeaveApplication>
            bdtx += "&lt;No&gt;" + _leaveApp.apno + "&lt;/No&gt;";  //No
            bdtx += "&lt;Employee&gt;" + emp.sfid + " - " + emp.ntnm + "&lt;/Employee&gt;";  //Employee
            bdtx += "&lt;Leave Type&gt;" + _leaveApp.tlvleatyp.ltnm + "&lt;/Leave Type&gt;";  //Leave Type
            bdtx += "&lt;Leave Reason&gt;" + _leaveApp.tlvlearsn.lrnm + "&lt;/Leave Reason&gt;";  //Leave Reason
            bdtx += "&lt;From Time&gt;" + UtilDatetime.FormateDateTime1(_leaveApp.frtm) + "&lt;/From Time&gt;";  //From Time
            bdtx += "&lt;To Time&gt;" + UtilDatetime.FormateDateTime1(_leaveApp.totm) + "&lt;/To Time&gt;";  //To Time
            bdtx += "&lt;Summary&gt;" + "D:" + _leaveApp.days.ToString() + " / H:" + _leaveApp.hurs.ToString() + "&lt;/TSummary&gt;";  //Summary
            bdtx += "&lt;Remark&gt;" + _leaveApp.remk + "&lt;/Remark&gt;";  //Remark
            bdtx += "<br>&lt;/LeaveApplication&gt;";   //</LeaveApplication>

            return bdtx;
        }

        public void UpdateLeaveApplication(tlvleaapp _leaveApp)
        {
            try
            {
                //分析请假并保存
                lvanalevBll bll = new lvanalevBll();

                List<ColumnInfo> dateParameters = new List<ColumnInfo>();
                dateParameters.Add(new ColumnInfo() { ColumnName = "lvstart", ColumnValue = UtilDatetime.FormateDateTime1(_leaveApp.frtm) });
                dateParameters.Add(new ColumnInfo() { ColumnName = "lvend", ColumnValue = UtilDatetime.FormateDateTime1(_leaveApp.totm), ColumnType = "datetime" });

                List<ColumnInfo> personParameters = new List<ColumnInfo>();
                personParameters.Add(new ColumnInfo() { ColumnName = "emp.emno", ColumnValue = _leaveApp.emno });


                using (TransactionScope scope = new TransactionScope())
                {

                    //先删除leave detail
                    List<ColumnInfo> apnoParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "apno", ColumnValue = _leaveApp.apno } };
                    dal.DeleteLeaveAppDtl(apnoParameters);

                    //如果审核过，则分析休假，并生成leave detail
                    if (_leaveApp.lvst == "Approved")
                        bll.AnalyzeLeave(dateParameters, personParameters, _leaveApp, true);

                    DoUpdate<tlvleaapp>(_leaveApp, apnoParameters);

                    scope.Complete();
                }
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

        public void DeleteLeaveApplication(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //先删除leave detail
                    dal.DeleteLeaveAppDtl(_parameters);

                    DoDelete<tlvleaapp>(_parameters);

                    scope.Complete();
                }
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

        public string CalcLeaveTime(tlvleaapp leaveApp)
        {
            try
            {
                //只分析请假
                lvanalevBll bll = new lvanalevBll();

                List<ColumnInfo> dateParameters = new List<ColumnInfo>();
                dateParameters.Add(new ColumnInfo() { ColumnName = "lvstart", ColumnValue = UtilDatetime.FormateDateTime1(leaveApp.frtm) });
                dateParameters.Add(new ColumnInfo() { ColumnName = "lvend", ColumnValue = UtilDatetime.FormateDateTime1(leaveApp.totm), ColumnType = "datetime" });

                List<ColumnInfo> personParameters = new List<ColumnInfo>();
                personParameters.Add(new ColumnInfo() { ColumnName = "emp.emno", ColumnValue = leaveApp.emno });

                bll.AnalyzeLeave(dateParameters, personParameters, null, false);

                return "totallvhours:'" + bll.TotalLvHours.ToString() + "',totallvdays:'" + bll.TotalLvDays.ToString() + "'";
            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
        }

        public LvSettingInfo GetEmpLeaveSettings(string _emno, string _ltcd, DateTime _leavedate)
        {
            try
            {
                lvanalevBll bll = new lvanalevBll();
                return bll.GetEmpLeaveSettings(_emno, _ltcd, _leavedate);
            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
        }

    }
}
