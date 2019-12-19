using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Utility;
using GotWell.Common;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class praccalcBll : BaseBll
    {
        praccalcDal dal = null;
        public UtilLog log = new UtilLog();

        public praccalcBll()
        {
            dal = new praccalcDal();
            baseDal = dal;
        }

        public void InitLog()
        {
            string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
            string fileName = "CostCenterAccountCalc" + "_" + UtilDatetime.FormatDateTime5(DateTime.Now) + ".trc";

            //file header
            List<string> header = new List<string>();
            header.Add("Bank account allocation log at " + UtilDatetime.FormatDateTime5(DateTime.Now) + " by " + Function.GetCurrentUser());
            header.Add("LOG");
            log.LogHeader(fileName, header);

            log = new UtilLog(fileName, Log_LoggingLevel.Admin);

        }

        public void Calculate(string rnno, string cond, List<ColumnInfo> personalParameters, string perd)
        {
            try
            {
                InitLog();

                List<vw_employment> lstStaff = GetPersonals(personalParameters,cond);

                //目前只分配
                List<tprsalhi> lstSalaryHistory = GetSalaryHistory(rnno);
                List<tprbaalcr> lstAccountAllocateRule = GetAllocationRule();
                List<tpsaccont> lstAccount = GetAccount();

                log.LogInfoWithLevel("Start allocate salary to bank account.", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Period:" + perd + "." , Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Run No:" + rnno + ".", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Total " + lstStaff.Count.ToString() + " staff(s) to allocate.", Log_LoggingLevel.Normal);

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,new TimeSpan(1,0,0)))
                {
                    for (int i = 0; i < lstStaff.Count; i++)
                    {
                        log.LogInfoWithLevel("Start to allocate staff[" + lstStaff[i].sfid + "," + lstStaff[i].ntnm + "]." , Log_LoggingLevel.Normal);

                        //Delete first
                        List<ColumnInfo> runParameter = new List<ColumnInfo>() { 
                                new ColumnInfo() { ColumnName = "rnno", ColumnValue = rnno } ,
                                new ColumnInfo() { ColumnName = "emno", ColumnValue = lstStaff[i].emno } 
                        };
                        DoMultiDelete<tpraccalc>(runParameter);

                        List<tprsalhi> lstCurrentSalaryHistory = lstSalaryHistory.Where(p => p.emno == lstStaff[i].emno).ToList();
                        List<tprbaalcr> lstCurrentAccountAllocateRule = lstAccountAllocateRule.Where(p => p.emno == lstStaff[i].emno).ToList();
                        List<tpsaccont> lstCurrentAccount = lstAccount.Where(p => p.emno == lstStaff[i].emno && p.isdf == "Y").ToList();

                        CalculateOneByOne(lstStaff[i], lstCurrentSalaryHistory, lstCurrentAccountAllocateRule, lstCurrentAccount);

                        log.LogInfoWithLevel("End to allocate staff[" + lstStaff[i].sfid + "," + lstStaff[i].ntnm + "]." , Log_LoggingLevel.Normal);
                        log.LogInfoWithLevel(" " , Log_LoggingLevel.Normal);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<vw_employment> GetPersonals(List<ColumnInfo> _personalParameters, string _cond)
        {
            psemplymDal dal = new psemplymDal();
            List<vw_employment> _lstStaff = dal.GetEmploymentForAllocation(_personalParameters);

            return _lstStaff;
        }

        public List<tprsalhi> GetSalaryHistory(string rnno)
        {
            prsalhisDal dal = new prsalhisDal();

            List<ColumnInfo> lstParameters = new List<ColumnInfo>() {
                new ColumnInfo(){ColumnName="rnno",ColumnValue=rnno}
            };

            List<tprsalhi> _lstSalaryHistory = dal.GetSalaryHistoryForAccountAllocate(lstParameters);

            return _lstSalaryHistory;
        }

        public List<tprbaalcr> GetAllocationRule()
        {
            prbaalcrDal dal = new prbaalcrDal();
            List<tprbaalcr> _lstAllocationRule = dal.GetSelectedRecords<tprbaalcr>(new List<ColumnInfo>() { });

            return _lstAllocationRule;
        }

        public List<tpsaccont> GetAccount()
        {
            psaccontDal dal = new psaccontDal();
            List<tpsaccont> _lstAccount = dal.GetSelectedRecords<tpsaccont>(new List<ColumnInfo>() { });

            return _lstAccount;
        }

        public void CalculateOneByOne(vw_employment currentStaff,List<tprsalhi> lstCurrentSalaryHistory, List<tprbaalcr> lstCurrentAccountAllocateRule, List<tpsaccont> lstCurrentAccount)
        {
            for (int i=0;i<lstCurrentSalaryHistory.Count;i++)
            {
                try
                {
                    log.LogInfoWithLevel("Salary Item[" + lstCurrentSalaryHistory[i].itcd + "]." , Log_LoggingLevel.Admin);
                    List<tprbaalcr> lstRule = lstCurrentAccountAllocateRule.Where(p => p.itcd == lstCurrentSalaryHistory[i].itcd).ToList();
                    string vtyp = string.Empty;
                    double valu = 0;
                    string acno = string.Empty;
                    double tmpSum = 0;
                    int sqno = 0;

                    for (int j = 0; j < lstRule.Count;j++ )
                    {
                        //有针对该itcd的分配规则
                        //如果定义了多套规则，则取第一套，其他忽略
                        vtyp = lstRule[j].vtyp;
                        valu = lstRule[j].valu.HasValue?lstRule[j].valu.Value:0;
                        acno = lstRule[j].acno;

                        log.LogInfoWithLevel("Rule definied at salary item [" + vtyp + "," + valu.ToString() + "," + acno + "]." , Log_LoggingLevel.Admin);

                        sqno++;
                        tmpSum += HandleIt(currentStaff, lstCurrentSalaryHistory[i], vtyp, valu, acno, sqno);

                    }

                    if (lstRule.Count <= 0)
                    {
                        //无针对该itcd的分配规则
                        lstRule = lstCurrentAccountAllocateRule.Where(p => p.itcd == string.Empty).ToList();
                        for (int n = 0; n < lstRule.Count;n++ )
                        {
                            //有针对所有itcd定义的分配规则
                            vtyp = lstRule[n].vtyp;
                            valu = lstRule[n].valu.HasValue ? lstRule[n].valu.Value : 0;
                            acno = lstRule[n].acno;

                            log.LogInfoWithLevel("Rule definied at all salary item [" + vtyp + "," + valu.ToString() + "," + acno + "]." , Log_LoggingLevel.Admin);

                            sqno++;
                            tmpSum += HandleIt(currentStaff, lstCurrentSalaryHistory[i], vtyp, valu, acno,sqno);
                        }

                        if (lstRule.Count <= 0)
                        {
                            //无针对所有itcd定义的分配规则
                            if (lstCurrentAccount.Count > 0)
                            {

                                //有默认账号,100%分配到该账号
                                vtyp = "Percentage";
                                valu = 100;
                                acno = lstCurrentAccount[0].acno;

                                log.LogInfoWithLevel("Allocate 100% to default account[" + acno + "].", Log_LoggingLevel.Admin);

                                sqno++;
                                tmpSum += HandleIt(currentStaff, lstCurrentSalaryHistory[i], vtyp, valu, acno, sqno);
                            }
                            else
                            {
                                log.LogInfoWithLevel("No any setting(s) found,do not allocate it." , Log_LoggingLevel.Admin);
                                //无默认账号，不分配
                                vtyp = string.Empty;
                                valu = 0;
                                acno = string.Empty;
                            }
                        }
                    }

                    //余额分配到默认账号，如果没有定义，则不分配
                    if (tmpSum < lstCurrentSalaryHistory[i].valu)
                    {
                        if (lstCurrentAccount.Count > 0)
                        {
                            acno = lstCurrentAccount[0].acno;

                            log.LogInfoWithLevel("Left amount [" + (lstCurrentSalaryHistory[i].valu - tmpSum).ToString() + "] was allocate to default account[" + acno + "].", Log_LoggingLevel.Admin);

                            sqno++;
                            HandleLeft(currentStaff, lstCurrentSalaryHistory[i], (lstCurrentSalaryHistory[i].valu - tmpSum), acno, sqno);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.LogInfoWithLevel("Error found,ignore it. Error message:[" + ex.Message + "." , Log_LoggingLevel.Admin);
                }
            }
        }

        private void HandleLeft(vw_employment currentStaff, tprsalhi salHis, double valu, string acno, int sqno)
        {
            if (acno != string.Empty)
            {
                tpraccalc accalc = new tpraccalc();
                accalc.acno = acno;
                accalc.amnt = valu;
                accalc.emno = currentStaff.emno;
                accalc.itcd = salHis.itcd;
                accalc.perd = salHis.perd;
                accalc.pkty = salHis.pkty;
                accalc.rnno = salHis.rnno;
                accalc.sqno = sqno;
                accalc.crcd = salHis.crcd;
                accalc.pdcd = salHis.pdcd;

                DoInsert<tpraccalc>(accalc);
            }

        }

        private double HandleIt(vw_employment currentStaff, tprsalhi salHis,string vtyp,double valu,string acno,int sqno)
        {
            double resultValue = 0;
            double validValue = 0;

            if (salHis.ajva.HasValue)
                validValue = salHis.ajva.Value;
            else
                validValue = salHis.valu;

            switch (vtyp)
            {
                case "Percentage":
                    resultValue = ReturnValueByPercentage(validValue, valu);
                    break;
                case "Value":
                    resultValue = ReturnValueByValue(validValue, valu);
                    break;
                default:
                    resultValue = 0;
                    break;
            }

            if (acno != string.Empty)
            {
                tpraccalc accalc = new tpraccalc();
                accalc.acno = acno;
                accalc.amnt = resultValue;
                accalc.emno = currentStaff.emno;
                accalc.itcd = salHis.itcd;
                accalc.perd = salHis.perd;
                accalc.pkty = salHis.pkty;
                accalc.rnno = salHis.rnno;
                accalc.sqno = sqno;
                accalc.crcd = salHis.crcd;
                accalc.pdcd = salHis.pdcd;

                DoInsert<tpraccalc>(accalc);
            }

            return resultValue;
        }

        private double ReturnValueByPercentage(double valu, double percentage)
        {
            try
            {
                //percentage = percentage == null ? 0 : percentage;
                percentage = percentage > 100 ? 100 : percentage;
                percentage = percentage < 0 ? 0 : percentage;

                return valu * percentage/100.0;
            }
            catch(Exception ex)
            {
                log.LogInfoWithLevel("Incorrect setting. Error message:[" + ex.Message + "." , Log_LoggingLevel.Admin);
                log.LogInfoWithLevel("Percentage:" + percentage.ToString() + ",value to allocate:" + valu.ToString() + ".", Log_LoggingLevel.Admin);
                return 0;
            }
        }

        private double ReturnValueByValue(double valu, double tvalue)
        {
            try
            {
                //tvalue = tvalue == null ? 0 : tvalue;
                tvalue = tvalue >= valu ? valu : tvalue;
                tvalue = tvalue < 0 ? 0 : tvalue;

                return valu * 1.0;
            }
            catch(Exception ex)
            {
                log.LogInfoWithLevel("Incorrect setting. Error message:[" + ex.Message + "." , Log_LoggingLevel.Admin);
                log.LogInfoWithLevel("Target value:" + tvalue.ToString() + ",value to allocate:" + valu.ToString() + "." , Log_LoggingLevel.Admin);
                return 0;
            }
        }


        public List<object> GetDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            return dal.GetDetails(_parameter, paging, start, num, ref totalRecordCount);
        }

        public List<object> GetSummary(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            return dal.GetSummary(_parameter, paging, start, num, ref totalRecordCount);
        }


    }
}
