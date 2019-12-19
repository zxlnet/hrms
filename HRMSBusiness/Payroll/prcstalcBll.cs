
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using GotWell.Utility;
using System.Transactions;
using GotWell.Common;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prcstalcBll : BaseBll
    {
        prcstalcDal dal = null;
        public UtilLog log = new UtilLog();

        public prcstalcBll()
        {
            dal = new prcstalcDal();
            baseDal = dal;
        }

        public void InitLog()
        {
            string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
            string fileName = "CostCenterAccountCalc" + "_" + UtilDatetime.FormatDateTime5(DateTime.Now) + ".trc";

            //file header
            List<string> header = new List<string>();
            header.Add("CostCenter allocation log at " + UtilDatetime.FormatDateTime5(DateTime.Now) + " by " + Function.GetCurrentUser());
            header.Add("LOG");
            log.LogHeader(fileName, header);

            log = new UtilLog(fileName, Log_LoggingLevel.Admin);

        }

        public void Calculate(string rnno, string cond, List<ColumnInfo> personalParameters, string perd)
        {
            try
            {
                InitLog();

                List<vw_employment> lstStaff = GetPersonals(personalParameters, cond);
                List<tprsalhi> lstSalaryHistory = GetSalaryHistory(rnno);
                List<tprccalcr> lstCCAllocateRule = GetAllocationRule();

                log.LogInfoWithLevel("Start allocate salary to cost center.", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Period:" + perd + "." , Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Run No:" + rnno + ".", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Total " + lstStaff.Count.ToString() + " staff(s) to allocate.", Log_LoggingLevel.Normal);

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(1, 0, 0)))
                {
                    for (int i = 0; i < lstStaff.Count; i++)
                    {
                        log.LogInfoWithLevel("Start to allocate staff[" + lstStaff[i].sfid + "," + lstStaff[i].ntnm + "]." , Log_LoggingLevel.Normal);

                        //Delete first
                        List<ColumnInfo> runParameter = new List<ColumnInfo>() { 
                                new ColumnInfo() { ColumnName = "rnno", ColumnValue = rnno } ,
                                new ColumnInfo() { ColumnName = "emno", ColumnValue = lstStaff[i].emno } 
                        };
                        DoMultiDelete<tprcstalc>(runParameter);

                        List<tprsalhi> lstCurrentSalaryHistory = lstSalaryHistory.Where(p => p.emno == lstStaff[i].emno).ToList();
                        List<tprccalcr> lstCurrentCCAllocateRule = lstCCAllocateRule.Where(p => p.emno == lstStaff[i].emno).ToList();

                        CalculateOneByOne(lstStaff[i], lstCurrentSalaryHistory, lstCCAllocateRule);

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

            List<tprsalhi> _lstSalaryHistory = dal.GetSalaryHistoryForCCAllocate(lstParameters);

            return _lstSalaryHistory;
        }

        public List<tprccalcr> GetAllocationRule()
        {
            prccalcrDal dal = new prccalcrDal();
            List<tprccalcr> _lstAllocationRule = dal.GetSelectedRecords<tprccalcr>(new List<ColumnInfo>() { });

            return _lstAllocationRule;
        }

        public void CalculateOneByOne(vw_employment currentStaff, List<tprsalhi> lstCurrentSalaryHistory, List<tprccalcr> lstCCAllocateRule)
        {
            for (int i=0;i<lstCurrentSalaryHistory.Count;i++)
            {
                try
                {
                    log.LogInfoWithLevel("Salary Item[" + lstCurrentSalaryHistory[i].itcd + "]." , Log_LoggingLevel.Admin);
                    List<tprccalcr> lstRule = lstCCAllocateRule.Where(p => p.itcd == lstCurrentSalaryHistory[i].itcd).ToList();
                    string vtyp = string.Empty;
                    double valu = 0;
                    string cccd = string.Empty;
                    double tmpSum = 0;
                    int sqno = 0;

                    for (int j = 0; j < lstRule.Count;j++ )
                    {
                        //有针对该itcd的分配规则
                        //如果定义了多套规则，则取第一套，其他忽略
                        vtyp = lstRule[j].vtyp;
                        valu = lstRule[j].valu.HasValue?lstRule[j].valu.Value:0;
                        cccd = lstRule[j].cccd;

                        log.LogInfoWithLevel("Rule definied at salary item [" + vtyp + "," + valu.ToString() + "," + cccd + "]." , Log_LoggingLevel.Admin);
                        sqno++;
                        tmpSum += HandleIt(currentStaff,lstCurrentSalaryHistory[i], vtyp, valu, cccd,sqno);

                    }

                    if (lstRule.Count <= 0)
                    {
                        //无针对该itcd的分配规则
                        lstRule = lstCCAllocateRule.Where(p => p.itcd == string.Empty).ToList();
                        for (int n = 0; n < lstRule.Count;n++ )
                        {
                            //有针对所有itcd定义的分配规则
                            vtyp = lstRule[n].vtyp;
                            valu = lstRule[n].valu.HasValue ? lstRule[n].valu.Value : 0;
                            cccd = lstRule[n].cccd;

                            log.LogInfoWithLevel("Rule definied at all salary item [" + vtyp + "," + valu.ToString() + "," + cccd + "]." , Log_LoggingLevel.Admin);

                            sqno++;
                            tmpSum += HandleIt(currentStaff, lstCurrentSalaryHistory[i], vtyp, valu, cccd,sqno);
                        }

                        if (lstRule.Count <= 0)
                        {
                            //无针对所有itcd定义的分配规则
                            if (currentStaff.cccd.Trim() != string.Empty)
                            {

                                //有默认账号,100%分配到该账号
                                vtyp = "Percentage";
                                valu = 100;
                                cccd = currentStaff.cccd;

                                log.LogInfoWithLevel("Allocate 100% to default cost center[" + currentStaff.cccd + "].", Log_LoggingLevel.Admin);

                                sqno++;
                                tmpSum += HandleIt(currentStaff, lstCurrentSalaryHistory[i], vtyp, valu, cccd,sqno);
                            }
                            else
                            {
                                log.LogInfoWithLevel("No any setting(s) found,do not allocate it." , Log_LoggingLevel.Admin);
                                //无默认账号，不分配
                                vtyp = string.Empty;
                                valu = 0;
                                cccd = string.Empty;
                            }
                        }
                    }

                    //余额分配到默认账号，如果没有定义，则不分配
                    if (tmpSum < lstCurrentSalaryHistory[i].valu)
                    {
                        if ((currentStaff.cccd == null) || (currentStaff.cccd.Trim() != string.Empty))
                        {
                            cccd = currentStaff.cccd;
                            log.LogInfoWithLevel("Left amount [" + (lstCurrentSalaryHistory[i].valu - tmpSum).ToString() + "] was allocate to default account[" + cccd + "].", Log_LoggingLevel.Admin);

                            sqno++;
                            HandleLeft(currentStaff, lstCurrentSalaryHistory[i], (lstCurrentSalaryHistory[i].valu - tmpSum), cccd,sqno);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.LogInfoWithLevel("Error found,ignore it. Error message:[" + ex.Message + "." , Log_LoggingLevel.Admin);
                }
            }
        }

        private void HandleLeft(vw_employment currentStaff, tprsalhi salHis, double valu, string cccd, int sqno)
        {
            if (cccd != string.Empty)
            {
                tprcstalc cstalc = new tprcstalc();
                cstalc.cccd = cccd;
                cstalc.amnt = valu;
                cstalc.emno = currentStaff.emno;
                cstalc.itcd = salHis.itcd;
                cstalc.perd = salHis.perd;
                cstalc.pkty = salHis.pkty;
                cstalc.rnno = salHis.rnno;
                cstalc.sqno = sqno;
                cstalc.crcd = salHis.crcd;
                cstalc.pdcd = salHis.pdcd;

                DoInsert<tprcstalc>(cstalc);
            }

        }

        private double HandleIt(vw_employment currentStaff, tprsalhi salHis,string vtyp,double valu,string cccd,int sqno)
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

            if (cccd != string.Empty)
            {
                tprcstalc cstalc = new tprcstalc();
                cstalc.cccd = cccd;
                cstalc.amnt = resultValue;
                cstalc.emno = currentStaff.emno;
                cstalc.itcd = salHis.itcd;
                cstalc.perd = salHis.perd;
                cstalc.pkty = salHis.pkty;
                cstalc.rnno = salHis.rnno;
                cstalc.sqno = sqno;
                cstalc.crcd = salHis.crcd;
                cstalc.sqno = salHis.sqno;
                cstalc.pdcd = salHis.pdcd;

                DoInsert<tprcstalc>(cstalc);
            }

            return resultValue;
        }

        private double ReturnValueByPercentage(double valu, double percentage)
        {
            try
            {
                percentage = percentage == null ? 0 : percentage;
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
                tvalue = tvalue == null ? 0 : tvalue;
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
