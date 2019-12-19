using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Utility;
using GotWell.Common;

namespace GotWell.HRMS.HRMSBusiness.Leave
{
     public class lvanalevBll : BaseBll
    {
        lvanalevDal localDal = null;
        DateTime AnalStartDate;
        DateTime AnalEndDate;
        string sSqlStaff = string.Empty;
        List<vw_employment> lstStaff = null;
        List<tatpricld> lstPriCalendar = null;
        List<tatclddtl> lstCalendarDetails = null;
        List<tatrosdtl> lstRosterDetails = null;
        List<tatroshi> lstRosterHistory = null;
        List<tlvleaapp> lstLeaveApp = null;

        AtCalculationInfo store = null;
        atanaattBll analatBll;

        public double TotalLvHours = 0;
        public double TotalLvDays = 0;

        public lvanalevBll()
        {
            localDal = new lvanalevDal();
            baseDal = localDal;
            analatBll = new atanaattBll();
        }

        public void AnalyzeLeave(List<ColumnInfo> _atdtParameters,
                            List<ColumnInfo> _personalParameters,tlvleaapp _leaveApp,bool _isSaveDetail)
        {
            try
            {
                //analatBll.GetAnalyzeDateRange(_atdtParameters, ref AnalStartDate, ref AnalEndDate);
                AnalStartDate = Convert.ToDateTime(_atdtParameters[0].ColumnValue);
                AnalEndDate = Convert.ToDateTime(_atdtParameters[1].ColumnValue);

                analatBll.GetPersonals(_personalParameters,ref lstStaff,ref sSqlStaff,AnalStartDate);

                lstPriCalendar = analatBll.GetPrivateCalendar(_personalParameters, sSqlStaff, AnalStartDate, AnalEndDate);
                lstCalendarDetails = analatBll.GetCalendar(AnalStartDate, AnalEndDate);
                lstRosterDetails = analatBll.GetRosterDetails(AnalStartDate, AnalEndDate);
                lstRosterHistory = analatBll.GetRosterHistory(sSqlStaff, AnalStartDate, AnalEndDate);
                //lstLeaveApp = analatBll.GetLeaveApp(sSqlStaff, AnalStartDate, AnalEndDate);

                DoAnalyze(_leaveApp,_isSaveDetail);

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

        public void DoAnalyze(tlvleaapp _leaveApp,bool _isSaveDetail)
        {
            for (int i = 0; i < lstStaff.Count; i++)
            {
                TotalLvHours = 0;
                TotalLvDays = 0;

                try
                {
                    DateTime calcStart = AnalStartDate;
                    DateTime calcEnd = AnalEndDate;
                    DateTime tmpStart = calcStart;
                    DateTime tmpEnd;

                    //如果请假跨天，需要拆分成多条数据
                    for (float k = 0; k < (calcEnd - calcStart).TotalDays; k++)
                    {
                        if ((calcEnd - tmpStart).TotalDays > 1)
                        {
                            tmpEnd = Convert.ToDateTime(UtilDatetime.FormatDate1(tmpStart.AddDays(1)) + " 00:00:00");
                        }
                        else
                            tmpEnd = calcEnd;

                        //取轮班历史
                        List<tatroshi> lstEmpRosterHistory = lstRosterHistory.Where(p => p.emno == lstStaff[i].emno
                                                                                       && ((p.exdt.HasValue == false) ||
                                                                                           (p.exdt.Value > tmpEnd))
                                                                                       && (p.efdt <= tmpStart)
                                                                                            ).ToList();
                        //确定轮班和班次
                        if (lstEmpRosterHistory.Count > 0)
                        {
                            //如果发现多条，则只取第一条
                            tatroshi curRosterHistory = lstEmpRosterHistory[0];

                            List<tatrosdtl> lstEmpRosterDtl = lstRosterDetails.Where(p => p.rscd == curRosterHistory.rscd).ToList();

                            //确定轮班明细和班次
                            tatrosdtl curRosterDtl = getCurrentRoster(curRosterHistory, lstEmpRosterDtl, tmpStart);

                            tatshift curShift = curRosterDtl.tatshift;

                            //分析此天的休假
                            AnalyzeOneDay(lstStaff[i], curRosterDtl, curShift, curRosterHistory, tmpStart, tmpEnd,_leaveApp, _isSaveDetail);

                            tmpStart = tmpEnd;
                        }
                        else
                        {
                            //如果没有定义轮班该如何计算？？？？

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void AnalyzeOneDay(vw_employment _emp, tatrosdtl curRosterDtl, tatshift curShift,tatroshi curRosterHistory, DateTime _calcStart,DateTime _calcEnd,tlvleaapp _leaveApp,bool _isSaveDetail)
        {
            store = new AtCalculationInfo();
            store.EmpInfo = _emp;
            store.CurDay = _calcStart;

            //curDayR = store.CurDay;

            //取得标准班次设定时间
            analatBll.GetStandardValue(curRosterDtl, curShift,ref store);

            //判断是否是休息日
            analatBll.CheckIsRestDay(curRosterDtl, curShift, curRosterHistory,lstPriCalendar, ref store);

            if (store.IsRestDay == false)
            {
                #region 非休息日
                if ((_calcStart <= store.StdTimeIn) && (_calcEnd >= store.StdTimeOut))
                {
                    //抓班次设定的工作小时数和天数
                    store.LvStart = store.StdTimeIn;
                    store.LvEnd = store.StdTimeOut;

                    store.LvHours = Convert.ToDouble(curShift.wkhr);
                    store.LvDays = Convert.ToDouble(curShift.wkda);
                }
                else
                {
                    if (_calcStart <= store.StdTimeIn)
                        store.LvStart = store.StdTimeIn;
                    else
                        store.LvStart = _calcStart;

                    if (_calcEnd >= store.StdTimeOut)
                        store.LvEnd = store.StdTimeOut;
                    else
                        store.LvEnd = _calcEnd;

                    store.LvHours = CountLeaveTime(store.LvStart, store.LvEnd, curShift, store);
                    store.LvHours = Math.Round(store.LvHours, 2);
                    //计算天数
                    store.LvDays = Math.Round((store.LvHours / Convert.ToDouble(curShift.wkhr)) * Convert.ToDouble(curShift.wkda),2);
                }
                #endregion

            }
            else
            {
                #region 休息日
                store.LvHours = 0;
                store.LvDays = 0;
                #endregion
            }

            TotalLvHours += store.LvHours;
            TotalLvDays += store.LvDays;

            if ((_isSaveDetail) &&(_leaveApp!=null) && (store.LvHours>0))
            {
                //如果需要保存则保存每一条明细到DTL表
                tlvleaapd newdtl = new tlvleaapd();

                newdtl.emno = _emp.emno;
                newdtl.days = store.LvDays;
                newdtl.hurs = store.LvHours;
                newdtl.ltcd = _leaveApp.ltcd;
                newdtl.apno = _leaveApp.apno;
                newdtl.totm = store.LvEnd.Value;
                newdtl.frtm = store.LvStart.Value;

                DoInsert<tlvleaapd>(newdtl);
            }
        }

        private tatrosdtl getCurrentRoster(tatroshi curRosterHistory,List<tatrosdtl> curRosterDtl, DateTime startDate)
        {
            #region 判断实效、失效日期
            if (curRosterHistory.efdt.Value > startDate)
            {
                //如果轮班历史的有效日期大于当前计算日期，则从轮班历史生效日期开始计算
                //startDateByPerson = _rosterHis.efdt.Value;
                return null;
            }

            if (curRosterHistory.tatroster.efdt.Value > startDate)
            {
                //如果轮班的有效日期大于当前计算日期，则从轮班生效日期开始计算
                //startDateByPerson = _rosterHis.tatroster.efdt.Value;
                return null;
            }
            #endregion

            //判断需要从哪个班次开始计算
            int shiftIndex = (int)((TimeSpan)(startDate - curRosterHistory.tatroster.efdt)).TotalDays % curRosterDtl.Count;

            return curRosterDtl[shiftIndex];

        }

        public LvSettingInfo GetEmpLeaveSettings(string _emno, string _ltcd, DateTime _leavedate)
        {
            LvSettingInfo settingInfo = new LvSettingInfo();
            double leaveBalance = 0;
            settingInfo.emp = GetSelectedObject<vw_employment>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emno } });

            //根据员工个人设定
            lvdfbyempDal empDal = new lvdfbyempDal();
            settingInfo.dfbyEmployment = empDal.getLeaveSettingsByEmp(_emno, _ltcd,DateTime.Now);
            leaveBalance = settingInfo.dfbyEmployment;
            if (settingInfo.dfbyEmployment > 0)
                settingInfo.SummaryText.Add("By Employment: " + settingInfo.dfbyEmployment.ToString());

            //根据服务年数设定
            lvdfbyyearsDal yearDal = new lvdfbyyearsDal();
            settingInfo.dfbyYear = yearDal.getLeaveSettingsByYear(_ltcd, Convert.ToDouble(DateTime.Now.Year));
            leaveBalance += settingInfo.dfbyYear;
            if (settingInfo.dfbyYear > 0)
                settingInfo.SummaryText.Add("By Year: " + settingInfo.dfbyYear.ToString());

            //根据其他设定
            lvdfbyotDal otherDay = new lvdfbyotDal();
            List<tlvdfbyod> lstSettingByOther = otherDay.getLeaveSettingsByOther(settingInfo.emp, _ltcd);

            var q1 = (from p in lstSettingByOther
                      where p.tlvdfbyot.dfva == settingInfo.emp.GetType().GetProperty(p.tlvdfbyot.tstdefcfg.finm).GetValue(settingInfo.emp, null).ToString().Trim()
                      where p.fryr <= settingInfo.emp.yearservice
                        && p.toyr >= (settingInfo.emp.yearservice - 1)
                      select p).ToList();

            for (int i = 0; i < q1.Count; i++)
            {
                settingInfo.dfbyOthers += lstSettingByOther[i].days;
                leaveBalance += lstSettingByOther[i].days;
                if (lstSettingByOther[i].days > 0)
                    settingInfo.SummaryText.Add("By " + lstSettingByOther[i].tlvdfbyot.dftx + ": " + lstSettingByOther[i].days.ToString());
            }

            //取得上年结转
            lvcryfwdDal carryDal = new lvcryfwdDal();
            settingInfo.DaysCarry = carryDal.getCarryDaysByEmp(settingInfo.emp, _ltcd, Convert.ToDouble(DateTime.Now.Year));
            leaveBalance += settingInfo.DaysCarry;
            if (settingInfo.DaysCarry > 0)
                settingInfo.SummaryText.Add("From Carryforward: " + settingInfo.DaysCarry.ToString());

            //取得最大限制
            lvlealmtDal limitDal = new lvlealmtDal();
            settingInfo.WeekLimit = limitDal.GetWeeklmbyEmp(settingInfo.emp, _ltcd, HRMS_Limit_Type.LeaveHours);
            settingInfo.MonthLimit = limitDal.GetMonthlmbyEmp(settingInfo.emp, _ltcd, HRMS_Limit_Type.LeaveHours);
            settingInfo.YearLimit = limitDal.GetYearlmbyEmp(settingInfo.emp, _ltcd, HRMS_Limit_Type.LeaveHours);

            if (settingInfo.WeekLimit != -1)
                settingInfo.SummaryText.Add("Limit in Week: " + settingInfo.WeekLimit.ToString());
            if (settingInfo.MonthLimit != -1)
                settingInfo.SummaryText.Add("Limit in Month: " + settingInfo.MonthLimit.ToString());
            if (settingInfo.YearLimit != -1)
                settingInfo.SummaryText.Add("Limit in Year: " + settingInfo.YearLimit.ToString());


            //取得已经休假天数
            lvleaappDal appDal = new lvleaappDal();
            settingInfo.WeekConsume = appDal.getWeekConsumedByEmp(settingInfo.emp, _ltcd, _leavedate);
            settingInfo.MonthConsume = appDal.getMonthConsumedByEmp(settingInfo.emp, _ltcd, _leavedate);
            settingInfo.YearConsume = appDal.getYearConsumedByEmp(settingInfo.emp, _ltcd, _leavedate);

            if (settingInfo.WeekLimit != -1)
                settingInfo.SummaryText.Add("Consume in Week: " + (Math.Round(settingInfo.WeekConsume,2)).ToString());
            if (settingInfo.MonthLimit != -1)
                settingInfo.SummaryText.Add("Consume in Month: " + (Math.Round(settingInfo.MonthConsume,2)).ToString());
            if (settingInfo.YearLimit != -1)
                settingInfo.SummaryText.Add("Consume in Year: " + (Math.Round(settingInfo.YearConsume,2)).ToString());


            if (settingInfo.WeekLimit != -1)
            {
                settingInfo.WeekBalance = Math.Round(settingInfo.WeekLimit - settingInfo.WeekConsume,2);
                settingInfo.SummaryText.Add("Balance in Week: " + settingInfo.WeekBalance.ToString());
            }
            if (settingInfo.MonthLimit != -1)
            {
                settingInfo.MonthBalance = Math.Round(settingInfo.MonthLimit - settingInfo.MonthConsume,2);
                settingInfo.SummaryText.Add("Balance in Month: " + settingInfo.MonthBalance.ToString());
            }
            if (settingInfo.YearLimit != -1)
            {
                settingInfo.YearBalance = Math.Round(settingInfo.YearLimit - settingInfo.YearConsume,2);
                settingInfo.SummaryText.Add("Balance in Year: " + settingInfo.YearBalance.ToString());
            }

            return settingInfo;
        }


        #region CountLeaveTime
        public Double CountLeaveTime(DateTime? _startDate, DateTime? _endDate, tatshift _shift, AtCalculationInfo _store)
        {
            double lvHours = 0;
            double tmpHours = 0;

            if (_shift == null)
            {
                return 0;
            }
            else
            {

                if (_startDate < _store.StdTimeIn)
                    _startDate = _store.StdTimeIn;

                if (_endDate > _store.StdTimeOut)
                    _endDate = _store.StdTimeOut;

                if (_shift.igrl == "Y")
                {
                    //如果小休属于工作时数内，则请假要考虑
                    if (_shift.rttm1 > 0)
                    {
                        //有小休1
                        if ((_startDate < _store.StdRest1Start) && (_endDate > _store.StdRest1End))
                        {
                            tmpHours = (_store.StdRest1End.Value - _store.StdRest1Start.Value).TotalHours;
                        }

                        if ((_startDate < _store.StdRest1Start) && (_endDate <= _store.StdRest1End) && (_endDate > _store.StdRest1Start))
                        {
                            tmpHours = (_endDate.Value - _store.StdRest1Start.Value).TotalHours;
                        }

                        if ((_startDate >= _store.StdRest1Start) && (_endDate > _store.StdRest1End) && (_startDate < _store.StdRest1End))
                        {
                            tmpHours = (_store.StdRest1End.Value - _startDate.Value).TotalHours;
                        }

                        if ((_startDate >= _store.StdRest1Start) && (_endDate <= _store.StdRest1End))
                        {
                            tmpHours = (_endDate.Value - _startDate.Value).TotalHours;
                        }
                    }
                }


                if (_shift.igbl == "Y")
                {
                    //如果午休属于工作时数，则在请假时要考虑
                    if (_shift.bken == "Y")
                    {
                        //有午休
                        if ((_startDate < _store.StdBreakStart) && (_endDate > _store.StdBreakEnd))
                        {
                            tmpHours += (_store.StdBreakEnd.Value - _store.StdBreakStart.Value).TotalHours;
                        }

                        if ((_startDate < _store.StdBreakStart) && (_endDate <= _store.StdBreakEnd) && (_endDate > _store.StdBreakStart))
                        {
                            tmpHours += (_endDate.Value - _store.StdBreakStart.Value).TotalHours;
                        }

                        if ((_startDate >= _store.StdBreakStart) && (_endDate > _store.StdBreakEnd) && (_startDate < _store.StdBreakEnd))
                        {
                            tmpHours += (_store.StdBreakEnd.Value - _startDate.Value).TotalHours;
                        }

                        if ((_startDate >= _store.StdBreakStart) && (_endDate <= _store.StdBreakEnd))
                        {
                            tmpHours += (_endDate.Value - _startDate.Value).TotalHours;
                        }
                    }
                }

                if (_shift.igrl == "Y")
                {
                    //小休在计算休假时考虑
                    if (_shift.rttm2 > 0)
                    {
                        //有小休1
                        if ((_startDate < _store.StdRest2Start) && (_endDate > _store.StdRest2End))
                        {
                            tmpHours += (_store.StdRest2End.Value - _store.StdRest2Start.Value).TotalHours;
                        }

                        if ((_startDate < _store.StdRest2Start) && (_endDate <= _store.StdRest2End) && (_endDate > _store.StdRest2Start))
                        {
                            tmpHours += (_endDate.Value - _store.StdRest2Start.Value).TotalHours;
                        }

                        if ((_startDate >= _store.StdRest2Start) && (_endDate > _store.StdRest2End) && (_startDate < _store.StdRest2End))
                        {
                            tmpHours += (_store.StdRest2End.Value - _startDate.Value).TotalHours;
                        }

                        if ((_startDate >= _store.StdRest2Start) && (_endDate <= _store.StdRest2End))
                        {
                            tmpHours += (_endDate.Value - _startDate.Value).TotalHours;
                        }
                    }
                }

                lvHours = (_endDate.Value - _startDate.Value).TotalHours - tmpHours;

                return lvHours;
            }
        }
        #endregion
    }
}
