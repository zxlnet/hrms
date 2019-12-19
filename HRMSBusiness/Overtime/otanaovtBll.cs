using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.HRMS.HRMSData.Overtime;

namespace GotWell.HRMS.HRMSBusiness.Overtime
{
    public class otanaovtBll : BaseBll
    {

        DateTime AnalStartDate;
        DateTime AnalEndDate;
        string sSqlStaff = string.Empty;
        List<vw_employment> lstStaff = null;
        List<tatpricld> lstPriCalendar = null;
        List<tatclddtl> lstCalendarDetails = null;
        List<tatrosdtl> lstRosterDetails = null;
        List<tatroshi> lstRosterHistory = null;
        List<tottype> lstOTType = null;
        List<tatoridat> lstOriAtData = null;
        AtCalculationInfo store = null;
        atanaattBll analatBll;

        public double TotalOTHours = 0;
        public string otcd = string.Empty;

        public otanaovtBll()
        {
            analatBll = new atanaattBll();
        }

        public void AnalyzeOvertime(List<ColumnInfo> _atdtParameters,
                            List<ColumnInfo> _personalParameters,totaplctn _otApp,bool _isSaveDetail)
        {
            try
            {
                AnalStartDate = Convert.ToDateTime(_atdtParameters[0].ColumnValue);
                AnalEndDate = Convert.ToDateTime(_atdtParameters[1].ColumnValue);

                analatBll.GetPersonals(_personalParameters,ref lstStaff,ref sSqlStaff,AnalStartDate);

                lstPriCalendar = analatBll.GetPrivateCalendar(_personalParameters, sSqlStaff, AnalStartDate, AnalEndDate);
                lstCalendarDetails = analatBll.GetCalendar(AnalStartDate, AnalEndDate);
                lstRosterDetails = analatBll.GetRosterDetails(AnalStartDate, AnalEndDate);
                lstRosterHistory = analatBll.GetRosterHistory(sSqlStaff, AnalStartDate, AnalEndDate);
                lstOTType = analatBll.GetOTType();
                lstOriAtData = analatBll.GetOriginalData(sSqlStaff, AnalStartDate, AnalEndDate);

                DoAnalyze(_otApp, _isSaveDetail);

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

        public void DoAnalyze(totaplctn _otApp, bool _isSaveDetail)
        {
            for (int i = 0; i < lstStaff.Count; i++)
            {
                TotalOTHours = 0;

                try
                {
                    DateTime calcStart = AnalStartDate;
                    DateTime calcEnd = AnalEndDate;
                    DateTime tmpStart = calcStart;
                    DateTime tmpEnd;

                    //如果加班跨天，需要拆分成多条数据
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

                            //分析此天的加班
                            AnalyzeOneDay(lstStaff[i], curRosterDtl, curShift, curRosterHistory, tmpStart, tmpEnd,_otApp, _isSaveDetail);

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
                    string x = ex.Message;
                }
            }
        }

        private void AnalyzeOneDay(vw_employment _emp, tatrosdtl curRosterDtl, tatshift curShift, tatroshi curRosterHistory, DateTime _calcStart, DateTime _calcEnd, totaplctn _otApp, bool _isSaveDetail)
        {
            store = new AtCalculationInfo();
            store.EmpInfo = _emp;
            store.CurDay = _calcStart;

            //curDayR = store.CurDay;

            //取得标准班次设定时间
            analatBll.GetStandardValue(curRosterDtl, curShift,ref store);

            //判断是否是休息日
            analatBll.CheckIsRestDay(curRosterDtl, curShift, curRosterHistory,lstPriCalendar, ref store);

            AnalyzeOT(ref store, curShift, lstOriAtData, _isSaveDetail);

            TotalOTHours += store.OTHours;

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

        public LvSettingInfo GetEmpOTSettings(string _emno, string _otcd, DateTime _otdt)
        {
            LvSettingInfo settingInfo = new LvSettingInfo();
            settingInfo.emp = GetSelectedObject<vw_employment>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emno } });


            //取得最大限制
            otlimitDal limitDal = new otlimitDal();
            settingInfo.WeekLimit = limitDal.GetWeeklmbyEmp(settingInfo.emp, _otcd, HRMS_Limit_Type.OvertimeHours);
            settingInfo.MonthLimit = limitDal.GetMonthlmbyEmp(settingInfo.emp, _otcd, HRMS_Limit_Type.OvertimeHours);
            settingInfo.YearLimit = limitDal.GetYearlmbyEmp(settingInfo.emp, _otcd, HRMS_Limit_Type.OvertimeHours);

            if (settingInfo.WeekLimit != -1)
                settingInfo.SummaryText.Add("Limit in Week: " + settingInfo.WeekLimit.ToString());
            if (settingInfo.MonthLimit != -1)
                settingInfo.SummaryText.Add("Limit in Month: " + settingInfo.MonthLimit.ToString());
            if (settingInfo.YearLimit != -1)
                settingInfo.SummaryText.Add("Limit in Year: " + settingInfo.YearLimit.ToString());


            //取得已经加班小时数
            otaplctnDal appDal = new otaplctnDal();
            settingInfo.WeekConsume = appDal.getWeekothrByEmp(settingInfo.emp, _otcd, _otdt);
            settingInfo.MonthConsume = appDal.getMonthothrByEmp(settingInfo.emp, _otcd, _otdt);
            settingInfo.YearConsume = appDal.getYearothrByEmp(settingInfo.emp, _otcd, _otdt);

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

            settingInfo.MinBalance = Math.Min((settingInfo.WeekBalance == -1 ? 10000 : settingInfo.WeekBalance), (settingInfo.MonthBalance == -1 ? 10000 : settingInfo.MonthBalance));
            settingInfo.MinBalance = Math.Min(settingInfo.MinBalance, (settingInfo.YearBalance == -1 ? 10000 : settingInfo.YearBalance));

            return settingInfo;
        }

