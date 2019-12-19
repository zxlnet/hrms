using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Personal;
using GotWell.Common;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.HRMS.HRMSData.Overtime;
using GotWell.HRMS.HRMSBusiness.Overtime;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{

    public class atanaattBll : BaseBll
    {
        atanaattDal localDal = null;
        private DateTime AnalStartDate;
        private DateTime AnalEndDate;
        private DateTime startDateByPerson;
        private DateTime endDateByPerson;
        string sSqlStaff = string.Empty;

        List<vw_employment> lstStaff = null;
        List<tatpricld> lstPriCalendar = null;
        List<tatclddtl> lstCalendarDetails = null;
        List<tatrosdtl> lstRosterDetails = null;
        List<tatroshi> lstRosterHistory = null;
        List<tatanarst> lstAnalResult = null;
        List<tatoridat> lstOriAtData = null;
        List<tatdatmnu> lstManuAtData = null;
        List<tlvleaapd> lstLeaveApp = null;
        List<tatshflat> lstShiftLate = null;
        List<tatshfely> lstShiftEarly = null;
        List<tottype> lstOTType = null;

        AtCalculationInfo store = null;
        //DateTime curDayR;

        public atanaattBll()
        {
            localDal = new atanaattDal();
            baseDal = localDal;
        }



        #region 取数据
        public void GetPersonals(List<ColumnInfo> _personalParameters,ref List<vw_employment> _lstStaff,ref string _sqlStaff,DateTime _analStartDate)
        {
            psemplymDal dal = new psemplymDal();
            _lstStaff = dal.GetHiringEmployment(_personalParameters, _analStartDate);
            _sqlStaff = dal.AnalyzeQueryCriterias(_personalParameters, _analStartDate);
            if (_sqlStaff.Trim() == "")
                _sqlStaff = "(1=1)";
        }

        public void GetAnalyzeDateRange(List<ColumnInfo> _atdtParameters,ref DateTime _analStartDate,ref DateTime _analEndDate)
        {
            //start date
            _analStartDate = Convert.ToDateTime(_atdtParameters[0].ColumnValue);

            //end date
            if (_atdtParameters[0].ColumnValue.Trim() == string.Empty)
                _analEndDate = Convert.ToDateTime("2050-01-01");
            else
                _analEndDate = Convert.ToDateTime(_atdtParameters[1].ColumnValue).AddDays(1);
        }

        public List<tatpricld> GetPrivateCalendar(List<ColumnInfo> _personalParameters, string _sqlStaff,DateTime _analStartDate, DateTime _analEndDate)
        {
            atpvtcldDal dal = new atpvtcldDal();
            return dal.GetPrivateCalendar(_personalParameters, _sqlStaff, _analStartDate, _analEndDate);
        }

        public List<tatclddtl> GetCalendar(DateTime _analStartDate, DateTime _analEndDate)
        {
            atcaldarDal dal = new atcaldarDal();
            return dal.GetCalendars(_analStartDate, _analEndDate);
        }

        public List<tatanarst> GetAnalResult(List<ColumnInfo> _personalParameters, string _sqlStaff, DateTime _analStartDate, DateTime _analEndDate)
        {
            atanarstDal dal = new atanarstDal();
            return dal.GetAnalResult(_personalParameters, _sqlStaff, _analStartDate, _analEndDate);
        }

        public List<tatrosdtl> GetRosterDetails(DateTime _analStartDate, DateTime _analEndDate)
        {
            atrosterDal dal = new atrosterDal();
            return dal.GetRosterDetails(_analStartDate, _analEndDate);
        }

        public List<tatroshi> GetRosterHistory(string _sqlStaff, DateTime _analStartDate, DateTime _analEndDate)
        {
            atroshisDal dal = new atroshisDal();
            return dal.GetRosterHistory(_sqlStaff, _analStartDate, _analEndDate);
        }

        public List<tatoridat> GetOriginalData(string _sqlStaff, DateTime _analStartDate, DateTime _analEndDate)
        {
            atoridatDal dal = new atoridatDal();
            return dal.GetOriginalData(_sqlStaff, _analStartDate, _analEndDate.AddDays(1));
        }

        public List<tatdatmnu> GetManualData(string _sqlStaff, DateTime _analStartDate, DateTime _analEndDate)
        {
            atdatmnuDal dal = new atdatmnuDal();
            return dal.GetManualData(_sqlStaff, _analStartDate, _analEndDate.AddDays(1));
        }

        public List<tlvleaapd> GetLeaveApp(string _sqlStaff, DateTime _analStartDate, DateTime _analEndDate)
        {
            lvleaappDal dal = new lvleaappDal();
            return dal.GetLeaveApps(_sqlStaff, _analStartDate, _analEndDate.AddDays(1));
        }

        public List<tatshfely> GetShiftEarly()
        {
            BaseDal dal = new BaseDal();
            return dal.GetSelectedRecords<tatshfely>(new List<ColumnInfo>() { });
        }

        public List<tatshflat> GetShiftLate()
        {
            BaseDal dal = new BaseDal();
            return dal.GetSelectedRecords<tatshflat>(new List<ColumnInfo>() { });
        }

        public List<tottype> GetOTType()
        {
            BaseDal dal = new BaseDal();
            return dal.GetSelectedRecords<tottype>(new List<ColumnInfo>() { });
        }

        #endregion

        public void DeleteDummyData()
        {
            //delete dummy manual attendance data
            //atdatmnuDal dal1 = new atdatmnuDal();
            //dal1.DeleteDummyData(sSqlStaff, AnalStartDate, AnalEndDate);

            //delete dummy absence details
            atabsdtlDal dal2 = new atabsdtlDal();
            dal2.DeleteDummyData(sSqlStaff, AnalStartDate, AnalEndDate);

            //delete dummy attendance data
            atanarstDal dal3 = new atanarstDal();
            dal3.DeleteDummyData(sSqlStaff, AnalStartDate, AnalEndDate);

            //delete dummy atotdetails data
            otdetailDal dal4 = new otdetailDal();
            dal4.DeleteDummyData(sSqlStaff, AnalStartDate, AnalEndDate);

        }

        public void AnalyzeAttendance(List<ColumnInfo> _atdtParameters,
                            List<ColumnInfo> _personalParameters,bool includelv,bool includeot)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    GetAnalyzeDateRange(_atdtParameters, ref AnalStartDate, ref AnalEndDate);
                    GetPersonals(_personalParameters, ref lstStaff, ref sSqlStaff, AnalStartDate);

                    lstPriCalendar = GetPrivateCalendar(_personalParameters, sSqlStaff, AnalStartDate, AnalEndDate);
                    lstCalendarDetails = GetCalendar(AnalStartDate, AnalEndDate);
                    lstRosterDetails = GetRosterDetails(AnalStartDate, AnalEndDate);
                    lstRosterHistory = GetRosterHistory(sSqlStaff, AnalStartDate, AnalEndDate);
                    lstAnalResult = GetAnalResult(_personalParameters, sSqlStaff, AnalStartDate, AnalEndDate);
                    lstOriAtData = GetOriginalData(sSqlStaff, AnalStartDate, AnalEndDate);
                    lstManuAtData = GetManualData(sSqlStaff, AnalStartDate, AnalEndDate);
                    lstLeaveApp = GetLeaveApp(sSqlStaff, AnalStartDate, AnalEndDate);
                    lstShiftEarly = GetShiftEarly();
                    lstShiftLate = GetShiftLate();
                    lstOTType = GetOTType();

                    DeleteDummyData();

                    DoAnalyze(includelv, includeot);

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

        public void GetStandardValue(tatrosdtl curRosterDtl, tatshift curShift,ref AtCalculationInfo _store)
        {
            #region 取得标准班次设定时间
            _store.StdEarylyTimeIn = Convert.ToDateTime(UtilDatetime.FormatDate1(_store.CurDay) + " " + curShift.eati);

            //分析是否班次存在跨天的现象，eati > timeid
            _store.StdTimeIn = CompareTime(_store.CurDay,curShift.eati, curShift.tmin);

            //判断是否定义了中间休息，如果定义了则计算中间休息记录
            if (curShift.bken == "Y")
            {
                #region 有中途休息

                #region 判断小休一
                if ((curShift.rttm1 > 0) && (curShift.r1st.Trim() != "") && (curShift.r1st.Trim() != "")
                    && (curShift.r1ed.Trim() != "") && (curShift.r1ed.Trim() != ""))
                {
                    //如果有小休一
                    _store.StdRest1Start = CompareTime(_store.CurDay, curShift.tmin, curShift.r1st);
                    _store.StdRest1End = CompareTime(_store.StdRest1Start.Value, curShift.r1st, curShift.r1ed);
                    _store.StdBreakStart = CompareTime(_store.StdRest1End.Value, curShift.r1ed, curShift.btst);
                    _store.StdBreakEnd = CompareTime(_store.StdBreakStart.Value, curShift.btst, curShift.bked);
                }
                else
                {
                    //如果没有小休一
                    _store.StdRest1Start = null;
                    _store.StdRest1End = null;
                    _store.StdBreakStart = CompareTime(_store.CurDay, curShift.tmin, curShift.btst);
                    _store.StdBreakEnd = CompareTime(_store.StdBreakStart.Value, curShift.btst, curShift.bked);
                }
                #endregion

                #region 判断小休二
                if ((curShift.rttm2 != 0) && (curShift.r2st.Trim() != "") && (curShift.r2ed.Trim() != ""))
                {
                    //如果有小休一
                    _store.StdRest2Start = CompareTime(_store.StdBreakEnd.Value, curShift.bked, curShift.r2st);
                    _store.StdRest2End = CompareTime(_store.StdRest2Start.Value, curShift.r2st, curShift.r2ed);
                    _store.StdTimeOut = CompareTime(_store.StdRest2End.Value, curShift.r2ed, curShift.tmot);
                }
                else
                {
                    //如果没有小休一
                    _store.StdRest2Start = null;
                    _store.StdRest2End = null;
                    _store.StdTimeOut = CompareTime(_store.StdBreakEnd.Value, curShift.bked, curShift.tmot);
                }
                #endregion

                #endregion

            }
            else
            {
                #region 无中途休息
                _store.StdBreakStart = null;
                _store.StdBreakEnd = null;

                #region 判断小休一
                if ((curShift.rttm1 != 0) && (curShift.r1st.Trim() != "") && (curShift.r1ed.Trim() != ""))
                {
                    #region 如果有小休一
                    _store.StdRest1Start = CompareTime(_store.CurDay, curShift.tmin, curShift.r1st);
                    _store.StdRest1End = CompareTime(_store.StdRest1Start.Value, curShift.r1st, curShift.r1ed);

                    if ((curShift.rttm2 != 0) && (curShift.r2st.Trim() != "") && (curShift.r2ed.Trim() != ""))
                    {
                        //如果有小休二
                        _store.StdRest2Start = CompareTime(_store.StdRest1End.Value, curShift.r1ed, curShift.r2st);
                        _store.StdRest2End = CompareTime(_store.StdRest2Start.Value, curShift.r2st, curShift.r2ed);
                        _store.StdTimeOut = CompareTime(_store.StdRest2End.Value, curShift.r2ed, curShift.tmot);
                    }
                    else
                    {
                        //如果没有小休二
                        _store.StdRest2Start = null;
                        _store.StdRest2End = null;
                        _store.StdTimeOut = CompareTime(_store.StdRest1End.Value, curShift.r1ed, curShift.tmot);
                    }
                    #endregion
                }
                else
                {
                    #region 如果没有小休一
                    _store.StdRest1Start = null;
                    _store.StdRest1End = null;
                    if ((curShift.rttm2 != 0) && (curShift.r2st.Trim() != "") && (curShift.r2ed.Trim() != ""))
                    {
                        //如果有小休二
                        _store.StdRest2Start = CompareTime(_store.StdTimeIn.Value, curShift.tmin, curShift.r2st);
                        _store.StdRest2End = CompareTime(_store.StdRest2Start.Value, curShift.r2st, curShift.r2ed);
                        _store.StdTimeOut = CompareTime(_store.StdRest2End.Value, curShift.r2ed, curShift.tmot);
                    }
                    else
                    {
                        //如果没有小休二
                        _store.StdRest2Start = null;
                        _store.StdRest2End = null;
                        _store.StdTimeOut = CompareTime(_store.StdTimeIn.Value, curShift.tmin, curShift.tmot);
                    }
                    #endregion
                }
                #endregion
                #endregion

            }

            _store.StdLateTimeOut = CompareTime(store.StdTimeOut.Value, curShift.tmot, curShift.lato);

            #endregion
        }

        public void CheckIsRestDay(tatrosdtl curRosterDtl, tatshift curShift, tatroshi rosterHis, List<tatpricld> _lstPriCalendar, ref AtCalculationInfo _store)
        {
            #region 判断是否是休息日
            DateTime tmpDate = _store.CurDay;
            string emno = _store.EmpInfo.emno;
            List<tatpricld> lstEmpPriCalendar = _lstPriCalendar.Where(p => p.emno == emno).ToList();

            //判断是否存在个人日历
            List<tatpricld> lstEmpPriCalendarcddt = lstEmpPriCalendar.Where(p => p.cddt == tmpDate).ToList();
            if (lstEmpPriCalendarcddt.Count > 0)
            {
                //存在个人日历
                _store.PrivateOTType = lstEmpPriCalendarcddt.Single().otcd;
                if (lstEmpPriCalendarcddt.Single().htcd == "-1") //-1: 工作日，hardcode
                {
                    _store.IsRestDay = false;
                    _store.IsPrivateRest = false;
                }
                else
                {
                    _store.IsRestDay = true;
                    _store.IsPrivateRest = true;
                }

            }
            else
            {
                //不存在个人日历
                //判断是否日历或轮班休息
                _store.RosterOTType = curRosterDtl.otcd;
                if (curRosterDtl.isrt == "Y")
                {
                    _store.IsRestDay = true;
                    _store.IsRosterRest = true;
                }
                else
                {
                    _store.IsRestDay = false;
                    _store.IsRosterRest = false;
                }

                if (rosterHis.tatroster.rtty == "Both")
                {
                    //Both表示轮班或日历任何休就休
                    tatclddtl clddtl = null;
                    string clcd = _store.EmpInfo.clcd;
                    List<tatclddtl> tmpCld = new List<tatclddtl>();

                    if (lstCalendarDetails != null)
                        tmpCld = lstCalendarDetails.Where(p => p.cddt == tmpDate && p.clcd == clcd).ToList();

                    if (tmpCld.Count > 0)
                    {
                        clddtl = tmpCld.Single();
                    }

                    if (clddtl != null)
                    {
                        _store.CalendarOTType = clddtl.otcd;
                        if (clddtl.htcd == "-1") //-1: 工作日，hardcode
                        {
                            _store.IsCalendarRest = false;
                        }
                        else
                        {
                            _store.IsRestDay = true;
                            _store.IsCalendarRest = true;
                        }
                    }
                }
            }

            //如果员工当天离职，则不计算缺勤，认为是休假日
            if ((_store.EmpInfo.tmdt.HasValue != false) && (_store.EmpInfo.tmdt.Value.CompareTo(_store.CurDay) > 0))
                _store.IsRestDay = true;

            #endregion
        }

        private void AnalyzeLeave(tatshift curShift)
        {
            #region 分析请假记录
            if ((store.IsAbsent)) //(store.ActTimeIn == -1) || (store.ActTimeOut == -1) || (ActBreakStart == -2) || (ActBreakEnd == -2) ||
            {
                List<tlvleaapd> lstEmpLeaveApp = lstLeaveApp.Where(p => p.emno == store.EmpInfo.emno &&
                        p.frtm.CompareTo(store.StdTimeOut) < 0 && p.totm.CompareTo(store.StdTimeIn) > 0).ToList();

                for (int j = 0; j < lstEmpLeaveApp.Count; j++)
                {
                    tlvleaapd leaveApp = lstEmpLeaveApp[j];
                    store.LvStart = leaveApp.frtm;
                    store.LvEnd = leaveApp.totm;
                    store.LvHours = leaveApp.hurs;
                    store.LvDays = leaveApp.days;

                    store.remark += " " + leaveApp.tlvleatyp.ltnm + "(" + Math.Round(store.LvHours, 1) + ")";
               
                    //冲销缺勤天数
                    store.AbsentHours = store.AbsentHours - store.LvHours;
                    store.AbsentHoursReal = store.AbsentHoursReal - store.LvHours;

                    //冲销迟到,早退时间
                    if (leaveApp.frtm >= store.ActTimeOut)
                    {
                        //冲销下班早退
                        store.ActEarlyOutHours = store.ActEarlyTimeOut.TotalHours  - store.LvHours;
                    }

                    if (leaveApp.frtm < store.ActTimeIn)
                    {
                        //冲销上班迟到
                        store.ActLateTimeHours = store.ActLateTimeIn.TotalHours - store.LvHours;
                    }

                    if (leaveApp.frtm < store.ActBreakStart && leaveApp.frtm > store.ActTimeIn) 
                    {
                        //冲销午休早退
                        store.ActEarlyBreakHours = store.ActEarlyBreakEndReal.TotalHours - store.LvHours;
                    }

                    if (leaveApp.frtm > store.ActBreakEnd && leaveApp.frtm < store.ActTimeOut) 
                    {
                        //冲销午休迟到
                        store.ActLateBreakHours = store.ActLateBreakStartReal.TotalHours - store.LvHours;
                    }

                }
            }
            #endregion
        }

        private void CalculateLateCount(tatshift curShift)
        {
            #region 分析迟到记录
            if (((store.ActLateTimeHours > 0) || (store.ActLateBreakHours > 0)) &&
                  ((store.ActLateTimeHours + store.ActLateBreakHours) > curShift.altm))
            {
                List<tatshflat> lstShiftLateByCode = lstShiftLate.Where(p => p.sfcd == curShift.sfcd).ToList();
                //如果没有定义区间法，则按非区间法计算

                if (curShift.lcty == "W" && lstShiftLateByCode.Count > 0)
                {
                    #region 区间法
                    #region 上班迟到
                    if (Math.Round(store.ActLateTimeHours * 60, 5) > curShift.altm)
                    {
                        if (lstShiftLateByCode.Count < 1)
                        {
                            //没有设定区间法，则默认值值为1
                            //isAbsentOrLate = false;
                            store.LateCountIn = 1;
                        }
                        else
                        {
                            if (Math.Round(store.ActLateTimeHours * 60) >= lstShiftLateByCode.Last().letm)
                            {
                                //超过最大设定，则取最大设定值
                                //isAbsentOrLate = false;
                                store.LateCountIn = lstShiftLateByCode.Last().lact;
                            }
                            else
                            {
                                for (int n = 0; n < lstShiftLateByCode.Count; n++)
                                {
                                    if (Math.Round(store.ActLateTimeHours * 60, 5) <= lstShiftLateByCode[n].letm)
                                    {
                                        store.LateCountIn = lstShiftLateByCode[n].lact;
                                        n = lstShiftLateByCode.Count;
                                    }
                                    else
                                    {
                                        store.LateCountIn = lstShiftLateByCode[n].lact;
                                    }
                                }
                            }
                        }
                        //记录迟到时间
                        store.LateHoursIn = Math.Round(store.ActLateTimeHours, 2);

                    }
                    #endregion

                    #region 中间休息迟到
                    if ((store.ActLateBreakHours > 0)) // && (Math.Round(store.ActLateBreakStartReal.TotalMinutes, 5) > curShift.altm))// && (isAbsentOrLate == true))
                    {
                        if (lstShiftLateByCode.Count < 1)
                        {
                            //没有设定区间法，则默认值值为1
                            //isAbsentOrLate = false;
                            store.LateCountBreak = 1;
                        }
                        else
                        {
                            if (Math.Round(store.ActLateBreakHours * 60, 5) > lstShiftLateByCode.Last().letm)
                            {
                                //超过最大设定，则取最大设定值
                                //isAbsentOrLate = false;
                                store.LateCountBreak = lstShiftLateByCode.Last().lact;
                            }
                            else
                            {
                                for (int n = 0; n < lstShiftLateByCode.Count; n++)
                                {
                                    if (Math.Round(store.ActLateBreakHours * 60, 5) <= lstShiftLateByCode[n].letm)
                                    {
                                        store.LateCountBreak = lstShiftLateByCode[n].lact;
                                        n = lstShiftLateByCode.Count;
                                    }
                                    else
                                    {
                                        store.LateCountBreak = lstShiftLateByCode[n].lact;
                                    }
                                }
                            }
                        }
                        //记录迟到时间
                        //store.LateCountBreak = store.LateCountIn + store.LateCountBreak;
                        store.LateHoursBreak = Math.Round(store.ActLateBreakHours, 2);
                    }
                    #endregion

                    #endregion
                }
                else
                {
                    #region 非区间法
                    #region 上班迟到
                    if (Math.Round(store.ActLateTimeInReal.TotalMinutes, 5) > curShift.altm)
                    {
                        store.LateCountIn = 1;
                        store.LateHoursIn = Math.Round(store.ActLateTimeInReal.TotalHours, 2);
                    }
                    #endregion

                    #region 中间休息迟到
                    if ((store.ActLateBreakStartReal.TotalSeconds > 0)) //&& (Math.Round(store.ActLateBreakStartReal.TotalMinutes, 5) > curShift.altm))// && (isAbsentOrLate == true))
                    {
                        store.LateCountBreak = 1;
                        store.LateHoursBreak = Math.Round(store.ActLateBreakStartReal.TotalHours, 2);
                    }
                    #endregion
                    #endregion
                }

                store.LateCountTotal = store.LateCountIn + store.LateCountBreak;
                store.LateHoursTotal = store.LateHoursIn + store.LateHoursBreak;

            }
            #endregion
        }

        private void CalculateEarlyTimeOut(tatshift curShift)
        {
            #region 分析早退记录
            //if ((store.ActEarlyTimeOut.TotalSeconds > 0) || (store.ActEarlyBreakEndReal.TotalSeconds > 0))//&& isAbsentOrLate)
            if (((store.ActEarlyBreakHours > 0) || (store.ActEarlyOutHours > 0)) &&
                ((store.ActEarlyBreakHours + store.ActEarlyOutHours) > curShift.aotm))
            {
                List<tatshfely> lstShiftEarlyByCode = lstShiftEarly.Where(p => p.sfcd == curShift.sfcd).ToList();
                //如果没有定义区间法，则按非区间法计算

                if (curShift.ecty == "W" && lstShiftEarlyByCode.Count > 0)
                {
                    #region 区间法
                    #region 下班早退
                    if (store.ActEarlyOutHours > 0)
                    {
                        if (lstShiftEarly.Count < 1)
                        {
                            //没有设定区间法，则默认值值为1
                            //isAbsentOrLate = false;
                            store.EarlyCountOut = 1;
                        }
                        else
                        {
                            if (Math.Round(store.ActEarlyOutHours * 60) > lstShiftEarlyByCode.Last().eotm)
                            {
                                //超过最大设定，则取最大设定值
                                //isAbsentOrLate = false;
                                store.EarlyCountOut = lstShiftEarlyByCode.Last().eact;
                            }
                            else
                            {
                                for (int n = 0; n < lstShiftEarlyByCode.Count; n++)
                                {
                                    if (Math.Round(store.ActEarlyOutHours * 60) <= lstShiftEarlyByCode[n].eotm)
                                    {
                                        store.EarlyCountOut = lstShiftEarlyByCode[n].eact;
                                        n = lstShiftEarlyByCode.Count;
                                    }
                                    else
                                    {
                                        store.EarlyCountOut = lstShiftEarlyByCode[n].eact;
                                    }
                                }
                            }
                        }
                        //记录早退时间
                        store.EarlyHoursOut = Math.Round(store.ActEarlyOutHours, 2);
                    }
                    #endregion

                    #region 中间休息早退
                    // && (Math.Round(store.ActEarlyBreakHours*60) > curShift.aotm)
                    if (store.ActEarlyBreakHours > 0)
                    {
                        if (lstShiftEarlyByCode.Count < 1)
                        {
                            //没有设定区间法，则默认值值为1
                            //isAbsentOrLate = false;
                            store.EarlyCountBreak = 1;
                        }
                        else
                        {
                            if (Math.Round(store.ActEarlyBreakHours * 60) > lstShiftEarlyByCode.Last().eotm)
                            {
                                //超过最大设定，则取最大设定值
                                //isAbsentOrLate = false;
                                store.EarlyCountBreak = lstShiftEarlyByCode.Last().eact;
                            }
                            else
                            {
                                for (int n = 0; n < lstShiftEarlyByCode.Count; n++)
                                {
                                    if (Math.Round(store.ActEarlyBreakHours * 60) <= lstShiftEarlyByCode[n].eotm)
                                    {
                                        store.EarlyCountBreak = lstShiftEarlyByCode[n].eact;
                                        n = lstShiftEarlyByCode.Count;
                                    }
                                    else
                                    {
                                        store.EarlyCountBreak = lstShiftEarlyByCode[n].eact;
                                    }
                                }
                            }
                        }
                        store.EarlyHoursBreak = Math.Round(store.ActEarlyBreakHours, 2);
                    }
                    #endregion

                    #endregion
                }
                else
                {
                    #region 非区间法
                    #region 下班早退
                    if (store.ActEarlyTimeOutReal.TotalSeconds > 0)
                    {
                        store.EarlyCountOut = 1;
                        store.EarlyHoursOut = Math.Round(store.ActEarlyTimeOutReal.TotalHours, 2);
                    }
                    #endregion

                    #region 中间休息早退
                    if ((store.ActEarlyBreakEndReal.TotalMinutes > 0))// && (Math.Round(store.ActEarlyBreakEndReal.TotalMinutes) > curShift.aotm))// && (isAbsentOrLate == true))
                    {
                        store.EarlyCountBreak = 1;
                        store.EarlyHoursBreak = Math.Round(store.ActEarlyBreakEndReal.TotalHours, 2);
                    }
                    #endregion

                    #endregion
                }
                store.EarlyCountTotal = store.EarlyCountOut + store.EarlyCountBreak;
                store.EarlyHoursTotal = store.EarlyHoursOut + store.EarlyHoursBreak;

            }
            #endregion
        }

        public void DoAnalyze(bool includeLv,bool includeOT)
        {
            for (int i = 0; i < lstStaff.Count; i++)
            {
                try
                {
                    //Person by person to analyze
                    startDateByPerson = AnalStartDate;
                    endDateByPerson = AnalEndDate;

                    List<tatroshi> lstEmpRosterHistory = lstRosterHistory.Where(p => p.emno == lstStaff[i].emno).ToList();
                    bool isExit = false;

                    for (int j = 0; j < lstEmpRosterHistory.Count; j++)
                    {
                        //理论上只会出现一个有效的roster
                        try
                        {
                            AnalyzeOneRosterHistory(lstStaff[i].emno, lstEmpRosterHistory[j], ref isExit,includeLv,includeOT);
                        }
                        catch (Exception ex)
                        {
                            string x = ex.Message;
                        }

                        if (isExit)
                        {
                            //退出
                            i = lstEmpRosterHistory.Count;
                            continue;
                        }

                    }
                }
                catch (Exception ex)
                {
                    string x = ex.Message;
                    throw ex;
                }
            }
        }

        public void AnalyzeOneRosterHistory(string _emno, tatroshi _rosterHis, ref bool isExit, bool includeLv, bool includeOT)
        {
            List<tatrosdtl> lstEmpRosterDtl = lstRosterDetails;
            List<tatrosdtl> lstEmpRosterDtlByCode = lstEmpRosterDtl.Where(p => p.rscd == _rosterHis.rscd).ToList();
            DateTime rosterEndDate;

            if (lstEmpRosterDtlByCode.Count < 1) return;

            #region 判断实效、失效日期
            if (_rosterHis.efdt.Value.CompareTo(startDateByPerson) > 0)
            {
                //如果轮班历史的有效日期大于当前计算日期，则从轮班历史生效日期开始计算
                startDateByPerson = _rosterHis.efdt.Value;
            }

            if (_rosterHis.tatroster.efdt.Value.CompareTo(startDateByPerson) > 0)
            {
                //如果轮班的有效日期大于当前计算日期，则从轮班生效日期开始计算
                startDateByPerson = _rosterHis.tatroster.efdt.Value;
            }

            if (startDateByPerson.CompareTo(endDateByPerson) > 0)
            {
                isExit = true;
                return;
            }
            #endregion

            //判断需要从哪个班次开始计算
            int shiftIndex = (int)((TimeSpan)(startDateByPerson - _rosterHis.tatroster.efdt)).TotalDays % lstEmpRosterDtlByCode.Count;

            #region 确定在该轮班历史中运算几天
            if ((_rosterHis.exdt.HasValue == false) || (_rosterHis.exdt.Value > endDateByPerson))
            {
                rosterEndDate = endDateByPerson;
            }
            else
            {
                rosterEndDate = _rosterHis.exdt.Value.AddDays(-1);
            }

            if ((_rosterHis.tatroster.exdt.HasValue == false) || (_rosterHis.tatroster.exdt.Value > rosterEndDate))
            {
                rosterEndDate = rosterEndDate;
            }
            else
            {
                rosterEndDate = _rosterHis.tatroster.exdt.Value.AddDays(-1);
            }
            #endregion

            //在一个轮班的生效期内循环
            for (int x = 0; x < (rosterEndDate - startDateByPerson).TotalDays; x++)
            {
                startDateByPerson = startDateByPerson.AddHours(x);
                tatrosdtl curRosterDtl = lstEmpRosterDtlByCode[shiftIndex + x];
                tatshift curShift = curRosterDtl.tatshift;
                try
                {
                    AnalyzeOneDay(_emno, curRosterDtl, curShift, _rosterHis,includeLv,includeOT);
                }
                catch (Exception ex)
                {
                    string er = ex.Message;
                    throw ex;
                }

                startDateByPerson = startDateByPerson.AddDays(1);
            }
        }

        private void AnalyzeOneDay(string _emno, tatrosdtl curRosterDtl, tatshift curShift, tatroshi _rosterHis,bool includeLv,bool includeOT)
        {
            store = new AtCalculationInfo();
            store.CurDay = startDateByPerson;
            store.EmpInfo = lstStaff.Where(p => p.emno == _emno).Single();

            List<tatanarst> lstEmpAR = lstAnalResult.Where(p => p.emno == store.EmpInfo.emno && p.atdt == store.CurDay && p.iscf=="Y").ToList();

            if (lstEmpAR.Count <= 0)
            {
                #region 该员工没有确认过的记录
                //该员工没有确认过的记录
                if ((store.EmpInfo.tmdt.HasValue == false) || (store.EmpInfo.tmdt.Value.CompareTo(store.CurDay) > 0))
                {
                    //如果该员工没有离职，则分析
                    if ((curRosterDtl.sfcd != null) && (curRosterDtl.sfcd.Trim() != string.Empty))
                    {
                        //curDayR = store.CurDay;

                        //取得标准班次设定时间
                        GetStandardValue(curRosterDtl, curShift,ref store);

                        //判断是否是休息日
                        CheckIsRestDay(curRosterDtl, curShift, _rosterHis,lstPriCalendar,ref store);

                        if (store.IsRestDay == false)
                        {
                            #region 不是休息日
                            List<tatanarst> tmpEmpAnalResult = lstAnalResult.Where(p => p.emno == store.EmpInfo.emno && p.atdt == store.CurDay).ToList();
                            tatanarst empAnalResult = null;

                            if (tmpEmpAnalResult.Count > 0)
                                empAnalResult = tmpEmpAnalResult.Single();

                            //过滤出原始出勤记录
                            List<tatoridat> lstEmpOriData = lstOriAtData.Where(p => p.sfid == store.EmpInfo.sfid
                                                                                   && p.retm.CompareTo(store.StdEarylyTimeIn) >= 0
                                                                                   && p.retm.CompareTo(store.StdLateTimeOut) <= 0)
                                                                        .OrderBy(p=>p.retm).ToList();

                            #region 判断是否只考虑出入记录
                            //判断是否只考虑出入记录
                            if (curShift.ifio == "Y")
                            {
                                #region 只考虑出入
                                for (int n = 0; n < lstEmpOriData.Count; n++)
                                {
                                    if (lstEmpOriData[n].retm.CompareTo(store.StdTimeIn) <= 0)
                                    {
                                        //小于tmin的记录，第一次打卡记录
                                        store.ActTimeIn = lstEmpOriData[n].retm;
                                        store.ActTimeInCalc = store.ActTimeIn;
                                    }
                                    else
                                    {
                                        if (lstEmpOriData[n].retm.CompareTo(store.StdTimeOut) >= 0)
                                        {
                                            //大于tmot的记录，最后一次打卡记录
                                            store.ActTimeOut = lstEmpOriData[n].retm;
                                            store.ActTimeOutCalc = store.ActTimeOut;
                                        }
                                        else
                                        {
                                            if (store.ActTimeIn == null)
                                            {
                                                //第一次，记入第一次上班打卡
                                                store.ActTimeIn = lstEmpOriData[n].retm;
                                                store.ActTimeInCalc = store.ActTimeIn;
                                            }
                                            else
                                            {
                                                //否则，记入下班打卡
                                                store.ActTimeOut = lstEmpOriData[n].retm;
                                                store.ActTimeOutCalc = store.ActTimeOut;
                                            }
                                        }
                                    }
                                }


                                if (empAnalResult != null)
                                {
                                    //有确认记录
                                    if (empAnalResult.ifma == "N")
                                    {
                                        #region 无手工记录
                                        if ((empAnalResult.intm.HasValue == false))
                                        {
                                            store.ActTimeIn = null;
                                            store.ActTimeOut = null;
                                        }
                                        else
                                        {
                                            store.ActTimeIn = empAnalResult.intm;
                                            store.ActTimeInCalc = empAnalResult.intm;

                                            store.ActTimeOut = empAnalResult.ottm;
                                            store.ActTimeOutCalc = empAnalResult.ottm;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 有手工记录
                                        //In
                                        if (empAnalResult.itmm.HasValue)
                                        {
                                            store.ActTimeIn = empAnalResult.itmm.Value;
                                            store.ActTimeInCalc = empAnalResult.itmm.Value;
                                        }
                                        //Out
                                        if (empAnalResult.otmm.HasValue)
                                        {
                                            store.ActTimeOut = empAnalResult.otmm.Value;
                                            store.ActTimeOutCalc = empAnalResult.otmm.Value;
                                        }

                                        //BreakStart
                                        if (empAnalResult.bstm.HasValue)
                                        {
                                            store.ActBreakStart = empAnalResult.bstm.Value;
                                        }

                                        //BreakEnd
                                        if (empAnalResult.betm.HasValue)
                                        {
                                            store.ActBreakEnd = empAnalResult.betm.Value;
                                        }
                                        #endregion
                                    }

                                    #region 上班
                                    if (store.ActTimeIn != null)
                                    {
                                        if (store.ActTimeIn < store.StdTimeIn)
                                        {
                                            store.ActTimeInCalc = store.StdTimeIn;
                                        }

                                        #region 有小休一
                                        if ((curShift.rttm1 != 0) && (curShift.r1st.Trim() != "") && (curShift.r1ed.Trim() != ""))
                                        {
                                            if ((store.StdRest1Start <= store.ActTimeIn) && (store.ActTimeIn <= store.StdRest1End))
                                            {
                                                //如果ActTimeIn正好处于Rest1 Start/End之内
                                                //如果小休属于工作时数，则缺勤时要考虑
                                                if (curShift.igrl == "N")
                                                    //不属于工作时
                                                    store.ActTimeInCalc = store.StdBreakStart;
                                                else
                                                    //属于工作时
                                                    store.ActTimeInCalc = store.ActTimeInCalc;
                                            }

                                            if (store.StdRest1End < store.ActTimeIn)
                                            {
                                                //如果ActTimeIn在rest1结束之后

                                                if (curShift.igrl == "N")
                                                {
                                                    //不属于工作时，则扣除小休时间
                                                    TimeSpan diff = Convert.ToDateTime(curShift.r1ed) - Convert.ToDateTime(curShift.r1st);
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * (diff.TotalDays)); 
                                                }
                                                else
                                                {
                                                    //属于工作时
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 有中间休息
                                        if (curShift.bken == "Y")
                                        {
                                            if ((store.StdBreakStart <= store.ActTimeIn) && (store.ActTimeIn <= store.StdBreakEnd))
                                            {
                                                //在午休时间内
                                                //如果午休属于工作时数，则缺勤时要考虑
                                                if (curShift.igbl == "Y")
                                                {
                                                    //属于工作时
                                                    store.ActTimeInCalc = store.ActTimeInCalc;
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan? diff = store.ActTimeIn - store.StdBreakStart;
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.Value.TotalDays);//DateAdd("n", -(Round(((Tin) - (Sbtst)) * 24 * 60, 0)), Tin1)
                                                }
                                            }

                                            if (store.StdBreakEnd < store.ActTimeIn)
                                            {
                                                //在午休时间之后
                                                if (curShift.igbl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan? diff = store.StdBreakEnd - store.StdBreakStart;
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.Value.TotalDays);//DateAdd("n", -(Round(((Sbked) - (Sbtst)) * 24 * 60, 0)), Tin1)
                                                }

                                            }
                                        }
                                        #endregion

                                        #region 有小休二
                                        if ((curShift.rttm2 != 0) && (curShift.r2st.Trim() != "") && (curShift.r2ed.Trim() != ""))
                                        {
                                            if ((store.StdRest2Start <= store.ActTimeIn) && (store.ActTimeIn <= store.StdRest2End))
                                            {
                                                //小休内
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan? diff = store.ActTimeIn - Convert.ToDateTime(curShift.r1st) ;
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.Value.TotalDays);//DateAdd("n", -(Round(((Tin) - (r2st)) * 24 * 60, 0)), Tin1)
                                                }
                                            }

                                            if (store.StdRest2End < store.ActTimeIn)
                                            {
                                                //小休后
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan? diff = Convert.ToDateTime(curShift.r2ed) - Convert.ToDateTime(curShift.r2st);
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.Value.TotalDays);//DateAdd("n", -(Round(((r2ed) - (r2st)) * 24 * 60, 0)), Tin1)
                                                }
                                            }
                                        }
                                        #endregion


                                    }
                                    #endregion

                                    #region 下班
                                    if (store.ActTimeOut == null)
                                    {
                                    }
                                    else
                                    {
                                        if (store.StdTimeOut.Value.CompareTo(store.ActTimeOut) < 0)
                                        {
                                            //如果标准下班时间小于实际下班时间，则取实际下班时间
                                            store.ActTimeOutCalc = store.StdTimeOut;
                                        }

                                        #region 有小休二
                                        if ((curShift.rttm2 != 0) && (curShift.r2st.Trim() != "") && (curShift.r2ed.Trim() != ""))
                                        {
                                            if ((store.StdRest2Start <= store.ActTimeOut) && (store.ActTimeOut <= store.StdRest2End))
                                            {
                                                //小休内
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    store.ActTimeOutCalc = store.StdRest2End;
                                                }
                                            }

                                            if (store.ActTimeOut < store.StdRest2Start)
                                            {
                                                //小休前
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = Convert.ToDateTime(curShift.r2ed) - Convert.ToDateTime(curShift.r2st);
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((r2ed) - (r2st)) * 24 * 60, 0)), Tout1)
                                                }
                                            }


                                        }
                                        #endregion

                                        #region 有中间休息
                                        if (curShift.bken == "Y")
                                        {
                                            if ((store.StdBreakStart <= store.ActTimeOut) && (store.ActTimeOut <= store.StdBreakEnd))
                                            {
                                                //Break内
                                                if (curShift.igbl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = store.StdBreakEnd.Value - store.ActTimeOut.Value;
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((Sbked) - (Tout)) * 24 * 60, 0)), Tout1)
                                                }
                                            }

                                            if (store.ActTimeOut < store.StdBreakStart)
                                            {
                                                //Break之前
                                                if (curShift.igbl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = store.StdBreakEnd.Value - store.StdBreakStart.Value;
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((Sbked) - (Sbtst)) * 24 * 60, 0)), Tout1)
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 有小休一
                                        if ((curShift.rttm1 != 0) && (curShift.r1st.Trim() != "") && (curShift.r1ed.Trim() != ""))
                                        {
                                            if ((store.StdRest1Start <= store.ActTimeOut) && (store.ActTimeOut <= store.StdRest1End))
                                            {
                                                //小休内
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = Convert.ToDateTime(curShift.r1ed) - Convert.ToDateTime(store.ActTimeOut);
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((r1ed) - (Tout)) * 24 * 60, 0)), Tout1)
                                                }
                                            }

                                            if (store.ActTimeOut < store.StdRest1Start)
                                            {
                                                //小休前
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = Convert.ToDateTime(curShift.r1ed) - Convert.ToDateTime(curShift.r1st);
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((r1ed) - (r1st)) * 24 * 60, 0)), Tout1)
                                                }
                                            }

                                        }
                                        #endregion

                                    }
                                    #endregion
                                }

                                if ((store.ActTimeInCalc != null) && (store.ActTimeOutCalc != null))
                                {
                                    #region 计算上班迟到时间
                                    if (store.ActTimeInCalc == null)
                                    {
                                        store.ActLateTimeIn = store.StdTimeOut.Value - store.StdTimeIn.Value;
                                    }
                                    else
                                    {
                                        store.ActLateTimeIn = store.ActTimeInCalc.Value - store.StdTimeIn.Value;
                                    }
                                    store.ActLateTimeInReal = store.ActLateTimeIn;

                                    if (store.ActLateTimeIn.TotalSeconds < 0)
                                    {
                                        store.ActLateTimeIn = new TimeSpan(0, 0, 0);
                                        store.ActLateTimeInReal = store.ActLateTimeIn;
                                    }

                                    if (Math.Round(store.ActLateTimeIn.TotalDays, 5) > Math.Round(Convert.ToDouble(curShift.aotm / 1440), 5))
                                    {
                                        store.IsLate = true;

                                        store.ActLateTimeHours = store.ActLateTimeInReal.TotalHours;
                                    }
                                    else
                                    {
                                        store.ActLateTimeIn = new TimeSpan(0, 0, 0);
                                        store.IsLate = false;
                                    }
                                    #endregion

                                    #region 算下班早退时间
                                    if (store.ActTimeOutCalc == null)
                                    {
                                        //如果同时没有上班打卡，则将缺勤记在迟到上
                                        if (store.ActTimeInCalc!=null)
                                            store.ActEarlyTimeOut = store.StdTimeOut.Value - store.StdTimeIn.Value;
                                    }
                                    else
                                    {
                                        store.ActEarlyTimeOut = store.StdTimeOut.Value - store.ActTimeOutCalc.Value;
                                    }
                                    store.ActEarlyTimeOutReal = store.ActEarlyTimeOut;

                                    if (store.ActEarlyTimeOut.TotalSeconds < 0)
                                    {
                                        store.ActEarlyTimeOut = new TimeSpan(0, 0, 0);
                                        store.ActEarlyTimeOutReal = new TimeSpan(0, 0, 0);
                                    }

                                    if (Math.Round(store.ActEarlyTimeOut.TotalSeconds, 5) > Math.Round(Convert.ToDouble(curShift.aotm) / 1440, 5))
                                    {
                                        store.IsEarly = true;

                                        store.ActEarlyOutHours = store.ActEarlyTimeOutReal.TotalHours; 
                                    }
                                    else
                                    {
                                        store.ActEarlyTimeOut = new TimeSpan(0, 0, 0);
                                        store.IsEarly = false;
                                    }

                                    #endregion
                                }
                                else
                                {
                                    store.IsAbsent = true;
                                }

                                #region 计算缺勤时间
                                if ((store.IsEarly) || (store.IsLate))
                                    store.IsAbsent = true;

                                if ((store.ActTimeIn == null) || (store.ActTimeOut == null))
                                {
                                    //如果没有发现任何考勤记录，则认为本日缺勤
                                    store.AbsentHours = Convert.ToDouble(curShift.wkhr);
                                    store.AbsentHoursReal = store.AbsentHours;
                                }
                                else
                                {
                                    store.AbsentHours = (store.ActLateTimeIn + store.ActEarlyTimeOut).TotalHours;
                                    store.AbsentHoursReal = (store.ActLateTimeInReal + store.ActEarlyTimeOutReal).TotalHours;
                                }
                                #endregion

                                #endregion

                            }
                            else
                            {
                                #region 考虑全部记录

                                #region 如果无中途休息
                                if (curShift.bken == "N")
                                {
                                    for (int n = 0; n < lstEmpOriData.Count; n++)
                                    {
                                        if (lstEmpOriData[n].retm.CompareTo(store.StdTimeIn) <= 0) //<=
                                        {
                                            store.ActTimeIn = lstEmpOriData[n].retm;
                                        }
                                        else
                                        {
                                            if (lstEmpOriData[n].retm.CompareTo(store.StdTimeOut) >= 0) //>=
                                            {
                                                store.ActTimeOut = lstEmpOriData[n].retm;
                                            }
                                            else
                                            {
                                                if (store.ActTimeIn == null)
                                                {
                                                    //第一次，记入第一次上班打卡
                                                    store.ActTimeIn = lstEmpOriData[n].retm;
                                                }
                                                else
                                                {
                                                    //否则，记入下班打卡
                                                    store.ActTimeOut = lstEmpOriData[n].retm;
                                                }
                                            }
                                        }
                                    }
                                    if (store.ActTimeIn < store.StdTimeIn)
                                        store.ActTimeInCalc = store.StdTimeIn;
                                    else
                                        store.ActTimeInCalc = store.ActTimeIn;

                                    if (store.ActTimeOut>store.StdTimeOut)
                                        store.ActTimeOutCalc = store.StdTimeOut;
                                    else
                                        store.ActTimeOutCalc = store.ActTimeOut;
                                }
                                #endregion

                                #region 如果有中途休息
                                //如果有中途休息
                                if (curShift.bken == "Y")
                                {
                                    #region 处理btst打卡
                                    //处理btst打卡
                                    for (int n = 0; n < lstEmpOriData.Count; n++)
                                    {
                                        if (lstEmpOriData[n].retm.CompareTo(store.StdTimeIn) <= 0) //<=
                                        {
                                            store.ActTimeIn = lstEmpOriData[n].retm;
                                        }
                                        else
                                        {
                                            if (lstEmpOriData[n].retm.CompareTo(store.StdBreakStart) <= 0) //>=
                                            {
                                                //小于休息开始时间
                                                if (store.ActTimeIn == null)
                                                {
                                                    store.ActTimeIn = lstEmpOriData[n].retm;
                                                }
                                                else
                                                {
                                                    if (curShift.bser == "Y")
                                                        //休息开始打卡时间
                                                        store.ActBreakEnd = lstEmpOriData[n].retm;
                                                }
                                            }
                                            else if (lstEmpOriData[n].retm.CompareTo(store.StdBreakEnd) <= 0)
                                            {
                                                if (store.ActTimeIn == null)
                                                    //第一次，记入第一次上班打卡
                                                    store.ActTimeIn = lstEmpOriData[n].retm;
                                                else
                                                    //否则，记入午休开始打卡
                                                    store.ActBreakEnd = lstEmpOriData[n].retm;
                                            }
                                        }
                                    }
                                    if (store.ActTimeIn < store.StdTimeIn)
                                        store.ActTimeInCalc = store.StdTimeIn;
                                    else
                                        store.ActTimeInCalc = store.ActTimeIn;

                                    if (store.ActBreakEnd > store.StdBreakStart)
                                        store.ActBreakEnd1 = store.StdBreakStart;
                                    else
                                        store.ActBreakEnd1 = store.ActBreakEnd;
                                    #endregion

                                    #region 处理bked打卡
                                    //处理bked打卡
                                    for (int n = 0; n < lstEmpOriData.Count; n++)
                                    {
                                        if (lstEmpOriData[n].retm.CompareTo(store.StdBreakEnd) <= 0) //<=
                                        {
                                            if (store.ActBreakEnd != null && store.ActBreakEnd != lstEmpOriData[n].retm)
                                            {
                                                if (curShift.bker == "Y")
                                                    //界于休息开始和休息结束之间，并要求打卡时
                                                    //作为休息后上班打卡时间
                                                    store.ActBreakStart = lstEmpOriData[n].retm;
                                            }
                                        }
                                        else
                                        {
                                            if (lstEmpOriData[n].retm.CompareTo(store.StdTimeOut) >= 0) //>=
                                            {
                                                //下班打卡时间
                                                store.ActTimeOut = lstEmpOriData[n].retm;
                                            }
                                            else
                                            {
                                                //if ((store.ActTimeIn == null) && (curShift.bker == "Y"))
                                                //    //第一次，记入第一次上班打卡
                                                //    store.ActBreakStart = lstEmpOriData[n].retm;
                                                //else
                                                //    //否则，记入下班打卡
                                                //    store.ActTimeOut = lstEmpOriData[n].retm;

                                                if (store.ActBreakStart == null)
                                                {
                                                    store.ActBreakStart = lstEmpOriData[n].retm;
                                                }
                                                else
                                                {
                                                    store.ActTimeOut = lstEmpOriData[n].retm;
                                                }
                                            }
                                        }
                                    }
                                    if (store.ActTimeOut > store.StdTimeOut)
                                        store.ActTimeOutCalc = store.StdTimeOut;
                                    else
                                        store.ActTimeOutCalc = store.ActTimeOut;

                                    if (store.ActBreakStart1 <= store.StdBreakEnd)
                                        store.ActBreakStart1 = store.StdBreakEnd;
                                    else
                                        store.ActBreakStart1 = store.ActBreakStart;
                                    #endregion

                                }

                                #endregion

                                if (empAnalResult != null)
                                {
                                    #region 上班
                                    if (empAnalResult.ifma == "Y")
                                    {
                                        #region 有手工记录
                                        if ((empAnalResult.itmm.HasValue == false))
                                        {
                                            store.ActTimeIn = null;
                                            store.ActTimeInCalc = null;
                                        }
                                        else
                                        {
                                            store.ActTimeIn = empAnalResult.itmm;
                                            store.ActTimeInCalc = empAnalResult.itmm;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 无手工记录
                                        List<tatanarst> lstEmpConfirmedAnalResult = lstAnalResult.Where(p => p.emno == store.EmpInfo.emno && p.intm < store.StdTimeOut && p.intm >= store.StdEarylyTimeIn).ToList();
                                        if (lstEmpConfirmedAnalResult.Count < 1)
                                        {
                                            store.ActTimeIn = null;
                                        }
                                        else
                                        {
                                            if (lstEmpConfirmedAnalResult[0].intm.HasValue)
                                            {
                                                store.ActTimeIn = lstEmpConfirmedAnalResult[0].intm.Value;
                                                store.ActTimeInCalc = lstEmpConfirmedAnalResult[0].intm.Value;
                                            }
                                            else
                                            {
                                                store.ActTimeIn = null;
                                            }
                                        }
                                        #endregion
                                    }

                                    if (store.ActTimeIn != null)
                                    {
                                        if (store.ActTimeIn < store.StdTimeIn)
                                        {
                                            store.ActTimeInCalc = store.StdTimeIn;
                                        }

                                        #region 有小休一
                                        if ((curShift.rttm1 != 0) && (curShift.r1st.Trim() != "") && (curShift.r1ed.Trim() != ""))
                                        {

                                            if ((store.StdRest1Start <= store.ActTimeIn) && (store.ActTimeIn <= store.StdRest1End))
                                            {
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    store.ActTimeInCalc = store.StdRest1Start;
                                                }
                                            }

                                            if (store.StdRest1End < store.ActTimeIn)
                                            {
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = Convert.ToDateTime(curShift.r1ed) - Convert.ToDateTime(curShift.r1st);
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.TotalDays);//DateAdd("n", -(Round(((r1ed) - (r1st)) * 24 * 60, 0)), Tin1)
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 有中间休息
                                        if (curShift.bken == "Y")
                                        {
                                            if ((store.StdBreakStart <= store.ActTimeIn) && (store.ActTimeIn <= store.StdBreakEnd))
                                            {
                                                if (curShift.igbl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = store.ActTimeIn.Value - store.StdBreakStart.Value;
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.TotalDays);//DateAdd("n", -(Round(((Tin) - (Sbtst)) * 24 * 60, 0)), Tin1)
                                                }
                                            }

                                            if (store.StdBreakEnd < store.ActTimeIn)
                                            {
                                                if (curShift.igbl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = store.StdBreakEnd.Value - store.StdBreakStart.Value;
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.TotalDays);//DateAdd("n", -(Round(((Sbked) - (Sbtst)) * 24 * 60, 0)), Tin1)
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 有小休二
                                        if ((curShift.rttm2 != 0) && (curShift.r2st.Trim() != "") && (curShift.r2ed.Trim() != ""))
                                        {
                                            if ((store.StdRest2Start <= store.ActTimeIn) && (store.ActTimeIn <= store.StdRest2End))
                                            {
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = store.ActTimeIn.Value - Convert.ToDateTime(curShift.r2st);
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.TotalDays);//DateAdd("n", -(Round(((Tin) - (r2st)) * 24 * 60, 0)), Tin1)
                                                }
                                            }

                                            if (store.StdRest2End < store.ActTimeIn)
                                            {
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = Convert.ToDateTime(curShift.r2ed) - Convert.ToDateTime(curShift.r2st);
                                                    store.ActTimeInCalc = store.ActTimeInCalc.Value.AddDays(-1 * diff.TotalDays);//DateAdd("n", -(Round(((r2ed) - (r2st)) * 24 * 60, 0)), Tin1)
                                                }
                                            }
                                        }
                                        #endregion

                                    }
                                    #endregion

                                    #region 中间休息下班
                                    if ((curShift.bken == "Y") && (curShift.bser == "Y"))
                                    {
                                        if (empAnalResult.ifma == "Y")
                                        {
                                            #region 有手工记录
                                            if ((empAnalResult.bstm.HasValue == false))
                                            {
                                                store.ActBreakEnd = null;
                                                store.ActBreakEnd1 = null;
                                            }
                                            else
                                            {
                                                store.ActBreakEnd = empAnalResult.bstm;
                                                store.ActBreakEnd1 = empAnalResult.bstm;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            #region 无手工记录
                                            if ((empAnalResult.brst.HasValue == false))
                                            {
                                                store.ActBreakEnd = null;
                                            }
                                            else
                                            {
                                                store.ActBreakEnd = empAnalResult.brst;
                                                store.ActBreakEnd1 = empAnalResult.brst;
                                            }
                                            #endregion
                                        }

                                        if (true) //(store.ActBreakEnd != -2) && (store.ActBreakEnd != -1)
                                        {
                                            if (store.StdBreakStart < store.ActBreakEnd)
                                                store.ActBreakEnd1 = store.StdBreakStart;

                                            #region 有小休一
                                            if ((curShift.rttm1 != 0) && (curShift.r1st.Trim() != "") && (curShift.r1ed.Trim() != ""))
                                            {
                                                if ((store.StdRest1Start <= store.ActBreakEnd) && (store.ActBreakEnd <= store.StdRest1End))
                                                {
                                                    if (curShift.igrl == "Y")
                                                    {
                                                        //属于工作时
                                                    }
                                                    else
                                                    {
                                                        //不属于工作时
                                                        store.ActBreakEnd1 = store.StdRest1End;
                                                    }
                                                }

                                                if (store.ActBreakEnd < store.StdRest1Start)
                                                {
                                                    if (curShift.igrl == "Y")
                                                    {
                                                        //属于工作时
                                                    }
                                                    else
                                                    {
                                                        //不属于工作时
                                                        TimeSpan diff = Convert.ToDateTime(curShift.r1ed) - Convert.ToDateTime(curShift.r1st);
                                                        store.ActBreakEnd1 = store.ActBreakEnd.Value.AddDays(-1 * diff.TotalDays);//DateAdd("n", (Round(((r1ed) - (r1st)) * 24 * 60, 0)), TBout1)
                                                    }
                                                }
                                            }
                                            #endregion

                                        }
                                        else
                                        {
                                            //store.ActBreakEnd = -2;
                                        }

                                    }
                                    #endregion

                                    #region 中间休息上班
                                    if ((curShift.bken == "Y") && (curShift.bker == "Y"))
                                    {
                                        if (empAnalResult.ifma == "Y")
                                        {
                                            #region 有手工记录
                                            if ((empAnalResult.betm.HasValue == false))
                                            {
                                                store.ActBreakStart = null;
                                                store.ActBreakStart1 = null;
                                            }
                                            else
                                            {
                                                store.ActBreakStart = empAnalResult.betm;
                                                store.ActBreakStart1 = empAnalResult.betm;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            #region 无手工记录
                                            if ((empAnalResult.bret.HasValue == false))
                                            {
                                                store.ActBreakStart = null;
                                            }
                                            else
                                            {
                                                store.ActBreakStart = empAnalResult.bret;
                                                store.ActBreakStart1 = empAnalResult.bret;
                                            }
                                            #endregion
                                        }

                                        if (true) //(store.ActBreakStart != -2) && (store.ActBreakStart != -1)
                                        {
                                            if (store.StdBreakEnd < store.ActBreakStart)
                                                store.ActBreakStart1 = store.StdBreakEnd;

                                            #region 有小休二
                                            if ((curShift.rttm2 != 0) && (curShift.r2st.Trim() != "") && (curShift.r2ed.Trim() != ""))
                                            {
                                                if ((store.StdRest2Start <= store.ActBreakStart) && (store.ActBreakStart <= store.StdRest2End))
                                                {
                                                    if (curShift.igrl == "Y")
                                                    {
                                                        //属于工作时
                                                    }
                                                    else
                                                    {
                                                        //不属于工作时
                                                        store.ActBreakStart1 = store.StdRest2Start;
                                                    }
                                                }

                                                if (store.StdRest2End < store.ActBreakStart)
                                                {
                                                    if (curShift.igrl == "Y")
                                                    {
                                                        //属于工作时
                                                    }
                                                    else
                                                    {
                                                        //不属于工作时
                                                        TimeSpan diff = Convert.ToDateTime(curShift.r2ed) - Convert.ToDateTime(curShift.r2st);
                                                        store.ActBreakStart1 = store.ActBreakStart1.Value.AddDays(-1 * diff.TotalDays);//DateAdd("n", -(Round(((r2ed) - (r2st)) * 24 * 60, 0)), TBin1)
                                                    }
                                                }
                                            }
                                            #endregion

                                        }
                                        else
                                        {
                                            //store.ActBreakStart = -2;
                                        }

                                    }
                                    #endregion

                                    #region 下班
                                    if (empAnalResult.ifma == "Y")
                                    {
                                        #region 有手工记录
                                        if ((empAnalResult.ottm.HasValue == false))
                                        {
                                            store.ActTimeOut = null;
                                            store.ActTimeOutCalc = null;
                                        }
                                        else
                                        {
                                            store.ActTimeOut = empAnalResult.ottm;
                                            store.ActTimeOutCalc = empAnalResult.ottm;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 无手工记录
                                        List<tatanarst> lstEmpConfirmedAnalResult = lstAnalResult.Where(p => p.emno == store.EmpInfo.emno && p.ottm < store.StdLateTimeOut && p.ottm >= store.StdTimeIn).ToList();
                                        if (lstEmpConfirmedAnalResult.Count < 1)
                                        {
                                            store.ActTimeOut = null;
                                        }
                                        else
                                        {
                                            if (lstEmpConfirmedAnalResult[0].ottm.HasValue)
                                            {
                                                store.ActTimeOut = lstEmpConfirmedAnalResult[0].ottm.Value;
                                                store.ActTimeOutCalc = lstEmpConfirmedAnalResult[0].ottm.Value;
                                            }
                                            else
                                            {
                                                store.ActTimeOut = null;
                                            }
                                        }
                                        #endregion
                                    }

                                    if (store.ActTimeOut != null)
                                    {
                                        if (store.ActTimeOut > store.StdTimeOut)
                                        {
                                            store.ActTimeOutCalc = store.StdTimeOut;
                                        }

                                        #region 有小休二
                                        if ((curShift.rttm2 != 0) && (curShift.r2st.Trim() != "") && (curShift.r2ed.Trim() != ""))
                                        {
                                            if ((store.StdRest2Start <= store.ActTimeOut) && (store.ActTimeOut <= store.StdRest2End))
                                            {
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    store.ActTimeOutCalc = store.StdRest2End;
                                                }
                                            }

                                            if (store.ActTimeOut < store.StdRest2Start)
                                            {
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = Convert.ToDateTime(curShift.r2ed) - Convert.ToDateTime(curShift.r2st);
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((r2ed) - (r2st)) * 24 * 60, 0)), Tout1)
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 有中间休息
                                        if (curShift.bken == "Y")
                                        {
                                            if ((store.StdBreakStart.Value <= store.ActTimeOut.Value) && (store.ActTimeOut.Value <= store.StdBreakEnd.Value))
                                            {
                                                if (curShift.igbl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = store.ActTimeOut.Value - store.StdBreakEnd.Value;
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((Sbked) - (Tout)) * 24 * 60, 0)), Tout1)
                                                }
                                            }

                                            if (store.ActTimeOut < store.StdBreakStart)
                                            {
                                                if (curShift.igbl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = store.StdBreakEnd.Value - store.StdBreakStart.Value;
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((Sbked) - (Sbtst)) * 24 * 60, 0)), Tout1)
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 有小休一
                                        if ((curShift.rttm1 != 0) && (curShift.r1st.Trim() != "") && (curShift.r1ed.Trim() != ""))
                                        {
                                            if ((store.StdRest1Start.Value <= store.ActTimeOut.Value) && (store.ActTimeOut.Value <= store.StdRest1End.Value))
                                            {
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = store.ActTimeOut.Value - Convert.ToDateTime(curShift.r2ed);
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((r1ed) - (Tout)) * 24 * 60, 0)), Tout1)
                                                }
                                            }

                                            if (store.ActTimeOut.Value < store.StdRest1Start.Value)
                                            {
                                                if (curShift.igrl == "Y")
                                                {
                                                    //属于工作时
                                                }
                                                else
                                                {
                                                    //不属于工作时
                                                    TimeSpan diff = Convert.ToDateTime(curShift.r1ed) - Convert.ToDateTime(curShift.r1st);
                                                    store.ActTimeOutCalc = store.ActTimeOutCalc.Value.AddDays(1 * diff.TotalDays);//DateAdd("n", (Round(((r1ed) - (r1st)) * 24 * 60, 0)), Tout1)
                                                }
                                            }
                                        }
                                        #endregion

                                    }

                                    #endregion

                                    #region 分析结束，汇总数据
                                    //if (curShift.bken == "N")
                                    //{
                                    //    #region 没有午休
                                    //    if (true) //(store.ActTimeIn == -1) || (store.ActTimeOut == -1)
                                    //    {
                                    //        store.AbsentHours = Convert.ToDouble(curShift.wkhr / 24);
                                    //        store.AbsentHoursReal = store.AbsentHours;
                                    //    }
                                    //    else
                                    //    {
                                    //        store.AbsentHours = (store.ActLateTimeIn + store.ActEarlyTimeOut).TotalHours;
                                    //        store.AbsentHoursReal = (store.ActLateTimeInReal + store.ActEarlyTimeOutReal).TotalHours;
                                    //    }
                                    //    #endregion
                                    //}
                                    //else
                                    //{
                                    //    #region 有午休

                                    //    if (true) //(store.ActTimeIn == -1) || (store.ActBreakEnd == -2)
                                    //    {
                                    //        store.AbsentHours = (store.StdBreakStart.Value - store.StdTimeIn.Value).TotalHours - curShift.rttm1.Value / 1440;
                                    //        store.AbsentHoursReal = store.AbsentHours;
                                    //    }
                                    //    else
                                    //    {
                                    //        store.AbsentHours = (store.ActLateTimeIn + store.ActEarlyTimeOut).TotalHours;
                                    //        //store.AbsentHoursReal = store.ActLateTimeInReal + store.ActEarlyBreakOutReal;
                                    //    }

                                    //    if (true)//(store.ActBreakStart == -2) || (ActTimeOut == -1)
                                    //    {
                                    //        store.AbsentHours = store.AbsentHours - (store.StdTimeOut.Value - store.StdBreakEnd.Value).TotalHours - curShift.rttm2.Value / 1440;
                                    //        store.AbsentHoursReal = store.AbsentHours;
                                    //    }
                                    //    else
                                    //    {
                                    //        //store.AbsentHours = store.AbsentHours + store.Latebtst + store.eatmOut;
                                    //        //store.AbsentHoursReal = store.AbsentTimeReal + store.LatebtstReal + store.eatmOutReal;
                                    //    }
                                    //    #endregion
                                    //}

                                    #endregion
                                }
                                else
                                {
                                    #region 计算缺勤时间
                                    //store.AbsentHours = Convert.ToDouble(curShift.wkhr);
                                    //store.AbsentHoursReal = Convert.ToDouble(curShift.wkhr);
                                    store.IsAbsent = true;
                                    #endregion
                                }

                                #region 计算上班迟到时间
                                if (store.ActTimeInCalc == null)
                                {
                                    if (curShift.bken == "Y")
                                    {
                                        store.ActLateTimeIn = store.StdBreakStart.Value - store.StdTimeIn.Value;
                                    }
                                    else
                                    {
                                        store.ActLateTimeIn = store.StdTimeOut.Value - store.StdTimeIn.Value;
                                    }
                                }
                                else
                                {
                                    store.ActLateTimeIn = store.ActTimeInCalc.Value - store.StdTimeIn.Value;
                                }

                                store.ActLateTimeInReal = store.ActLateTimeIn;

                                if (store.ActLateTimeIn.TotalSeconds < 0)
                                {
                                    store.ActLateTimeIn = new TimeSpan(0, 0, 0);
                                    store.ActLateTimeInReal = store.ActLateTimeIn;
                                }

                                if (Math.Round(store.ActLateTimeIn.TotalDays, 5) > Math.Round(Convert.ToDouble(curShift.altm / 1440), 5))
                                {
                                    store.IsLate = true;

                                    store.ActLateTimeHours = store.ActLateTimeIn.TotalHours;
                                }
                                else
                                {
                                    store.ActLateTimeIn = new TimeSpan(0, 0, 0);
                                }
                                #endregion

                                #region 算午休下班早退时间
                                if ((curShift.bken == "Y") && (curShift.bser == "Y"))
                                {
                                    //如果BreakStart没有打卡，则早退
                                    if (store.ActBreakEnd1 != null)
                                        store.ActEarlyBreakEnd = store.StdBreakStart.Value - store.ActBreakEnd1.Value;
                                    else
                                    {
                                        if (store.ActTimeIn == null || store.ActTimeIn <= store.StdTimeIn)
                                        {
                                            store.ActEarlyBreakEnd = store.StdBreakStart.Value - store.StdTimeIn.Value;
                                            //不计迟到时间，改成记录Ｂｒｅａｋ早退时间
                                            store.ActLateTimeIn = new TimeSpan(0, 0, 0);
                                            store.ActLateTimeInReal = store.ActLateTimeIn;
                                        }
                                        else
                                        {
                                            store.ActEarlyBreakEnd = store.StdBreakStart.Value - store.ActTimeInCalc.Value;
                                        }
                                    }

                                    store.ActEarlyBreakEndReal = store.ActEarlyBreakEnd;

                                    if (store.ActEarlyBreakEnd.TotalSeconds < 0)
                                    {
                                        store.ActEarlyBreakEnd = new TimeSpan(0, 0, 0);
                                        store.ActEarlyBreakEndReal = new TimeSpan(0, 0, 0);
                                    }

                                    if (Math.Round(store.ActEarlyBreakEnd.TotalDays, 5) > Math.Round(Convert.ToDouble(curShift.aotm / 1440), 5))
                                    {
                                        store.IsEarly = true;
                                        store.ActEarlyOutHours += store.ActEarlyBreakEndReal.TotalHours;
                                    }
                                    else
                                    {
                                        store.ActEarlyBreakEnd = new TimeSpan(0, 0, 0);
                                    }
                                    
                                }
                                #endregion

                                #region 算午休上班迟到时间
                                if (store.ActTimeOutCalc == null)
                                {
                                    if ((curShift.bken == "Y"))
                                    {
                                        if (curShift.igbl == "Y")
                                            //午休算做工作时
                                            store.ActEarlyTimeOut = store.StdTimeOut.Value - store.StdBreakStart.Value;
                                        else
                                            //午休不算做工作时
                                            store.ActEarlyTimeOut = store.StdTimeOut.Value - store.StdBreakEnd.Value;
                                    }
                                    else
                                    {
                                        //如果同时没有上班打卡，则将缺勤记在迟到上
                                        if (store.ActTimeInCalc != null)
                                            store.ActEarlyTimeOut = store.StdTimeOut.Value - store.StdTimeIn.Value;
                                    }
                                }
                                else
                                {
                                    store.ActEarlyTimeOut = store.StdTimeOut.Value - store.ActTimeOutCalc.Value;
                                }

                                store.ActEarlyTimeOutReal = store.ActEarlyTimeOut;

                                if ((curShift.bken == "Y") && (curShift.bker == "Y"))
                                {
                                    //如果BreakEnd没有打卡，则迟到
                                    if (store.ActBreakStart1 != null)
                                    {
                                        store.ActLateBreakStart = store.ActBreakStart1.Value - store.StdBreakEnd.Value;
                                    }
                                    else
                                    {
                                        if (store.ActTimeIn == null || store.ActTimeIn <= store.StdTimeIn)
                                        {
                                            store.ActLateBreakStart = store.StdTimeOut.Value - store.StdBreakEnd.Value;

                                            store.ActEarlyTimeOut = new TimeSpan(0, 0, 0);
                                            store.ActEarlyTimeOutReal = store.ActEarlyTimeOut;
                                        }
                                        else
                                        {
                                            store.ActLateBreakStart = store.ActTimeOutCalc.Value - store.StdBreakEnd.Value;
                                        }
                                    }

                                    store.ActLateBreakStartReal = store.ActLateBreakStart;

                                    if (store.ActLateBreakStart.TotalSeconds < 0)
                                    {
                                        store.ActLateBreakStart = new TimeSpan(0, 0, 0);
                                    }

                                    if (Math.Round(store.ActLateBreakStart.TotalDays, 5) > Math.Round(Convert.ToDouble(curShift.altm / 1440), 5))
                                    {
                                        store.IsLate = true;
                                        store.ActLateTimeHours += store.ActLateBreakStartReal.TotalHours;
                                    }
                                    else
                                    {
                                        store.ActLateBreakStart = new TimeSpan(0, 0, 0);
                                    }
                                    
                                }
                                #endregion

                                #region 计算下班早退时间

                                if (store.ActEarlyTimeOut.TotalSeconds < 0)
                                {
                                    store.ActEarlyTimeOut = new TimeSpan(0, 0, 0);
                                    store.ActEarlyTimeOutReal = store.ActEarlyTimeOut;
                                }

                                if (Math.Round(store.ActEarlyTimeOut.TotalDays, 5) > Math.Round(Convert.ToDouble(curShift.aotm / 1440), 5))
                                {
                                    //如果同时没有午休上班打卡，则将缺勤记在迟到上

                                    if (curShift.bken == "Y" && curShift.bker == "Y")
                                    {
                                        if (store.ActBreakStart1 != null)
                                            store.ActEarlyOutHours = store.ActEarlyTimeOut.TotalHours;
                                    }
                                    else
                                    {
                                        store.ActEarlyOutHours = store.ActEarlyTimeOut.TotalHours;
                                    }

                                    store.IsEarly = true;
                                }
                                else
                                {
                                    store.ActEarlyTimeOut = new TimeSpan(0, 0, 0);
                                }
                                #endregion

                                #region 分析结束，汇总数据
                                //if (curShift.bken == "N")
                                //{
                                //    #region 没有午休
                                //    if ((store.ActTimeIn == null) || (store.ActTimeOut == null))
                                //    {
                                //        store.AbsentHours = Convert.ToDouble(curShift.wkhr / 24);
                                //        store.AbsentHoursReal = store.AbsentHours;
                                //    }
                                //    else
                                //    {
                                //        store.AbsentHours = (store.ActLateTimeIn + store.ActEarlyTimeOut).TotalHours;
                                //        store.AbsentHoursReal = (store.ActLateTimeInReal + store.ActEarlyTimeOutReal).TotalHours;
                                //    }
                                //    #endregion
                                //}
                                //else
                                //{
                                //    #region 有午休

                                //    //if (true) //(store.ActTimeIn == -1) || (store.ActBreakEnd == -2)
                                //    if ((store.ActTimeIn == null))
                                //    {
                                //        //如果没有上班打卡时间或者午休结束上班打卡时间
                                //        //则认为半天迟到
                                //        store.AbsentHours = (store.StdBreakStart.Value - store.StdTimeIn.Value).TotalHours;

                                //        if (curShift.igrl == "N")
                                //        {
                                //            //如果小休不属于工作时数,则迟到时间需要减去小休时间
                                //            store.AbsentHours -= curShift.rttm1.Value / 1440;
                                //        }
                                //        store.AbsentHoursReal = store.AbsentHours;
                                //    }
                                //    else
                                //    {
                                //        store.AbsentHours = (store.ActLateTimeIn + store.ActEarlyTimeOut).TotalHours;
                                //        store.AbsentHoursReal = store.ActLateTimeInReal + store.ActEarlyBreakOutReal;
                                //    }

                                //    if ((store.ActBreakEnd == null))
                                //    {
                                //        //如果午休结束上班打卡时间
                                //        //则认为半天迟到
                                //    }

                                //    //if (true)//(store.ActBreakStart == -2) || (ActTimeOut == -1)
                                //    if ((store.ActBreakStart == null) || (store.ActTimeOut == null))
                                //    {
                                //        store.AbsentHours = store.AbsentHours - (store.StdTimeOut.Value - store.StdBreakEnd.Value).TotalHours - curShift.rttm2.Value / 1440;
                                //        store.AbsentHoursReal = store.AbsentHours;
                                //    }
                                //    else
                                //    {
                                //        store.AbsentHours = store.AbsentHours + store.Latebtst + store.eatmOut;
                                //        store.AbsentHoursReal = store.AbsentTimeReal + store.LatebtstReal + store.eatmOutReal;
                                //    }
                                //    #endregion
                                //}
                                if ((store.IsEarly) || (store.IsLate))
                                    store.IsAbsent = true;

                                if ((store.ActTimeIn == null) || (store.ActTimeOut == null))
                                {
                                    //如果没有发现任何考勤记录，则认为本日缺勤
                                    store.AbsentHours = Convert.ToDouble(curShift.wkhr);
                                    store.AbsentHoursReal = store.AbsentHours;
                                }
                                else
                                {
                                    store.AbsentHours = (store.ActLateTimeIn + store.ActEarlyTimeOut + store.ActEarlyBreakEnd + store.ActLateBreakStart).TotalHours;
                                    store.AbsentHoursReal = (store.ActLateTimeInReal + store.ActEarlyTimeOutReal + +store.ActEarlyBreakEndReal + store.ActLateBreakStartReal).TotalHours;
                                }

                                #endregion

                                #endregion
                            }
                            #endregion

                            //分析请假记录
                            if (includeLv)
                            {
                                AnalyzeLeave(curShift);
                            }

                            #region 忽略过小的缺勤情况
                            //调整了一下计算缺勤时间精度，认为计算的缺勤时间>=0.5分钟为缺勤
                            if (store.AbsentHours  * 60 < 0.5)
                                store.AbsentHours = 0;

                            //if (store.AbsentHoursReal * 60 < 0.5)
                            //    store.AbsentHoursReal = 0;
                            #endregion

                            #region 未缺勤记录实际（迟到/早退）缺勤时间
                            if (Math.Round(store.AbsentHours * 60, 5) > 0)
                            {
                                #region 有缺勤情况
                                //bool isAbsentOrLate = true;
                                //store.LateHoursIn = 0;
                                //store.EarlyHoursOut = 0;

                                //分析迟到记录
                                CalculateEarlyTimeOut(curShift);

                                //分析早退记录
                                CalculateLateCount(curShift);

                                //bool isLateOrAbsent = false;

                                //if (true)//(store.ActTimeIn = -1) || (ActTimeOut = -1) || (ActBreakStart == -2) || (ActBreakEnd == -2)
                                //{
                                //    isLateOrAbsent = false;
                                //}

                                //if (isLateOrAbsent)
                                //    store.AbsentHours = 0;
                                //else
                                //{
                                //    store.lact = 0;
                                //    store.eact = 0;
                                //    store.LateHours = 0;
                                //    store.EarlyHours = 0;
                                //}


                                #endregion
                            }
                            else
                            {
                                #region 无缺勤情况

                                #endregion
                            }
                            #endregion
                            #endregion
                        }
                        else
                        {
                            #region 休息日

                            #endregion
                        }

                    }

                    //分析加班
                    if (includeOT)
                    {
                        new otanaovtBll().AnalyzeOT(ref store,curShift,lstOriAtData,true);
                    }

                    #region 保存

                    SaveAnalResult(curRosterDtl,curShift);

                    #endregion
                }
                #endregion
            }

        }

        public DateTime CompareTime(DateTime _curDay,string _time1, string _time2)
        {
            DateTime tmpDate = DateTime.Now;
            _time1 = DateTime.Parse(tmpDate.ToString("yyyy-MM-dd") + " " + _time1).ToString("HH:mm:ss");
            _time2 = DateTime.Parse(tmpDate.ToString("yyyy-MM-dd") + " " + _time2).ToString("HH:mm:ss");

            if ((_time1 == null) || (_time2 == null))
            {
                //
            }
            else
            {
                if (_time1.CompareTo(_time2) > 0)
                {
                    tmpDate = Convert.ToDateTime(UtilDatetime.FormatDate1(_curDay.AddDays(1)) + " " + _time2);
                }
                else
                {
                    tmpDate = Convert.ToDateTime(UtilDatetime.FormatDate1(_curDay) + " " + _time2);
                }

            }

            return tmpDate;
        }

        //#region CountLeaveTime
        //public Double CountLeaveTime(DateTime? _startDate, DateTime? _endDate, tatshift _shift, AtCalculationInfo _store)
        //{
        //    double lvHours = 0;
        //    double tmpHours = 0;

        //    if (_shift == null)
        //    {
        //        return 0;
        //    }
        //    else
        //    {

        //        if (_startDate<_store.StdTimeIn)
        //            _startDate = _store.StdTimeIn;

        //        if (_endDate>_store.StdTimeOut)
        //            _endDate = _store.StdTimeOut;

        //        if (_shift.igrl=="Y")
        //        {
        //            //小休在计算休假时考虑
        //            if (_shift.rttm1 > 0)
        //            {
        //                //有小休1
        //                if ((_startDate < _store.StdRest1Start) && (_endDate > _store.StdRest1End))
        //                {
        //                    tmpHours = (_store.StdRest1End.Value - _store.StdRest1Start.Value).TotalHours;
        //                }

        //                if ((_startDate < _store.StdRest1Start) && (_endDate <= _store.StdRest1End) && (_endDate > _store.StdRest1Start))
        //                {
        //                    tmpHours = (_endDate.Value - _store.StdRest1Start.Value).TotalHours;
        //                }

        //                if ((_startDate >= _store.StdRest1Start) && (_endDate > _store.StdRest1End) && (_startDate < _store.StdRest1End))
        //                {
        //                    tmpHours = (_store.StdRest1End.Value - _startDate.Value).TotalHours;
        //                }

        //                if ((_startDate >= _store.StdRest1Start) && (_endDate <= _store.StdRest1End))
        //                {
        //                    tmpHours = (_endDate.Value - _startDate.Value).TotalHours;
        //                }
        //            }
        //        }


        //        if (_shift.igbl=="Y")
        //        {
        //            //午休在计算休假时考虑
        //            if (_shift.bken=="Y")
        //            {
        //                //有午休
        //                if ((_startDate < _store.StdBreakStart) && (_endDate > _store.StdBreakEnd))
        //                {
        //                    tmpHours += (_store.StdBreakEnd.Value - _store.StdBreakStart.Value).TotalHours;
        //                }

        //                if ((_startDate < _store.StdBreakStart) && (_endDate <= _store.StdBreakEnd) && (_endDate>_store.StdBreakStart))
        //                {
        //                    tmpHours += (_endDate.Value - _store.StdBreakStart.Value).TotalHours;
        //                }

        //                if ((_startDate >= _store.StdBreakStart) && (_endDate > _store.StdBreakEnd) && (_startDate < _store.StdBreakEnd))
        //                {
        //                    tmpHours += (_store.StdBreakEnd.Value - _startDate.Value).TotalHours;
        //                }

        //                if ((_startDate >= _store.StdBreakStart) && (_endDate <= _store.StdBreakEnd))
        //                {
        //                    tmpHours += (_endDate.Value - _startDate.Value).TotalHours;
        //                }
        //            }
        //        }

        //        if (_shift.igrl == "Y")
        //        {
        //            //小休在计算休假时考虑
        //            if (_shift.rttm2 > 0)
        //            {
        //                //有小休1
        //                if ((_startDate < _store.StdRest2Start) && (_endDate > _store.StdRest2End))
        //                {
        //                    tmpHours += (_store.StdRest2End.Value - _store.StdRest2Start.Value).TotalHours;
        //                }

        //                if ((_startDate < _store.StdRest2Start) && (_endDate <= _store.StdRest2End) && (_endDate > _store.StdRest2Start))
        //                {
        //                    tmpHours += (_endDate.Value - _store.StdRest2Start.Value).TotalHours;
        //                }

        //                if ((_startDate >= _store.StdRest2Start) && (_endDate > _store.StdRest2End) && (_startDate < _store.StdRest2End))
        //                {
        //                    tmpHours += (_store.StdRest2End.Value - _startDate.Value).TotalHours;
        //                }

        //                if ((_startDate >= _store.StdRest2Start) && (_endDate <= _store.StdRest2End))
        //                {
        //                    tmpHours += (_endDate.Value - _startDate.Value).TotalHours;
        //                }
        //            }
        //        }

        //        lvHours = (_endDate.Value - _startDate.Value).TotalHours - tmpHours;

        //        //if ((_startDate <= _store.StdTimeIn) && (_endDate >= _store.StdTimeOut))
        //        //{
        //        //    //全天
        //        //    lvHours = (_store.StdTimeOut.Value - store.StdTimeIn.Value - (_store.StdBreakStart.Value - _store.StdBreakStart.Value)).TotalHours - ((_shift.rttm1 + _shift.rttm2)) / 60;
        //        //    lvStartDate = _store.StdTimeIn;
        //        //    lvEndDate = _store.StdTimeOut;
        //        //}

        //        //if ((_startDate <= _store.StdTimeIn) && (_endDate > _store.StdBreakStart) && (_endDate < _store.StdTimeOut))
        //        //{
        //        //    //
        //        //    lvHours = (_endDate.Value - _store.StdTimeIn.Value - (_store.StdBreakEnd.Value - _store.StdBreakStart.Value)).TotalHours - (_shift.rttm1 + _shift.rttm2) / 60;
        //        //    lvStartDate = _store.StdTimeIn;
        //        //    lvEndDate = _endDate.Value;
        //        //}

        //        //if ((_startDate <= _store.StdTimeIn) && (_endDate >= _store.StdBreakStart) && (_endDate <= _store.StdBreakEnd))
        //        //{
        //        //    //
        //        //    lvHours = (_store.StdBreakStart.Value - _store.StdTimeIn.Value).TotalHours - _shift.rttm1 / 60;
        //        //    lvStartDate = _store.StdTimeIn;
        //        //    lvEndDate = _store.StdBreakStart;
        //        //}

        //        //if ((_startDate <= _store.StdTimeIn) && (_endDate > _store.StdTimeIn) && (_endDate < _store.StdBreakStart))
        //        //{
        //        //    //
        //        //    lvHours = (_endDate.Value - _startDate.Value).TotalHours - _shift.rttm1 / 60;
        //        //    lvStartDate = _store.StdTimeIn;
        //        //    lvEndDate = _endDate.Value;
        //        //}

        //        //if ((_startDate > _store.StdTimeIn) && (_startDate < _store.StdBreakStart) && (_endDate >= _store.StdTimeOut))
        //        //{
        //        //    //
        //        //    lvHours = (_store.StdTimeOut.Value - _startDate.Value - (_store.StdBreakEnd.Value - _store.StdBreakStart.Value)).TotalHours - (_shift.rttm2) / 60;
        //        //    lvStartDate = _startDate.Value;
        //        //    lvEndDate = _store.StdTimeOut;
        //        //}

        //        //if ((_startDate > _store.StdTimeIn) && (_startDate < _store.StdBreakStart) && (_endDate > _store.StdBreakEnd) && (_endDate < _store.StdTimeOut))
        //        //{
        //        //    //
        //        //    lvHours = (_endDate.Value - _startDate.Value - (_store.StdBreakEnd.Value - _store.StdBreakStart.Value)).TotalHours;
        //        //    lvStartDate = _startDate.Value;
        //        //    lvEndDate = _endDate.Value;
        //        //}

        //        //if ((_startDate > _store.StdTimeIn) && (_startDate < _store.StdBreakStart) && (_endDate >= _store.StdBreakStart) && (_endDate <= _store.StdBreakEnd))
        //        //{
        //        //    //
        //        //    lvHours = (_store.StdBreakStart.Value - _startDate.Value).TotalHours;
        //        //    lvStartDate = _startDate.Value;
        //        //    lvEndDate = _store.StdBreakStart;
        //        //}

        //        //if ((_startDate > _store.StdTimeIn) && (_startDate < _store.StdBreakStart) && (_endDate >= _store.StdTimeIn) && (_endDate < _store.StdBreakStart))
        //        //{
        //        //    //
        //        //    lvHours = (_endDate.Value - _startDate.Value).TotalHours - _shift.rttm1 / 60;
        //        //    lvStartDate = _startDate.Value;
        //        //    lvEndDate = _endDate.Value;
        //        //}

        //        //if ((_startDate >= _store.StdBreakStart) && (_startDate <= _store.StdBreakEnd) && (_endDate >= _store.StdTimeOut))
        //        //{
        //        //    //
        //        //    lvHours = (_store.StdTimeOut.Value - _store.StdBreakEnd.Value).TotalHours - _shift.rttm2 / 60;
        //        //    lvStartDate = _store.StdBreakEnd;
        //        //    lvEndDate = _store.StdTimeOut;
        //        //}

        //        //if ((_startDate >= _store.StdBreakStart) && (_startDate <= _store.StdBreakEnd) && (_endDate > _store.StdBreakEnd) && (_endDate < _store.StdTimeOut))
        //        //{
        //        //    //
        //        //    lvHours = (_endDate.Value - _store.StdBreakEnd.Value).TotalHours - _shift.rttm2 / 60;
        //        //    lvStartDate = _store.StdBreakEnd;
        //        //    lvEndDate = _endDate.Value;
        //        //}

        //        //if ((_startDate > _store.StdBreakEnd) && (_startDate < _store.StdTimeOut) && (_endDate >= _store.StdTimeOut))
        //        //{
        //        //    //
        //        //    lvHours = (_store.StdTimeOut.Value - _startDate.Value).TotalHours;
        //        //    lvStartDate = _startDate.Value;
        //        //    lvEndDate = _store.StdTimeOut;
        //        //}

        //        //if ((_startDate > _store.StdBreakEnd) && (_startDate < _store.StdTimeOut) && (_endDate > _store.StdBreakEnd) && (_endDate < _store.StdTimeOut))
        //        //{
        //        //    //
        //        //    lvHours = (_endDate.Value - _startDate.Value).TotalHours;
        //        //    lvStartDate = _startDate.Value;
        //        //    lvEndDate = _store.StdTimeOut;
        //        //}

        //        return lvHours;
        //    }
        //}
        //#endregion

        #region SaveAnalResult
        private void SaveAnalResult(tatrosdtl curRosterDtl,tatshift curShift)
        {
            if (store.IsRestDay)
            {
                #region 休息日

                #endregion
            }
            else
            {
                #region 保存分析结果
                tatanarst analresult = new tatanarst();
                analresult.atdt = store.CurDay;
                analresult.athr = curShift.wkhr - store.AbsentHours - store.LvHours;
                if (analresult.athr < 0) analresult.athr = 0;

                analresult.atst = "0"; //Normal
                analresult.bret = store.ActBreakEnd;
                analresult.brst = store.ActBreakStart;
                analresult.emno = store.EmpInfo.emno;
                analresult.ifma = "N";
                analresult.intm = store.ActTimeIn;
                analresult.isrt = store.IsRestDay ? "Y" : "N";
                analresult.lmur = Function.GetCurrentUser();
                analresult.lmtm = DateTime.Now;
                analresult.ottm = store.ActTimeOut;
                analresult.rscd = curRosterDtl.rscd;
                analresult.sfcd = curShift.sfcd;
                analresult.remk = store.remark;
                analresult.rfid = Function.GetGUID();
                analresult.lvhr = store.LvHours;
                analresult.othr = store.OTHours;

                List<tatdatmnu> lstEmpManuAtData = lstManuAtData.Where(p => p.emno == store.EmpInfo.emno && p.atdt == store.CurDay).ToList();
                if (lstEmpManuAtData.Count > 0)
                {
                    analresult.betm = lstEmpManuAtData.First().betm;
                    analresult.bstm = lstEmpManuAtData.First().bstm;
                    analresult.intm = lstEmpManuAtData.First().ittm;
                    analresult.ottm = lstEmpManuAtData.First().ottm;
                    analresult.itmm = lstEmpManuAtData.First().ittm;
                    analresult.otmm = lstEmpManuAtData.First().ottm;
                }

                if (store.IsAbsent)
                {
                    #region 有缺勤
                    if ((store.IsEarly) || (store.IsLate))
                    {
                        analresult.atst = "1"; //Early,Late
                        analresult.iscf = "Y";
                    }
                    else
                    {
                        analresult.atst = "2"; //Abnormal
                        analresult.iscf = "N";
                    }
                    #endregion
                }
                else
                {
                    #region 无缺勤情况
                    analresult.atst = "0"; //Normal
                    analresult.iscf = "Y";
                    #endregion
                }
                atanarstDal analdal = new atanarstDal();
                analdal.DoInsert<tatanarst>(analresult);
                #endregion

                #region 如果有缺勤，保存缺勤记录
                if (store.IsAbsent)
                {
                    tatabsdtl absenceDtl = new tatabsdtl();

                    absenceDtl.abda = store.AbsentHours / curShift.wkhr;
                    absenceDtl.adam = 0;
                    absenceDtl.ahrs = store.AbsentHours;
                    absenceDtl.ahrm = 0;
                    absenceDtl.ahrr = store.AbsentHoursReal;
                    absenceDtl.atdt = store.CurDay;
                    absenceDtl.eact = store.EarlyCountTotal;
                    absenceDtl.ectm = 0;
                    absenceDtl.eahr = store.EarlyHoursTotal;
                    absenceDtl.ehrm = 0;
                    absenceDtl.emno = store.EmpInfo.emno;
                    absenceDtl.lmtm = DateTime.Now;
                    absenceDtl.lmur = Function.GetCurrentUser();
                    absenceDtl.lact = store.LateCountTotal;
                    absenceDtl.lctm = 0;
                    absenceDtl.lahr = store.LateHoursTotal;
                    absenceDtl.lhrm = 0;

                    atabsdtlDal absencedal = new atabsdtlDal();
                    absencedal.DoInsert<tatabsdtl>(absenceDtl);
                }
                #endregion
            }

        }
        #endregion

        //#region Analyze Overtime
        //public void AnalyzeOT(AtCalculationInfo _store, tatshift curShift, List<tatoridat> _lstOriAtData)
        //{
        //    DateTime effOTStart = compareTime(_store.CurDay, curShift.eati, curShift.tmin);
        //    DateTime effOTEnd = compareTime(_store.CurDay, curShift.lato, curShift.tmin);
        //    DateTime otStart = compareTime(_store.CurDay, curShift.nots, curShift.tmin);
        //    double othr = 0;
        //    double othrReal =0;
        //    string otType = AnalyzeOTType(_store,lstOTType);


        //    List<tatoridat> lstEmpOriAtData = _lstOriAtData.Where(p => p.sfid == _store.EmpInfo.sfid
        //                                                            && p.retm >= effOTStart
        //                                                            && p.retm < effOTEnd).ToList();
        //    if (lstEmpOriAtData.Count < 1)
        //    {
        //        //没有加班打卡记录
        //        othr = 0;
        //        othrReal = 0;
        //    }
        //    else
        //    {
        //        atotdetailDal dal = new atotdetailDal();

        //        //有上班前加班打卡记录
        //        if (lstEmpOriAtData.First().retm < _store.StdTimeIn)
        //        {
        //            TimeSpan diff = _store.StdTimeIn.Value - lstEmpOriAtData.First().retm;
        //            if (diff.TotalMinutes > curShift.miot)
        //            {
        //                othr = (diff.TotalMinutes % curShift.otun) * curShift.otun / 60;
        //                othrReal = diff.TotalMinutes / 60;
        //            }
        //            else
        //            {
        //                othr = 0;
        //                othrReal = 0;
        //            }

        //            dal.SaveOTDetail(_store.EmpInfo.emno, otType, _store.CurDay, lstEmpOriAtData.First().retm, _store.StdTimeIn.Value, othr, othrReal, 0.0);
        //        }

        //        //有下班后加班打卡记录
        //        if (lstEmpOriAtData.Last().retm > otStart)
        //        {
        //            TimeSpan diff = lstEmpOriAtData.Last().retm - otStart;
        //            if (diff.TotalMinutes > curShift.miot)
        //            {
        //                othr += (diff.TotalMinutes % curShift.otun) * curShift.otun / 60;
        //                othrReal += diff.TotalMinutes / 60;

        //                dal.SaveOTDetail(_store.EmpInfo.emno, otType, _store.CurDay, otStart, lstEmpOriAtData.Last().retm, othr, othrReal, 0);
        //            }
        //        }
        //    }

        //}

        //public string AnalyzeOTType(AtCalculationInfo _store, List<tatottype> _lstOTType)
        //{
        //    //判断加班类型的顺序: 个人日历>轮班>公司日历>默认日历
        //    string retOTType =string.Empty;

        //    var q = from p in _lstOTType
        //            where p.isdefault == "Y"
        //            select p;

        //    //默认日期
        //    if (q.ToList().Count < 1)
        //    {
        //        //没有指定默认日历
        //    }
        //    else
        //    {
        //        retOTType = q.ToList().First().otcd;
        //    }
            
        //    //公司日历
        //    if (_store.CalendarOTType.Trim() != string.Empty)
        //        retOTType = _store.CalendarOTType;

        //    //轮班日历
        //    if (_store.RosterOTType.Trim() != string.Empty)
        //        retOTType = _store.RosterOTType;

        //    //个人日历
        //    if (_store.PrivateOTType.Trim() != string.Empty)
        //        retOTType = _store.PrivateOTType;

        //    return retOTType;

        //}

    }
}

