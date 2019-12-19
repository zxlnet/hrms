using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using System.Transactions;
using GotWell.Model.HRMS;
using GotWell.Utility;
using Newtonsoft.Json;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Payroll.Variables;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSData.Personal;
using GotWell.HRMS.HRMSData.Overtime;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.HRMS.HRMSData.Payroll;
using GotWell.HRMS.HRMSData.Syst;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.HRMS.HRMSData.Common;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class PrCalculationStore
    {
        public List<vw_employment> lstStaffs { get; set; }
        public List<totdetail> lstOTDetails { get; set; }
        public List<tlvleaapd> lstLeaveDetails { get; set; }
        public List<tprfmular> lstFormula { get; set; }
        public List<tprfmudtl> lstFormulaDetails { get; set; }
        public List<tstcondtn> lstSystemConditions { get; set; }
        public List<tatabsdtl> lstAbsenceDetails { get; set; }
        public List<totlimit> lstOTLimit { get; set; }
        public List<tlvlealmt> lstLeaveLimit { get; set; }
        public List<tprprsdtl> lstPubRuleSet { get; set; }
        public List<tprprirst> lstPrivateRuleSet { get; set; }
        public List<tpraddrst> lstAdditionalRuleSet { get; set; }
        public List<tprvarble> lstVariables { get; set; }
        public List<tprpbrhi> lstStaffPubRuleSetHis { get; set; }
        public List<tprrstsit> lstRelItem { get; set; }
        public List<tprsalhi> lstSalaryResult { get; set; }
        public List<tprsalitm> lstSalaryItem { get; set; }

        public vw_employment CurrentEmployment { get; set; }
        public tprfmular CurrentFormula { get; set; }
        public tprfmudtl CurrentFormulaDtl { get; set; }
        public string CurrentExpressionLeft { get; set; }
        public string CurrentExpressionRight { get; set; }
        public string CurrentExpressionOP { get; set; }
        public List<object> ParametersForValue { get; set; }
        public List<string> ExpressionKeeper { get; set; }
        public bool ExpressionKeeperFlag { get; set; }

        public double TotalAmount { get; set; }
        public double StdWorkHours { get; set; }
        public string PayDay { get; set; }

        public tstperiod CurrentPeriod;
        public DateTime PeriodStart;
        public DateTime PeriodEnd;

        public tbscompny CurrentCompany;
        public tprrun CurrentRun;

        public string SqlStaff;

        public PrCalculationStore()
        {
            lstSalaryResult = new List<tprsalhi>();
            ParametersForValue = new List<object>();
            ExpressionKeeper = new List<string>();
            ExpressionKeeperFlag=false;
        }
    }

    public class RuleSetItem
    {
        //sqno,itcd,efdt,exdt.valu,vtyp,rlcd,crcd,isca,pdcd,cred
        public int sqno { get; set; }
        public string itcd { get; set; }
        public DateTime efdt { get; set; }
        public DateTime? exdt { get; set; }
        public string valu { get; set; }
        public string vtyp { get; set; }
        public string rlcd { get; set; }
        public string crcd { get; set; }
        public string isca { get; set; }
        public string pdcd { get; set; }
        public string cred { get; set; }
        public string rscd { get; set; }
    }

    public class prcalculBll : BaseBll
    {
        public string DefinedStartChar="{";
        public string DefinedEndChar = "}";
        public List<string> FuncDictionary;
        public UtilLog log = new UtilLog();

        public prcalculBll()
        {
            InitFuncDict();
            InitLog();
        }

        public void InitLog()
        {
            string pathName = AppDomain.CurrentDomain.BaseDirectory + @"\Log\";
            string fileName = "PayrollCalc" + "_" + UtilDatetime.FormatDateTime5(DateTime.Now) + ".trc";

            //file header
            List<string> header = new List<string>();
            header.Add("Payroll calculation log at " + UtilDatetime.FormatDateTime5(DateTime.Now) + " by " + Function.GetCurrentUser());
            header.Add("LOG");
            log.LogHeader(fileName,header);

            log = new UtilLog(fileName,Log_LoggingLevel.Admin);

        }

        public void InitFuncDict()
        {
            FuncDictionary = new List<string>();
            FuncDictionary.Add("round");
            FuncDictionary.Add("max");
            FuncDictionary.Add("min");
            FuncDictionary.Add("abs");
            FuncDictionary.Add("ceiling");
            FuncDictionary.Add("floor");
            FuncDictionary.Add("truncate");

        }

        public void Calculate(string _type, string _cond, List<ColumnInfo> _personalParameters, string _perd, string _icnj, 
                                        string _ictm,string _pdcd)
        {
            PrCalculationStore store = new PrCalculationStore();

            store.CurrentPeriod = GetSelectedObject<tstperiod>(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "perd", ColumnValue = _perd } });

            store.PeriodStart = store.CurrentPeriod.pest;
            store.PeriodEnd = store.CurrentPeriod.peen;

            store.CurrentRun = CreateRun(store, _type);

            log.LogInfoWithLevel("", Log_LoggingLevel.Normal);           

            log.LogInfoWithLevel("Start payroll calculation.", Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("Run no:" + store.CurrentRun.rnno + ".", Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("Type:" + (_type == "A" ? "Actual" : "Simulation"), Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("Condition 1:" + _cond, Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("Condition 2:" + _personalParameters, Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("Period:" + _perd, Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("Include new join:" + _icnj=="Y"?"Yes":"No", Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("Include terminated:" + _ictm == "Y" ? "Yes" : "No", Log_LoggingLevel.Normal);
            

            //store.PeriodStart = new DateTime(2003,3,1);
            //store.PeriodEnd = (new DateTime(2003, 3, 31)).AddDays(1);

            log.LogInfoWithLevel("Period start date:" + UtilDatetime.FormatDate1(store.PeriodStart) , Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("Period end date:" + UtilDatetime.FormatDate1(store.PeriodEnd), Log_LoggingLevel.Normal);

            log.LogInfoWithLevel("Start to retrieve data.", Log_LoggingLevel.Normal);
            store.PayDay = _pdcd.Trim();
            GetData(store, _personalParameters, _cond, _icnj, _ictm);
            log.LogInfoWithLevel("End to retrieve data.", Log_LoggingLevel.Normal);

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(10,0,0)))
            {
                log.LogInfoWithLevel("Total " + store.lstStaffs.Count + " staff(s) to calculate.", Log_LoggingLevel.Normal);

                for (int i = 0; i < store.lstStaffs.Count; i++)
                {
                    store.CurrentEmployment = store.lstStaffs[i];

                    log.LogInfoWithLevel(" ", Log_LoggingLevel.Normal);
                    log.LogInfoWithLevel("Calculate start for [Employment no:" + store.CurrentEmployment.sfid + ",Staff id:" + store.CurrentEmployment.sfid + ",Staff name:" + store.CurrentEmployment.ntnm +"]", Log_LoggingLevel.Normal);
                    CalculateOneByOne(store);
                    log.LogInfoWithLevel("Calculate end.", Log_LoggingLevel.Normal);
                }

                store.CurrentRun.tamt = store.TotalAmount;
                store.CurrentRun.tsct = store.lstStaffs.Count;
                store.CurrentRun.pdcd = store.PayDay;

                DoInsert<tprrun>(store.CurrentRun);

                scope.Complete();
            }
        }

        private tprrun CreateRun(PrCalculationStore store,string runType)
        {
            tprrun run = new tprrun();
            run.perd = store.CurrentPeriod.perd;
            run.rndt = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            run.rnno = new prrunDal().GetNewRun(run.rndt.Value);

            switch (runType)
            {
                case "A":
                    run.rnty = "Actual";
                    break;
                case "S":
                    run.rnty = "Simulation";
                    break;
            }

            run.lmtm = DateTime.Now;
            run.lmur = Function.GetCurrentUser();

            return run;
        }

        public void GetData(PrCalculationStore store, List<ColumnInfo> _personalParameters,string _cond, 
                                        string _icnj, string _ictm)
        {

            store.lstSystemConditions = GetSystemCondition();

            store.lstStaffs = GetPersonals(_personalParameters, ref store.SqlStaff, store.PeriodStart, _cond, _icnj, _ictm);
            store.lstOTDetails = GetOTDetails(store);
            store.lstLeaveDetails = GetLeaveDetails(store);
            store.lstFormula = GetFormula();
            store.lstFormulaDetails = GetFormulaDetail();
            store.lstVariables = GetVariables();
            store.lstPubRuleSet = GetPublicRuleSet(store);
            store.lstPrivateRuleSet = GetPrivateRuleSet(store);
            store.lstAdditionalRuleSet = GetAdditionalRuleSet(store);
            store.lstAbsenceDetails = GetAbsenceDetails(store);
            store.lstStaffPubRuleSetHis = GetStaffPubRuleSetHis(store);
            store.lstRelItem = GetRelItem(store);
            store.lstSalaryItem = GetSalaryItem(store);

            //如果取得Limit数据,only month limit
            store.lstLeaveLimit = GetLeaveLimit(store);
            store.lstOTLimit = GetOvertimeLimit(store);

            store.CurrentCompany = GetCompany(store);

        }

        #region 取数据
        public tbscompny GetCompany(PrCalculationStore store)
        {
            BaseBll bll = new BaseBll();
            List<tbscompny> lstCompany = bll.GetSelectedRecords<tbscompny>(new List<ColumnInfo>() { });
            if (lstCompany.Count > 0)
            {
                return lstCompany[0];
            }
            return null;
        }

        public void PrepareCalculation(PrCalculationStore store)
        {
            #region standard work hours per days
            double stdWorkHours = 0;
            try
            {
                var q = (from p in store.lstVariables
                         where p.vacd == "usrStdWorksHours"
                         select p.vlva).ToList();

                if (q.Count < 0)
                    stdWorkHours = Convert.ToDouble(((StSystemConfig)Parameter.CURRENT_SYSTEM_CONFIG).PrSWHPD);

                for (int i = 0; i < q.Count; i++)
                {
                    stdWorkHours = Double.Parse(q[i]);
                }
            }
            catch(Exception ex)
            {
                stdWorkHours = Convert.ToDouble(((StSystemConfig)Parameter.CURRENT_SYSTEM_CONFIG).PrSWHPD);
            }

            store.StdWorkHours = stdWorkHours;
            #endregion

        }

        public List<tlvlealmt> GetLeaveLimit(PrCalculationStore store)
        {
            return null;
        }

        public List<totlimit> GetOvertimeLimit(PrCalculationStore store)
        {
            return null;
        }

        public List<totdetail> GetOTDetails(PrCalculationStore store)
        {
            otdetailDal dal = new otdetailDal();
            return dal.GetOTDetailForPayroll(store.SqlStaff, store.PeriodStart, store.PeriodEnd);
        }

        public List<tlvleaapd> GetLeaveDetails(PrCalculationStore store)
        {
            lvleaappDal dal = new lvleaappDal();
            return dal.GetLeaveDetailForPayroll(store.SqlStaff, store.PeriodStart, store.PeriodEnd);
        }

        public List<tprfmular> GetFormula()
        {
            BaseBll dal = new BaseBll();
            return dal.GetSelectedRecords<tprfmular>(new List<ColumnInfo>() { });
        }

        public List<tprfmudtl> GetFormulaDetail()
        {
            BaseBll dal = new BaseBll();
            return dal.GetSelectedRecords<tprfmudtl>(new List<ColumnInfo>() { });
        }

        public List<tstcondtn> GetSystemCondition()
        {
            BaseBll dal = new BaseBll();
            return dal.GetSelectedRecords<tstcondtn>(new List<ColumnInfo>() { });
        }

        public List<tprvarble> GetVariables()
        {
            BaseBll dal = new BaseBll();
            return dal.GetSelectedRecords<tprvarble>(new List<ColumnInfo>() { });
        }

        public List<tprprsdtl> GetPublicRuleSet(PrCalculationStore store)
        {
            prpubrstDal dal = new prpubrstDal();
            return dal.GetPubRuleSetDtlForPayroll(store.PeriodStart, store.PeriodEnd,store.PayDay);
        }

        public List<tprprirst> GetPrivateRuleSet(PrCalculationStore store)
        {
            prprirstDal dal = new prprirstDal();
            return dal.GetPrivateRuleSetDtlForPayroll(store.SqlStaff,store.PeriodStart, store.PeriodEnd,store.PayDay);
        }

        public List<tpraddrst> GetAdditionalRuleSet(PrCalculationStore store)
        {
            praddrstDal dal = new praddrstDal();
            return dal.GetAdditionalRuleSetDtlForPayroll(store.SqlStaff,store.PeriodStart, store.PeriodEnd,store.PayDay);
        }

        public List<tatabsdtl> GetAbsenceDetails(PrCalculationStore store)
        {
            atabsdtlDal dal = new atabsdtlDal();
            return dal.GetLeaveDetailForPayroll(store.SqlStaff, store.PeriodStart, store.PeriodEnd);
        }

        public List<tprpbrhi> GetStaffPubRuleSetHis(PrCalculationStore store)
        {
            prpbrhisDal dal = new prpbrhisDal();
            return dal.GetStaffPubRuleSetHis(store.SqlStaff, store.PeriodStart, store.PeriodEnd);
        }

        public List<tprrstsit> GetRelItem(PrCalculationStore store)
        {
            BaseDal dal = new BaseDal();
            return dal.GetSelectedRecords<tprrstsit>(new List<ColumnInfo>() { });
        }

        public List<tprsalitm> GetSalaryItem(PrCalculationStore store)
        {
            BaseDal dal = new BaseDal();
            return dal.GetSelectedRecords<tprsalitm>(new List<ColumnInfo>() { });
        }


        public List<vw_employment> GetPersonals(List<ColumnInfo> _personalParameters, ref string _sqlStaff, DateTime _analStartDate,string _cond, string _icnj, string _ictm)
        {
            psemplymDal dal = new psemplymDal();
            //List<vw_employment> _lstStaff = dal.GetHiringEmployment(_personalParameters, _analStartDate, _cond,  _icnj,  _ictm);
            List<vw_employment> _lstStaff = dal.GetHiringEmployment(_personalParameters, _analStartDate);
            _sqlStaff = dal.AnalyzeQueryCriterias(_personalParameters, _analStartDate);
            if (_sqlStaff.Trim() == "")
                _sqlStaff = "(1=1)";

            return _lstStaff;
        }
        #endregion

        public void CalculateOneByOne(PrCalculationStore store)
        {
            //One staff by one staff calculation
            try
            {
                #region 处理 Additional rule set
                List<tpraddrst> lstCurrentAdditionalRuleSet = store.lstAdditionalRuleSet.Where(p => p.emno == store.CurrentEmployment.emno).ToList();
                List<RuleSetItem> lstRuleSetItem = (from p in lstCurrentAdditionalRuleSet
                                                    orderby p.sqno
                                                    select new RuleSetItem
                                                    {
                                                        sqno = p.sqno,
                                                        itcd = p.itcd,
                                                        efdt = DateTime.Now,
                                                        exdt = DateTime.Now,
                                                        valu = (p.valu.HasValue?p.valu.Value:0).ToString(),
                                                        vtyp = HRMS_SalaryItem_ValueType.Value.ToString(),
                                                        rlcd = p.rlcd,
                                                        crcd = p.crcd,
                                                        isca = p.isca,
                                                        pdcd = p.pdcd,
                                                        cred = "",
                                                        rscd = ""
                                                    }).ToList();

                log.LogInfoWithLevel("Total " + lstRuleSetItem.Count.ToString() + " additional rule set item found.", Log_LoggingLevel.Normal);

                for (int i = 0; i < lstRuleSetItem.Count; i++)
                {
                    CalculateRuleSetItem(store, lstRuleSetItem[i], "Additional");
                }

                log.LogInfoWithLevel("End of calculate additional rule set.", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel(" ", Log_LoggingLevel.Normal);
                #endregion

                #region 处理private rule set
                List<tprprirst> lstCurrentPrivateRuleSet = store.lstPrivateRuleSet.Where(p => p.emno == store.CurrentEmployment.emno).ToList();
                lstRuleSetItem = (from p in lstCurrentPrivateRuleSet
                                                    orderby p.sqno
                                                    select new RuleSetItem
                                                    {
                                                        sqno = p.sqno,
                                                        itcd = p.itcd,
                                                        efdt = p.efdt,
                                                        exdt = p.exdt,
                                                        valu = p.valu,
                                                        vtyp = p.vtyp,
                                                        rlcd = p.rlcd,
                                                        crcd = p.crcd,
                                                        isca = p.isca,
                                                        pdcd = p.pdcd,
                                                        cred = p.cred,
                                                        rscd = store.CurrentEmployment.emno
                                                    }).ToList();

                log.LogInfoWithLevel("Total " + lstRuleSetItem.Count.ToString() + " private rule set item found.", Log_LoggingLevel.Normal);

                for (int i = 0; i < lstRuleSetItem.Count; i++)
                {
                    CalculateRuleSetItem(store, lstRuleSetItem[i], "Private");
                }

                log.LogInfoWithLevel("End of calculate private rule set.", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel(" ", Log_LoggingLevel.Normal);
                #endregion

                #region 处理public rule set
                List<tprpbrhi> lstCurrentStaffPubRuleSet = store.lstStaffPubRuleSetHis.Where(p => p.emno == store.CurrentEmployment.emno).ToList();
                if (lstCurrentStaffPubRuleSet.Count > 0)
                {
                    List<tprprsdtl> lstCurrentPublicRuleSet = store.lstPubRuleSet.Where(p => p.rscd == lstCurrentStaffPubRuleSet.First().rscd).ToList();

                    //处理public rule set
                    lstRuleSetItem = (from p in lstCurrentPublicRuleSet
                                      orderby p.sqno
                                      select new RuleSetItem
                                      {
                                          sqno = p.sqno,
                                          itcd = p.itcd,
                                          efdt = p.efdt,
                                          exdt = p.exdt,
                                          valu = p.valu,
                                          vtyp = p.vtyp,
                                          rlcd = p.rlcd,
                                          crcd = p.crcd,
                                          isca = p.isca,
                                          pdcd = p.pdcd,
                                          cred = p.cred,
                                          rscd = p.rscd
                                      }).ToList();

                    log.LogInfoWithLevel("Total " + lstRuleSetItem.Count.ToString() + " public rule set item found.", Log_LoggingLevel.Normal);

                    for (int i = 0; i < lstRuleSetItem.Count; i++)
                    {
                        CalculateRuleSetItem(store, lstRuleSetItem[i], "Public");
                    }

                }
                else
                {
                    log.LogInfoWithLevel("No public rule set assigned to the staff.", Log_LoggingLevel.Normal);
                }

                log.LogInfoWithLevel("End of calculate public rule set.", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel(" ", Log_LoggingLevel.Normal);

                #endregion
            }
            catch (UtilException ex)
            {
                log.LogInfoWithLevel("Error found,error message:[" + ex.Message + "]", Log_LoggingLevel.Normal);

            	throw ex;
            }
            catch(Exception ex)
            {
                log.LogInfoWithLevel("Error found,error message:[" + ex.Message + "]", Log_LoggingLevel.Normal);
                
                throw new UtilException(ex.Message, ex);
            }
        }

        private void CalculateRuleSetItem(PrCalculationStore store, RuleSetItem rsItem,string pkgType)
        {
            double itemValue = 0;
            HRMS_SalaryItem_ValueType vtyp = (HRMS_SalaryItem_ValueType)Enum.Parse(typeof(HRMS_SalaryItem_ValueType),rsItem.vtyp);

            log.LogInfoWithLevel("Calculate start for item code:[" + rsItem.itcd + "(" + pkgType + ")]", Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("The item is a " + vtyp.ToString() + " item.", Log_LoggingLevel.Normal);

            switch (vtyp)
            {
                case HRMS_SalaryItem_ValueType.Value:
                    double.TryParse(rsItem.valu, out itemValue);
                    break;
                case HRMS_SalaryItem_ValueType.Formula:
                    itemValue = CalculateFormulaValue(store, rsItem);
                    break;
                case HRMS_SalaryItem_ValueType.Sum:
                    itemValue = CalculateSumValue(store, rsItem);
                    break;
                case HRMS_SalaryItem_ValueType.Customization:
                    itemValue = CallCustomizationMethod(rsItem.valu);
                    break;
            }

            KeepIn(store, itemValue, rsItem, pkgType);
            log.LogInfoWithLevel("Save result to database.", Log_LoggingLevel.Normal);

            log.LogInfoWithLevel("Calculate end for item code:[" + rsItem.itcd + "(" + pkgType + ")]", Log_LoggingLevel.Normal);
            log.LogInfoWithLevel(" ", Log_LoggingLevel.Normal);

        }

        private void KeepIn(PrCalculationStore store,double valu, RuleSetItem rsItem,string pkgType)
        {
            //save to history
            tprsalhi his = new tprsalhi();
            his.ajva = null;
            //his.efdt = store.PeriodEnd;
            his.itcd = rsItem.itcd.Trim();
            his.lmtm = DateTime.Now;
            his.lmur = Function.GetCurrentUser();
            his.padt = store.PeriodEnd;//此处需要改成实际的
            his.pkty = pkgType;
            //his.rlcd = rsItem.rlcd;
            his.rscd = rsItem.rscd;
            his.rnno = store.CurrentRun.rnno;
            his.sqno = rsItem.sqno;
            his.valu = valu;
            his.emno = store.CurrentEmployment.emno;
            his.perd = store.CurrentPeriod.perd;
            his.crcd = rsItem.crcd;
            his.pdcd = store.PayDay;
            his.isca = rsItem.isca == string.Empty ? "Y" : rsItem.isca;

            if (his.crcd == null)
            {
                //如果货币单位为空，则取默认货币单位
                if (store.CurrentCompany == null)
                    his.crcd = "N/A";
                else
                    his.crcd = store.CurrentCompany.dfcu;
            }

            store.lstSalaryResult.Add(his);

            //TotalAmount不知道统计哪个Item
            //store.TotalAmount += his.valu;

            //Save to database
            DoInsert<tprsalhi>(his);
        }

        public double GetEffectDaysInPeriod(PrCalculationStore store, RuleSetItem rsItem)
        {
            //计算项目有效天数
            DateTime tmpStartDate;
            DateTime tmpEndTime;
            double effDays = 0;
            if (rsItem.efdt > store.PeriodStart)
                tmpStartDate = rsItem.efdt;
            else
                tmpStartDate = store.PeriodStart;

            if (rsItem.exdt > store.PeriodEnd)
                tmpEndTime = store.PeriodEnd;
            else
                tmpEndTime = rsItem.exdt.HasValue ? rsItem.exdt.Value.AddDays(-1) : new DateTime(2050, 1, 1);

            if (tmpStartDate == null)
                effDays = 0;
            else
            {
                if (tmpEndTime == null)
                    tmpEndTime = store.PeriodEnd;

                effDays = (tmpEndTime - tmpStartDate).TotalDays;
            }

            return effDays;
        }

        public double CallCustomizationMethod(string className)
        {
            if (className.Trim().Equals(string.Empty)) return 0;

            try
            {
                log.LogInfoWithLevel("Start to call customization method[" + className +"]", Log_LoggingLevel.Normal);
                
                object obj = Type.GetType(className).GetConstructor(new Type[] { }).Invoke(new object[] { });
                double returnValue = (double)Type.GetType(className).GetMethod("GetValue").Invoke(obj, new object[] { });

                log.LogInfoWithLevel("Return value:" + returnValue.ToString() + ".", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("End to call customization method", Log_LoggingLevel.Normal);
                return returnValue;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public double CalculateFormulaValue(PrCalculationStore store, RuleSetItem rsItem)
        {
            tprfmular formula = store.lstFormula.Where(p => p.frcd.Trim() == rsItem.valu).Single();
            List<tprfmudtl> lstFormulaDtl = store.lstFormulaDetails.Where(p => p.frcd.Trim() == rsItem.valu).ToList();
            double valu = 0;
            double cnt  =0;

            log.LogInfoWithLevel("Start to calculate formula[" + formula.frcd + "," + formula.frnm +"]", Log_LoggingLevel.Normal);

            store.CurrentFormula = formula;

            for (int i=0;i<lstFormulaDtl.Count;i++)
            {
                store.ParametersForValue = new List<object>();
                double v = 0;

                store.CurrentFormulaDtl = lstFormulaDtl[i];

                bool r = CheckFormularCondition(store,lstFormulaDtl[i].cdtl);
                if (r)
                {
                    log.LogInfoWithLevel("Formula condition checking return TRUE.", Log_LoggingLevel.Normal);
                    v = (double)GetFormularItemValue(store, lstFormulaDtl[i].valu);
                }
                else
                {
                    log.LogInfoWithLevel("Formula condition checking return FALSE.", Log_LoggingLevel.Normal);
                    v = 0;
                }

                //需要判断Formula DTL的统计方法
                switch (formula.chfn)
                {
                    case "Sum":
                        valu += v;
                        break;
                    case "Count":
                        if (r)
                            //只有条件成立才计数
                            valu++;
                        break;
                    case "Max":
                        valu = Math.Max(valu, v);
                        break;
                    case "Min":
                        valu = Math.Min(valu, v);
                        break;
                    case "Avg":
                        valu += v;
                        if (r)
                            //只有条件成立才求平均值    
                            cnt++;
                        break;
                    default:
                        if (r)  ///如果多个条件满足,又没有定义child function的话，则取最后一个满足条件的值
                            valu = v;
                        break;
                }
            }

            if (formula.chfn == "Avg") valu = cnt == 0 ? 0 : valu / cnt;

            log.LogInfoWithLevel("Return value:" + valu.ToString(), Log_LoggingLevel.Normal);
            log.LogInfoWithLevel("End to calculate formula", Log_LoggingLevel.Normal);

            if (valu == -55)
            {
                string x = "aaaa";
            }
            return valu;
        }

        public double CalculateSumValue(PrCalculationStore store, RuleSetItem rsItem)
        {
            List<tprrstsit> lstRel = store.lstRelItem.Where(p => p.rlcd == rsItem.rlcd).ToList();
            double sumValue = 0;

            for (int i = 0; i < lstRel.Count; i++)
            {
                double v = 0;
                try
                {
                    List<tprsalhi> q = (from p in store.lstSalaryResult
                                        where p.itcd == lstRel[i].itcd
                                        select p).ToList();

                    tprsalitm item = (from p in store.lstSalaryItem
                                      where p.itcd == lstRel[i].itcd
                                      select p).Single();

                    if (q.Count > 0 && item != null)
                    {
                        //判断加减项
                        v = Convert.ToDouble(q.Single().valu) * (item.opty == "A" ? 1 : (item.opty == "D" ? -1 : 0));
                        log.LogInfoWithLevel("[" + item.itcd + "]" + item.itnm + "  ==>Value:" + q.Single().valu.ToString(), Log_LoggingLevel.Normal);
                    }
                    else
                    {
                        v = 0;
                        log.LogInfoWithLevel("[" + item.itcd + "] is unknown or uncalculation.  ==>Value:= 0", Log_LoggingLevel.Normal);
                    }
                }
                catch (Exception ex)
                {
                    v = 0;
                }
                sumValue += v;
            }

            return sumValue;
        }

        public bool CheckFormularCondition(PrCalculationStore store,string cond)
        {
            //判断公式条件是否成立
            try
            {
                if (cond.Trim().Equals(string.Empty))
                    return false;

                string actValue = ReplaceConditionWithActualValue(store,cond);

                log.LogInfoWithLevel("Formula condition original[" + cond + "].", Log_LoggingLevel.Normal);
                log.LogInfoWithLevel("Formula condition parsed[" + actValue + "].", Log_LoggingLevel.Normal);

                return (bool)UtilEvaluator.EvaluateToBool(actValue);
            }
            catch (UtilException ex)
            {
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public string ReplaceConditionWithActualValue(PrCalculationStore store,string cond)
        {
            try
            {
                //把条件替换成实际的值
                //先抓去cond中的变量
                string result = string.Empty;
                string tmpleft = string.Empty;
                string tmpright = string.Empty;
                //cond = "((( {#Pro_Basic} >= ({$usrPro_Basic} * 0.6)) and ( {#Pro_Basic} <  {$usrPro_Basic} * 3)) and ({@sfid} like \"%00004%\") and ( {#Pro_Basic} <>0) and ({@sfid} like \"%00004%\") and ({@sfid} not like \"%00004%\"))";
                List<string> lstExpression = RetrieveExpressions(store,ref cond);

                for (int j = 0; j < lstExpression.Count; j++)
                {
                    if ((lstExpression[j].Trim().ToLower() != "and") && (lstExpression[j].Trim().ToLower() != "or") 
                        && (lstExpression[j].Trim().ToLower() != "(") && (lstExpression[j].Trim().ToLower() != ")")
                        && (lstExpression[j].Trim().ToLower() != "") && (lstExpression[j].Trim().ToLower() != ",")
                        && (!FuncDictionary.Contains(lstExpression[j].Trim().ToLower())))
                    {
                        string left = "";
                        string right = "";
                        string op = "";

                        ParseExpression(lstExpression[j], ref left, ref right, ref op);

                        store.CurrentExpressionLeft = left;
                        store.CurrentExpressionOP = op;
                        store.CurrentExpressionRight = right;

                        List<string> lstVariables = RetrieveVariables(left);
                        tmpleft = left;
                        for (int i = 0; i < lstVariables.Count; i++)
                        {
                            object valu = ParseVariable(store, lstVariables[i]);
                            tmpleft = tmpleft.Replace(DefinedStartChar + lstVariables[i] + DefinedEndChar, FormatValue(valu));
                        }

                        lstVariables = RetrieveVariables(right);
                        tmpright = right;
                        for (int i = 0; i < lstVariables.Count; i++)
                        {
                            object valu = ParseVariable(store, lstVariables[i]);
                            tmpright = tmpright.Replace(DefinedStartChar + lstVariables[i] + DefinedEndChar, FormatValue(valu));
                        }

                        result += StandardExpression(tmpleft, op, tmpright);
                    }
                    else
                    {
                        if (lstExpression[j].Trim().ToLower() == "and")
                            result += " && ";

                        if (lstExpression[j].Trim().ToLower() == "or")
                            result += " || ";

                        if (lstExpression[j].Trim().ToLower() == "(")
                            result += " ( ";

                        if (lstExpression[j].Trim().ToLower() == ")")
                            result += " ) ";

                        if (lstExpression[j].Trim().ToLower() == ",")
                            result += " , ";

                        if (FuncDictionary.Contains(lstExpression[j].Trim().ToLower()))
                        {
                            var s = lstExpression[j].Trim().ToLower();

                            result += "Math." + s.Substring(0, 1).ToUpper() + s.Substring(1, s.Length - 1) ;
                        }
                    }
                }

                return result;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
        }

        public string StandardExpression(string left, string op, string right)
        {
            string result = string.Empty;
            //转换表达式成标准表达式
            switch (op.Trim())
            {
                case "<>":
                    result = " (" + left + " " + "!=" + " " + right + ") ";
                    break;
                case "=":
                    result = " (" + left + " " + "==" + " " + right + ") ";
                    break;
                case "in":
                    result = ConvertOPIn(left, op, right, false);
                    break;
                case "not in":
                    result = ConvertOPIn(left, op, right, true);
                    break;
                case "like":
                    result = " ((" + left + ") " + ".Contains(" + right + ")) ";
                    break;
                case "not like":
                    result = " (!(" + left + ") " + ".Contains(" + right + ")) ";
                    break;
                default:
                    result = " " + left + " " + op + " " + right + " ";
                    break;
            }

            return " " + result + " ";
        }

        public string ConvertOPIn(string left, string op, string right,bool isNot)
        {
            string result = string.Empty;
            //aaa in ('a','(b)','c');

            //拿掉两边的括号
            right = right.Trim();
            if (right.Substring(0, 1) == "(")
                right = right.Substring(1, right.Length - 1);

            if (right.Substring(right.Length - 1, 1) == ")")
                right = right.Substring(0, right.Length - 1);

            string[] arr = right.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                if (isNot)
                    result += (result == "" ? "" : " && ") + "(" + left + " != " + arr[i] + ")";
                else
                    result += (result == "" ? "" : " || ") + "(" + left + " != " + arr[i] + ")";
            }

            return result;
        }

        public List<string> RetrieveVariables(string cond)
        {
            List<string> lstVariables = new List<string>();
            int startPos = -1;
            int endPos = -1;
            for (int i = 0; i < cond.Length; i++)
            {
                if (cond.Substring(i, 1) == DefinedStartChar)
                {
                    startPos = i;
                }

                if (cond.Substring(i, 1) == DefinedEndChar)
                {
                    endPos = i;

                    if (startPos != -1)
                    {
                        lstVariables.Add(cond.Substring(startPos + 1, endPos - startPos-1));
                        startPos = -1;
                        endPos = -1;
                    }
                }
            }

            return lstVariables;
        }

        public List<string> RetrieveExpressions(PrCalculationStore store,ref string cond)
        {
            try
            {
                //cond = "{aaa} = 'aaa' and 1 and 'bbb' >= {bbb} or {cccc}>=2 and {ddd} like 'dd' and {eee} not like 'ee' and {fff} not in 'fff' and {hhh} in 'ff' and 'a and b' ='c or d'";
                //cond = "((( {#Pro_Basic} >= ({$usrPro_Basic} * 0.6)) and ( {#Pro_Basic} <  {$usrPro_Basic} * 3)) and ({@sfid} like \"%00004%\") and ( {#Pro_Basic} <>0) and ({@sfid} like \"%00004%\") and ({@sfid} not like \"%00004%\"))";
                List<string> lstExpression = new List<string>();
                bool inQuotation = false;
                string tmp = string.Empty;

                cond = CleanExpression(cond);

                char[] arrChar = cond.ToCharArray();

                for (int i = 0; i < arrChar.Length; i++)
                {
                    if ((arrChar[i] == '\'') && (!inQuotation))
                    {
                        inQuotation = true;
                        tmp += arrChar[i].ToString();
                    }

                    if ((arrChar[i] == '\'') && (inQuotation))
                    {
                        inQuotation = false;
                        lstExpression.Add(tmp);
                        tmp = "";
                    }

                    if ((arrChar[i] == ',') && (!inQuotation))
                    {
                        lstExpression.Add(tmp);
                        tmp = "";
                        lstExpression.Add(",");
                    }

                    if ((arrChar[i] == '(') && (!inQuotation))
                    {
                        lstExpression.Add(tmp);
                        tmp = "";
                        lstExpression.Add("(");
                    }

                    if ((arrChar[i] == ')') && (!inQuotation))
                    {
                        lstExpression.Add(tmp);
                        tmp = "";
                        lstExpression.Add(")");
                    }

                    if ((arrChar[i] == ' ') && (!inQuotation) )
                    {
                        if (tmp.Trim().Length >= 3)
                        {
                            if ((tmp.Trim().ToLower().Substring(tmp.Trim().Length - 3, 3) == "and") || ((tmp.Trim().ToLower().Substring(tmp.Trim().Length - 2, 2) == "or")))
                            {
                                if (tmp.Trim().ToLower().Substring(tmp.Trim().Length - 3, 3) == "and")
                                {
                                    lstExpression.Add(tmp.Trim().Substring(0, tmp.Trim().Length - 3));
                                    lstExpression.Add("and");
                                }
                                else
                                {
                                    lstExpression.Add(tmp.Trim().Substring(0, tmp.Trim().Length - 2));
                                    lstExpression.Add("or");
                                }

                                tmp = "";
                            }
                        }
                    }

                    if ((arrChar[i] != ')') && (arrChar[i] != '(') && (arrChar[i] != ',') && (!inQuotation))
                    {
                        tmp += arrChar[i].ToString();
                    }
                }

                if (!tmp.Equals(string.Empty))
                    lstExpression.Add(tmp);

                return lstExpression;
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

        public void ParseExpression(string expression, ref string left, ref string right, ref string op)
        {
            bool inQuotation = false;
            string tmp = string.Empty;

            char[] arrChar = expression.ToCharArray();

            for (int i = 0; i < arrChar.Length; i++)
            {
                if ((arrChar[i] == '\'') && (!inQuotation))
                {
                    inQuotation = true;
                    tmp += arrChar[i].ToString();
                }
                else
                {
                    if (((arrChar[i] == '=') || (arrChar[i] == '<') || (arrChar[i] == '>') || (arrChar[i] == '!')) && (!inQuotation) ||
                        (arrChar[i] == ' '))
                    {
                        if ((left.Trim() == "") && (tmp.Trim()!=""))
                        {
                            tmp = tmp.Trim();
                            if (tmp.Substring(0, 1) == "(")
                                left = tmp.Substring(1, tmp.Length - 1);
                            else
                                left = tmp;

                            tmp = "";
                        }
                    }

                    if (arrChar[i] == '\'')
                        inQuotation = false;

                    tmp += arrChar[i].ToString();

                    if ((tmp.Trim() == "=") || (tmp.Trim() == "<>") || (tmp.Trim() == "!=") || (tmp.Trim() == ">=") || (tmp.Trim() == "<=")
                        || (tmp.Trim().ToLower() == "in") || (tmp.Trim().ToLower() == "not in") || (tmp.Trim().ToLower() == "like")
                        || (tmp.Trim().ToLower() == "not like") || (tmp.Trim() == "<") || (tmp.Trim() == ">"))
                    {
                        if (tmp.Trim() == ">" || tmp.Trim() == "<")
                        {
                            if ((i + 1) < arrChar.Length)
                            {
                                if (arrChar[i + 1] == '=')
                                {
                                    op = tmp + "=";
                                    i++;
                                }
                                else
                                    op = tmp;
                            }
                            else
                                op = tmp;
                        }
                        else
                        {
                            op = tmp;
                        }
                        tmp = "";
                    }

                }
            }

            if (tmp.Trim() != "")
            {
                tmp = tmp.Trim();
                if ((tmp.Substring(tmp.Length - 1, 1) == ")") && (tmp.Substring(0, 1) == "("))
                    right = tmp.Substring(0,tmp.Length-1);
                else
                    right = tmp;
            }

        }

        public string CleanExpression(string expression)
        {
            bool inQuotation = false;
            string tmp = string.Empty;
            string result = string.Empty;

            #region remove unused space
            char[] arrChar = expression.ToCharArray();

            for (int i = 0; i < arrChar.Length; i++)
            {
                if ((arrChar[i] == '\'') && (!inQuotation))
                {
                    inQuotation = true;
                    tmp += arrChar[i].ToString();
                }
                else
                {
                    if ((arrChar[i] == ' ') && (!inQuotation))
                    {
                        if (tmp.Trim().Length >= 2)
                        {
                            if ((tmp.Trim().ToLower().Substring(tmp.Trim().Length - 2, 2) == "or") || (tmp.Trim().ToLower().Substring(tmp.Trim().Length - 2, 2) == "in"))
                            {
                                if ((tmp.Trim().ToLower().Substring(tmp.Trim().Length - 2, 2) == "or") ||
                                    (tmp.Trim().ToLower().Substring(tmp.Trim().Length - 2, 2) == "in"))
                                {
                                    result += tmp.Trim().Substring(0, tmp.Trim().Length - 2) + " " + tmp.Trim().Substring(tmp.Trim().Length - 2, 2) + " ";
                                }

                                tmp = "";
                            }
                        }

                        if (tmp.Trim().Length >= 3)
                        {
                            if ((tmp.Trim().ToLower().Substring(tmp.Trim().Length - 3, 3) == "and")
                                || (tmp.Trim().ToLower().Substring(tmp.Trim().Length - 3, 3) == "not"))
                            {
                                if ((tmp.Trim().ToLower().Substring(tmp.Trim().Length - 3, 3) == "and") ||
                                    (tmp.Trim().ToLower().Substring(tmp.Trim().Length - 3, 3) == "not"))
                                {
                                    result += tmp.Trim().Substring(0, tmp.Trim().Length - 3) + " " + tmp.Trim().Substring(tmp.Trim().Length - 3, 3) + " ";
                                }

                                tmp = "";
                            }
                        }

                        if (tmp.Trim().Length >= 4)
                        {
                            if ((tmp.Trim().ToLower().Substring(tmp.Trim().Length - 4, 4) == "like"))
                            {
                                if ((tmp.Trim().ToLower().Substring(tmp.Trim().Length - 4, 4) == "like"))
                                {
                                    result += tmp.Trim().Substring(0, tmp.Trim().Length - 4) + " " + tmp.Trim().Substring(tmp.Trim().Length - 4, 4) + " ";
                                }

                                tmp = "";
                            }
                        }
                    }
                    else
                    {
                        tmp += arrChar[i].ToString();
                    }
                }
            }

            if (tmp.Trim() != "")
                result += tmp;

            #endregion

            //replace <> with !=
            result = result.Replace("<>", "!=");

            return result;
        }

        public string FormatValue(object obj)
        {
            string r = string.Empty;
            switch (obj.GetType().FullName)
            {
                case "System.String":
                    r = "\"" + obj.ToString() + "\""; 
                    break;
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Float":
                case "System.Decimal":
                    r = "Convert.ToDouble(" + obj.ToString() + ")";
                    break;
                case "System.Datetime":
                    r = UtilDatetime.FormateDateTime1(DateTime.Parse(obj.ToString()));
                    break;
            }

            return r;
        }

        public object ParseVariable(PrCalculationStore store,string variable)
        {
            //variable: {$sysLvType},{$usrLvType}
            //attribute:{@gradename}
            //salaryitem:{#YinFa}

            object valu = null;
            switch (variable.Trim().Substring(0, 1))
            {
                case "$":
                    valu = ParseVariableVariable(store,variable);
                    break;
                case "@":
                    valu = ParseVariableAttribute(store, variable);
                    break;
                case "#":
                    valu = ParseVariableSalaryItem(store, variable);
                    break;
                default:
                    valu = "####";
                    break;
            }

            return valu;
        }

        public object ParseVariableVariable(PrCalculationStore store, string variable)
        {
            object valu = null;
            string vari = variable.Substring(1, variable.Length - 1).Trim();
            if (vari.Substring(0, 3).ToLower() == "sys")
            {
                //系统变量
                valu = ParseVariableVariableSystem(store, variable);
            }
            else if (vari.Substring(0, 3).ToLower() == "usr")
            {
                //用户变量
                valu = ParseVariableVariableUser(store, variable);
            }
            else
            {
                //非法定义(即不是系统变量也是不用户变量

                log.LogInfoWithLevel("Unrecognized variable[" + vari + "] found. Return value=0.", Log_LoggingLevel.Normal);
                valu = 0;
            }

            return valu;
        }

        public object ParseVariableVariableUser(PrCalculationStore store, string variable)
        {
            try
            {
                //用户变量,则直接抓去值
                object valu = null;
                string vari = variable.Substring(1, variable.Length - 1).Trim();

                var q = store.lstVariables.Where(p => p.vacd == vari).ToList();

                if (q.Count > 0)
                {
                    tprvarble mdl = q.First();
                    switch (mdl.vlty)
                    {
                        case "N":
                            try
                            {
                                valu = double.Parse(mdl.vlva);
                            }
                            catch (Exception ex)
                            {
                                //如果值定义有错误，则直接取0
                                valu = 0;
                            }
                            break;
                        case "C":
                            valu = mdl.vlva;
                            break;
                        case "D":
                            valu = DateTime.Parse(mdl.vlva);
                            break;
                        default:
                            //如果类型定义错误，则直接取0
                            valu = 0;
                            break;
                    }
                }
                else
                {
                    log.LogInfoWithLevel("Undefined user variable[" + vari + "] found. Return value=0.", Log_LoggingLevel.Normal);
                    valu = 0;
                }

                return valu;
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

        public object ParseVariableVariableSystem(PrCalculationStore store, string variable)
        {
            object valu = null;
            try
            {
                string vari = variable.Substring(1, variable.Length-1).Trim();

                var q = store.lstVariables.Where(p => p.vacd.Trim() == vari).ToList();

                if (q.Count > 0)
                {
                    tprvarble mdl = q.First();

                    ISysVariables sysvar = Type.GetType(mdl.vlva.Trim()).GetConstructor(new Type[] { }).Invoke(new object[] { }) as ISysVariables;
                    valu = sysvar.GetValue(store);
                }
                else
                {
                    log.LogInfoWithLevel("Undefined system variable[" + vari + "] found. Return value=0.", Log_LoggingLevel.Normal);
                    valu = 0;
                }
            }
            catch (Exception ex)
            {
                valu = 0;
            }

            return valu;
        }

        public object ParseVariableAttribute(PrCalculationStore store,string variable)
        {
            object valu = null;
            string vari = variable.Substring(1,variable.Length-1).Trim();
            try
            {
                valu = typeof(vw_employment).GetProperty(vari).GetValue(store.CurrentEmployment, new object[] { });
            }
            catch(Exception  ex)
            {
                //如果员工属性没有找到，或者抓取错误，则给空串
                log.LogInfoWithLevel("Undefined staff attribute[" + vari + "] found. Return value=0.", Log_LoggingLevel.Normal);
                valu = "";
            }

            return valu;
        }

        public object ParseVariableSalaryItem(PrCalculationStore store,string variable)
        {
            object valu = null;
            string vari = string.Empty;
            try
            {
                vari = variable.Substring(1, variable.Length-1).Trim();
                List<tprsalhi> q = (from p in store.lstSalaryResult
                                    where p.itcd == vari
                                    select p).ToList();

                if (q.Count > 0)
                {
                    //如果在rule set中一个salary item被定义多次，则取第一次出现的值
                    valu = q.First().valu;
                }
                else
                {
                    //如果没有定义，或者还没有计算出来，则取0
                    log.LogInfoWithLevel("Salary item is unknown or does not be calculated yet[" + vari + "]. Return value=0.", Log_LoggingLevel.Normal);
                    valu = 0;
                }
            }
            catch (Exception ex)
            {
                //如果出现任何错误则取值0
                log.LogInfoWithLevel("Get salary item fail.[" + vari + "]. Return value=0.", Log_LoggingLevel.Normal);
                valu = 0;
            }

            return valu;
        }

        public object GetFormularItemValue(PrCalculationStore store, string valu)
        {
            //计算公式的值
            try
            {
                if (valu.Trim().Equals(string.Empty))
                    return "";

                string actValue = ReplaceConditionWithActualValue(store, valu);

                return UtilEvaluator.EvaluateToDouble(actValue);
            }
            catch (UtilException ex)
            {
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }
}