        public LvSettingInfo GetEmpTTLVSettings(string _emno, string _otcd, DateTime _otdt)
        {
            LvSettingInfo settingInfo = new LvSettingInfo();
            settingInfo.emp = GetSelectedObject<vw_employment>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emno } });

            //取得最大限制
            otlimitDal limitDal = new otlimitDal();
            settingInfo.WeekLimit = limitDal.GetWeeklmbyEmp(settingInfo.emp, _otcd, HRMS_Limit_Type.OvertimeTTLVHours);
            settingInfo.MonthLimit = limitDal.GetMonthlmbyEmp(settingInfo.emp, _otcd, HRMS_Limit_Type.OvertimeTTLVHours);
            settingInfo.YearLimit = limitDal.GetYearlmbyEmp(settingInfo.emp, _otcd, HRMS_Limit_Type.OvertimeTTLVHours);

            //if (settingInfo.WeekLimit != -1)
            //    settingInfo.SummaryText.Add("Limit in Week: " + settingInfo.WeekLimit.ToString());
            //if (settingInfo.MonthLimit != -1)
            //    settingInfo.SummaryText.Add("Limit in Month: " + settingInfo.MonthLimit.ToString());
            //if (settingInfo.YearLimit != -1)
            //    settingInfo.SummaryText.Add("Limit in Year: " + settingInfo.YearLimit.ToString());


            //取得已经加班小时数
            otaplctnDal appDal = new otaplctnDal();
            settingInfo.WeekConsume = appDal.getWeekTTLVHoursByEmp(settingInfo.emp, _otcd, _otdt);
            settingInfo.MonthConsume = appDal.getMonthTTLVHoursByEmp(settingInfo.emp, _otcd, _otdt);
            settingInfo.YearConsume = appDal.getYearTTLVHoursByEmp(settingInfo.emp, _otcd, _otdt);

            //if (settingInfo.WeekLimit != -1)
            //    settingInfo.SummaryText.Add("Consume in Week: " + (Math.Round(settingInfo.WeekConsume, 2)).ToString());
            //if (settingInfo.MonthLimit != -1)
            //    settingInfo.SummaryText.Add("Consume in Month: " + (Math.Round(settingInfo.MonthConsume, 2)).ToString());
            //if (settingInfo.YearLimit != -1)
            //    settingInfo.SummaryText.Add("Consume in Year: " + (Math.Round(settingInfo.YearConsume, 2)).ToString());


            if (settingInfo.WeekLimit != -1)
            {
                settingInfo.WeekBalance = Math.Round(settingInfo.WeekLimit - settingInfo.WeekConsume, 2);
                //settingInfo.SummaryText.Add("Balance in Week: " + settingInfo.WeekBalance.ToString());
            }
            if (settingInfo.MonthLimit != -1)
            {
                settingInfo.MonthBalance = Math.Round(settingInfo.MonthLimit - settingInfo.MonthConsume, 2);
                //settingInfo.SummaryText.Add("Balance in Month: " + settingInfo.MonthBalance.ToString());
            }
            if (settingInfo.YearLimit != -1)
            {
                settingInfo.YearBalance = Math.Round(settingInfo.YearLimit - settingInfo.YearConsume, 2);
                //settingInfo.SummaryText.Add("Balance in Year: " + settingInfo.YearBalance.ToString());
            }

            settingInfo.MinBalance = Math.Min((settingInfo.WeekBalance == -1 ? 10000 : settingInfo.WeekBalance), (settingInfo.MonthBalance == -1 ? 10000 : settingInfo.MonthBalance));
            settingInfo.MinBalance = Math.Min(settingInfo.MinBalance, (settingInfo.YearBalance == -1 ? 10000 : settingInfo.YearBalance));

            return settingInfo;
        }
        
        public void AnalyzeOT(ref AtCalculationInfo _store, tatshift curShift, List<tatoridat> _lstOriAtData, bool _isSaveDetail)
        {
            DateTime effOTStart = DateTime.Parse(UtilDatetime.FormatDate1(_store.CurDay) + " " + curShift.eati);// analatBll.CompareTime(_store.CurDay, curShift.eati, curShift.tmin);
            //有效加班时间=第二日最早有效上班打卡时间之前
            DateTime effOTEnd = effOTStart.AddDays(1); //analatBll.CompareTime(_store.CurDay, curShift.tmot, curShift.lato);
            DateTime otStart = analatBll.CompareTime(_store.CurDay, curShift.tmin, curShift.nots);
            double othr = 0;
            double othrReal =0;
            double totalOthr = 0;
            double totalOthrReal = 0;
            List<tatshfott> lstShiftOT = new List<tatshfott>();

            if (lstOTType == null) lstOTType = analatBll.GetOTType();
            lstShiftOT = new BaseBll().GetSelectedRecords<tatshfott>(new List<ColumnInfo>() { }).OrderBy(p => p.otst).ToList();

            string otType = AnalyzeOTType(_store,lstOTType);

            string sfid = _store.EmpInfo.sfid;
            List<tatoridat> lstEmpOriAtData = _lstOriAtData.Where(p => p.sfid == sfid
                                                                    && p.retm >= effOTStart
                                                                    && p.retm < effOTEnd)
                                                           .OrderBy(p=>p.retm)
                                                           .ToList();
            if (lstEmpOriAtData.Count < 1)
            {
                //没有加班打卡记录
                othr = 0;
                othrReal = 0;
                totalOthr = 0;
                totalOthrReal = 0;
            }
            else
            {
                otdetailDal dal = new otdetailDal();

                //有上班前加班打卡记录
                //暂时不考虑上班打卡，2009-01-01 by Administrator
                //if (lstEmpOriAtData.First().retm < _store.StdTimeIn)
                //{
                //    TimeSpan diff = _store.StdTimeIn.Value - lstEmpOriAtData.First().retm;
                //    if (diff.TotalMinutes > curShift.miot)
                //    {
                //        othr = (diff.TotalMinutes % curShift.otun.Value) * curShift.otun.Value / 60;
                //        othrReal = diff.TotalMinutes / 60;
                //    }
                //    else
                //    {
                //        othr = 0;
                //        othrReal = 0;
                //    }

                //    if ((_isSaveDetail) && (othr!=0))
                //        dal.SaveOTDetail(_store.EmpInfo.emno, otType, _store.CurDay, lstEmpOriAtData.First().retm, _store.StdTimeIn.Value, othr, othrReal, 0.0);
                //}

                //有下班后加班打卡记录
                if (lstEmpOriAtData.Last().retm > otStart)
                {
                    if (lstShiftOT.Count > 0)
                    {
                        //定义了加班权重
                        DateTime retm = lstEmpOriAtData.Last().retm;
                        DateTime otst;
                        List<DateTime> lstDateTime = new List<DateTime>();
                        List<string> lstShiftOTType = new List<string>();
                        lstDateTime.Add(otStart);

                        for (int i = 0; i < lstShiftOT.Count; i++)
                        {
                            tatshfott shiftOT = lstShiftOT[i];
                            otst = analatBll.CompareTime(otStart, otStart.ToString("HH:mm:ss"), shiftOT.otst);
                            if (otst > retm)
                            {
                                lstDateTime.Add(retm);
                            }
                            else
                            {
                                if (i == (lstShiftOT.Count-1))
                                    lstDateTime.Add(retm);
                                else
                                    lstDateTime.Add(otst);
                            }
                            lstShiftOTType.Add(shiftOT.otcd);
                        }

                        for (int i = 1; i < lstDateTime.Count; i++)
                        {
                            //如果下一个时段的比最小加班时间小，则累计到上一个区间
                            TimeSpan diff1 = new TimeSpan(0, 0, 0);
                            if ((i+1)<=(lstDateTime.Count-1))
                            {
                                diff1 = lstDateTime[i+1] - lstDateTime[i];
                                if (diff1.TotalMinutes > curShift.miot)
                                    diff1 = new TimeSpan(0, 0, 0);
                            }

                            TimeSpan diff = lstDateTime[i] - lstDateTime[i - 1] + diff1;
                            if (diff.TotalMinutes > curShift.miot)
                            {
                                othr = Math.Floor(diff.TotalMinutes / curShift.otun.Value) * curShift.otun.Value / 60;
                                othrReal = diff.TotalMinutes / 60;

                                totalOthr += othr;
                                totalOthrReal += othrReal;

                                if ((_isSaveDetail) && (othr != 0))
                                    dal.SaveOTDetail(_store.EmpInfo.emno, lstShiftOTType[i - 1], _store.CurDay, lstDateTime[i - 1], lstDateTime[i], othr, othrReal, 0);
                            }
                        }
                    }
                    else
                    {
                        //没有定义加班权重
                        TimeSpan diff = lstEmpOriAtData.Last().retm - otStart;
                        if (diff.TotalMinutes > curShift.miot)
                        {
                            othr = Math.Floor(diff.TotalMinutes / curShift.otun.Value) * curShift.otun.Value / 60;
                            othrReal = diff.TotalMinutes / 60;

                            totalOthr += othr;
                            totalOthrReal += othrReal;

                            if ((_isSaveDetail) && (othr != 0))
                                dal.SaveOTDetail(_store.EmpInfo.emno, otType, _store.CurDay, otStart, lstEmpOriAtData.Last().retm, othr, othrReal, 0);
                        }
                    }
                }
            }

            otcd = otType;
            _store.OTHours = totalOthr;
        }

        public string AnalyzeOTType(AtCalculationInfo _store, List<tottype> _lstOTType)
        {
            //判断加班类型的顺序: 个人日历>轮班>公司日历>默认日历
            string retOTType =string.Empty;

            var q = from p in _lstOTType
                    where p.isdf == "Y"
                    select p;

            //默认日期
            if (q.ToList().Count < 1)
            {
                //没有指定默认日历
            }
            else
            {
                retOTType = q.ToList().First().otcd;
            }
            
            //公司日历
            if (_store.CalendarOTType != null)
            {
                if (_store.CalendarOTType != null)
                {
                    if (_store.CalendarOTType.Trim() != string.Empty)
                        retOTType = _store.CalendarOTType;
                }
            }

            //轮班日历
            if (_store.RosterOTType != null)
            {
                if (_store.RosterOTType.Trim() != string.Empty)
                    retOTType = _store.RosterOTType;
            }

            //个人日历
            if (_store.PrivateOTType != null)
            {
                if (_store.PrivateOTType.Trim() != string.Empty)
                    retOTType = _store.PrivateOTType;
            }

            return retOTType;

        }

    }
}
