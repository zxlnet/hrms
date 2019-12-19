using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using GotWell.HRMS.HRMSBusiness.Attendance;
using GotWell.Common;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Leave
{
    public class lvcryfwdBll : BaseBll
    {
         lvcryfwdDal dal = null;

         public lvcryfwdBll()
         {
             dal = new lvcryfwdDal();
             baseDal = dal;
         }

         public void CarryForward(List<ColumnInfo> _carryforwardParameters,
                     List<ColumnInfo> _personalParameters)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     int CarryStartYear = Convert.ToDateTime(_carryforwardParameters[0].ColumnValue).Year;
                     int CarryEndYear = Convert.ToDateTime(_carryforwardParameters[1].ColumnValue).Year;

                     List<vw_employment> lstStaff = null;
                     string sSqlStaff = string.Empty;


                     //期间内所有离职员工都不进行结转
                     atanaattBll bll = new atanaattBll();
                     bll.GetPersonals(_personalParameters, ref lstStaff, ref sSqlStaff, new DateTime(CarryEndYear, 12, 31).AddDays(1));

                     //得到休假类型
                     List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "iscf", ColumnValue = "Y" } };
                     BaseBll bBll = new BaseBll();
                     List<tlvleatyp> lstLeaveType = bBll.GetSelectedRecords<tlvleatyp>(parameters);

                     for (int i = 0; i < (CarryEndYear - CarryStartYear) + 1; i++)
                     {
                         //结转前删除所有的结转年度数据

                         parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "year", ColumnValue = (CarryStartYear + i).ToString() } };
                         dal.DoMultiDelete<tlvcryfwd>(parameters);


                         for (int n = 0; n < lstLeaveType.Count; n++)
                         {
                             string ltcd = lstLeaveType[n].ltcd;
                             for (int j = 0; j < lstStaff.Count; j++)
                             {
                                 DoCarryforward(lstStaff[j], ltcd, CarryStartYear + i);
                             }
                         }
                     }

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

        public void DoCarryforward(vw_employment _emp,string _ltcd,int _yearToCarry)
        {
            try
            {
                #region get Standard Work Hours
                double stdWorkHours = 0;
                string strStdWorkHours = ((StSystemConfig)Parameter.CURRENT_SYSTEM_CONFIG).PrSWHPD;

                try
                {
                    if (strStdWorkHours == string.Empty)
                        stdWorkHours = 8;
                    else
                        stdWorkHours = Convert.ToDouble(strStdWorkHours);
                }
                catch
                {
                    stdWorkHours = 8;
                }
                #endregion

                double leaveEntitlement = 0;
                double hoursToCarry = 0;
                LvSettingInfo settingInfo = new LvSettingInfo();
                settingInfo.emp = _emp;
                
                DateTime startDate = new DateTime(_yearToCarry,1,1);
                DateTime endDate = new DateTime(_yearToCarry,12,31);

                //根据员工个人设定
                lvdfbyempDal empDal = new lvdfbyempDal();
                settingInfo.dfbyEmployment = empDal.getLeaveSettingsByEmp(_emp.emno, _ltcd, endDate);
                leaveEntitlement = settingInfo.dfbyEmployment;

                //根据服务年数设定
                double yearDiff = DateTime.Now.Year - _yearToCarry;
                lvdfbyyearsDal yearDal = new lvdfbyyearsDal();
                settingInfo.dfbyYear = yearDal.getLeaveSettingsByYear(_ltcd, (settingInfo.emp.yearservice.Value - yearDiff) < 0 ? 0 : (settingInfo.emp.yearservice.Value - yearDiff));
                leaveEntitlement += settingInfo.dfbyYear;

                //根据其他设定
                lvdfbyotDal otherDay = new lvdfbyotDal();
                List<tlvdfbyod> lstSettingByOther = otherDay.getLeaveSettingsByOther(settingInfo.emp, _ltcd);

                var q1 = (from p in lstSettingByOther
                          where p.tlvdfbyot.dfva == settingInfo.emp.GetType().GetProperty(p.tlvdfbyot.tstdefcfg.finm).GetValue(settingInfo.emp, null).ToString().Trim()
                          where p.fryr <= ((settingInfo.emp.yearservice - yearDiff) < 0 ? 0 : (settingInfo.emp.yearservice - yearDiff))
                            && p.toyr >= (((settingInfo.emp.yearservice - yearDiff) - 1) < 0 ? 0 : ((settingInfo.emp.yearservice - yearDiff) - 1))
                            && p.tlvdfbyot.ltcd == _ltcd
                          select p).ToList();

                for (int i = 0; i < q1.Count; i++)
                {
                    settingInfo.dfbyOthers += lstSettingByOther[i].days;
                    leaveEntitlement += lstSettingByOther[i].days;
                }

                //取得上年结转
                lvcryfwdDal carryDal = new lvcryfwdDal();
                settingInfo.DaysCarry = carryDal.getCarryDaysByEmp(settingInfo.emp, _ltcd, startDate.Year);
                leaveEntitlement += settingInfo.DaysCarry;

                //取得最大限制
                lvlealmtDal limitDal = new lvlealmtDal();
                settingInfo.YearLimit = limitDal.GetYearlmbyEmp(settingInfo.emp, _ltcd, HRMS_Limit_Type.LeaveCarryforwardHours);

                //取得已经休假小时数
                lvleaappDal appDal = new lvleaappDal();
                settingInfo.YearConsume = appDal.getYearConsumedByEmp(settingInfo.emp, _ltcd, startDate);

                if ((settingInfo.YearLimit != -1) && (Math.Round(settingInfo.YearLimit / stdWorkHours, 2) <= (leaveEntitlement - Math.Round(settingInfo.YearConsume / stdWorkHours, 2))))
                {
                    //settingInfo.YearBalance = Math.Round(settingInfo.YearLimit - settingInfo.YearConsume, 2);
                    hoursToCarry = settingInfo.YearLimit;
                }
                else
                {
                    hoursToCarry = leaveEntitlement * stdWorkHours - settingInfo.YearConsume;
                }

                #region 保存
                bool isNeedToSave = true;


                if (((StSystemConfig)Parameter.CURRENT_SYSTEM_CONFIG).LvINCV=="Y")
                {
                    if (hoursToCarry <= 0)
                        hoursToCarry = 0;
                }

                if (isNeedToSave &&
                    (settingInfo.YearConsume != 0 ||
                     leaveEntitlement != 0 || settingInfo.YearLimit != -1 || hoursToCarry != 0))
                {
                    tlvcryfwd carryForward = new tlvcryfwd();
                    carryForward.daro = null;
                    carryForward.days = Math.Round(hoursToCarry / stdWorkHours, 2);
                    carryForward.hrcs = 0;
                    carryForward.emno = _emp.emno;
                    carryForward.hors = hoursToCarry;
                    carryForward.ltcd = _ltcd;
                    carryForward.year = _yearToCarry.ToString();
                    carryForward.lmur = Function.GetCurrentUser();
                    carryForward.lmtm = DateTime.Now;
                    carryForward.cnsu = settingInfo.YearConsume;
                    carryForward.enti = leaveEntitlement;
                    carryForward.limt = settingInfo.YearLimit;

                    DoInsert<tlvcryfwd>(carryForward);
                }
                #endregion
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
