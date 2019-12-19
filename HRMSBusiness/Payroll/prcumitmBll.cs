using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.HRMS.HRMSData.Payroll;
using GotWell.Utility;
using GotWell.Model.Common;
using System.Transactions;
using GotWell.Model.HRMS;
using GotWell.Common;
using GotWell.HRMS.HRMSData.Personal;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prcumitmBll : BaseBll
    {
        prcumitmDal dal = null;
        public UtilLog log = new UtilLog();

        public prcumitmBll()
        {
            dal = new prcumitmDal();
            baseDal = dal;
        }

        public void InitLog()
        {
            string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
            string fileName = "CumulationCalc" + "_" + UtilDatetime.FormatDateTime5(DateTime.Now) + ".trc";

            //file header
            List<string> header = new List<string>();
            header.Add("Cumulation allocation log at " + UtilDatetime.FormatDateTime5(DateTime.Now) + " by " + Function.GetCurrentUser());
            header.Add("LOG");
            log.LogHeader(fileName, header);

            log = new UtilLog(fileName, Log_LoggingLevel.Admin);

        }

        public void Calculate(string rnno, string cond, List<ColumnInfo> personalParameters, string perd)
        {
            try
            {
                List<vw_employment> lstStaff = GetPersonals(personalParameters,cond);

                //目前只分配
                List<tprsalhi> lstSalaryHistory = GetSalaryHistory(rnno);

                log.LogInfoWithLevel("Start calculate salary to cumulation.", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Period:" + perd + "." , Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Run No:" + rnno + ".", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Total " + lstStaff.Count.ToString() + " staff(s) to calculate.", Log_LoggingLevel.Normal);

                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,new TimeSpan(1,0,0)))
                {
                    for (int i = 0; i < lstStaff.Count; i++)
                    {
                        log.LogInfoWithLevel("Start to calculate staff[" + lstStaff[i].sfid + "," + lstStaff[i].ntnm + "]." , Log_LoggingLevel.Normal);

                        //Delete first
                        List<ColumnInfo> runParameter = new List<ColumnInfo>() { 
                                new ColumnInfo() { ColumnName = "rnno", ColumnValue = rnno } ,
                                new ColumnInfo() { ColumnName = "emno", ColumnValue = lstStaff[i].emno } 
                        };
                        DoMultiDelete<tpraccalc>(runParameter);

                        List<tprsalhi> lstCurrentSalaryHistory = lstSalaryHistory.Where(p => p.emno == lstStaff[i].emno).ToList();

                        CalculateOneByOne(lstStaff[i], lstCurrentSalaryHistory);

                        log.LogInfoWithLevel("End to calculate staff[" + lstStaff[i].sfid + "," + lstStaff[i].ntnm + "]." , Log_LoggingLevel.Normal);
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

            List<tprsalhi> _lstSalaryHistory = dal.GetSalaryHistoryForCumulation(lstParameters);

            return _lstSalaryHistory;
        }


        public void CalculateOneByOne(vw_employment currentStaff,List<tprsalhi> lstCurrentSalaryHistory)
        {
            for (int i=0;i<lstCurrentSalaryHistory.Count;i++)
            {
                try
                {
                    log.LogInfoWithLevel("Salary Item[" + lstCurrentSalaryHistory[i].itcd + "]." , Log_LoggingLevel.Admin);

                    tprcumitm cumitm = new tprcumitm();
                    cumitm.amnt = lstCurrentSalaryHistory[i].valu;
                    cumitm.crcd = lstCurrentSalaryHistory[i].crcd;
                    cumitm.emno = lstCurrentSalaryHistory[i].emno;
                    cumitm.itcd = lstCurrentSalaryHistory[i].itcd;
                    cumitm.lmtm = DateTime.Now;
                    cumitm.lmur = Function.GetCurrentUser();
                    cumitm.perd = lstCurrentSalaryHistory[i].perd;
                    cumitm.rnno = lstCurrentSalaryHistory[i].rnno;
                    cumitm.sqno = GetNextSeq(lstCurrentSalaryHistory[i].emno, lstCurrentSalaryHistory[i].itcd);

                    DoInsert<tprcumitm>(cumitm);
                }
                catch (Exception ex)
                {
                    log.LogInfoWithLevel("Error found,ignore it. Error message:[" + ex.Message + "." , Log_LoggingLevel.Admin);
                }
            }
        }

        public int GetNextSeq(string emno, string itcd)
        {
            int sqno = 0;
            List<tprcumitm> lstCumItm = GetSelectedRecords<tprcumitm>(new List<ColumnInfo>()
            {
                new ColumnInfo(){ColumnName="emno",ColumnValue=emno},
                new ColumnInfo(){ColumnName="itcd",ColumnValue=itcd}
            });

            if (lstCumItm.Count > 0)
            {
                sqno = lstCumItm.Max(p => p.sqno) + 1;
            }
            else
            {
                sqno = 1;
            }

            return sqno;
        }
 
        public List<object> GetDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            return dal.GetDetails(_parameter, paging, start, num, ref totalRecordCount);
        }
    }
}
