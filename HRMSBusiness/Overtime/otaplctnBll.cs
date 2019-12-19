using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSData.Overtime;
using GotWell.Common;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Overtime
{
    public class otaplctnBll : BaseBll
    {
         otaplctnDal dal = null;

         public otaplctnBll()
         {
             dal = new otaplctnDal();
             baseDal = dal;
         }

        public string getNewAppNo()
        {
            try
            {
                //得到最大加班申请单号
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

        public void SaveOTApplication(totaplctn _overtimeApp)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //分析请假并保存
                    otaplctnBll bll = new otaplctnBll();

                    if (_overtimeApp.otst == "Approved")
                    {
                        //如果自动转换成休假，则新增lvdfbyemp
                        double hoursToTTLV = 0;
                        int lvdefseq = 0;

                        List<ColumnInfo> ottypeParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "otcd", ColumnValue = _overtimeApp.otcd } };

                        tottype otType = GetSelectedObject<tottype>(ottypeParameters);

                        if (otType.autr == "Y")
                        {
                            otttlvBll ttlvBll = new otttlvBll();
                            hoursToTTLV = ttlvBll.TransferToLeave(_overtimeApp, otType, ref lvdefseq);
                        }

                        //如果审核，则更新otdetail
                        SaveToOvertimeDetail(_overtimeApp, hoursToTTLV, lvdefseq);
                    }

                    DoInsert<totaplctn>(_overtimeApp);

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

        public void UpdateOTApplication(totaplctn _overtimeApp)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    otaplctnBll bll = new otaplctnBll();

                    List<ColumnInfo> apnoParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "apno", ColumnValue = _overtimeApp.apno } };
                    List<ColumnInfo> dtlParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "refno", ColumnValue = _overtimeApp.apno } };
                    totdetail dtl = GetSelectedObject<totdetail>(dtlParameters);
                    if (dtl != null)
                    {
                        //删除lvdfbyemp
                        //如果转换成的休假已经被申请了，则无法控制
                        List<ColumnInfo> lvdefempParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = dtl.emno },
                                                                                   new ColumnInfo() { ColumnName = "sqno", ColumnValue = dtl.tlrf.ToString() ,ColumnType="int"}};
                        DoDelete<tlvdfbyem>(lvdefempParameters);

                        //删除otdetail
                        dal.DeleteOTDtl(dtlParameters);
                    }

                    if (_overtimeApp.otst == "Approved")
                    {
                        //如果自动转换成休假，则新增lvdfbyemp
                        double hoursToTTLV = 0;
                        int lvdefseq = 0;

                        List<ColumnInfo> ottypeParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "otcd", ColumnValue = _overtimeApp.otcd } };

                        tottype otType = GetSelectedObject<tottype>(ottypeParameters);

                        if (otType.autr == "Y")
                        {
                            otttlvBll ttlvBll = new otttlvBll();
                            hoursToTTLV = ttlvBll.TransferToLeave(_overtimeApp, otType, ref lvdefseq);
                        }

                        //如果审核，则更新otdetail
                        SaveToOvertimeDetail(_overtimeApp, hoursToTTLV, lvdefseq);
                    }

                    DoUpdate<totaplctn>(_overtimeApp, apnoParameters);

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

        public void DeleteOTApplication(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    totdetail dtl = GetSelectedObject<totdetail>(_parameters);

                    if (dtl != null)
                    {
                        //删除lvdfbyemp
                        //如果转换成的休假已经被申请了，则无法控制
                        List<ColumnInfo> lvdefempParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = dtl.emno },
                                                                                new ColumnInfo() { ColumnName = "sqno", ColumnValue = dtl.tlrf.ToString() ,ColumnType="int"}};
                        DoDelete<tlvdfbyem>(lvdefempParameters);

                        //删除otdetail
                        dal.DeleteOTDtl(_parameters);
                    }

                    DoDelete<totaplctn>(_parameters);

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

        private void SaveToOvertimeDetail(totaplctn _overtimeApp, double _hoursToTTLV, int _lvdefseq)
        {
            //如果OT申请审核了话，则插入otdetail
            totdetail dtl = new totdetail();
            dtl.emno = _overtimeApp.emno;
            dtl.edtm = _overtimeApp.totm;
            dtl.lmtm = _overtimeApp.lmtm;
            dtl.lmur = _overtimeApp.lmur;
            dtl.otdt = _overtimeApp.frtm;
            dtl.othr = _overtimeApp.othr;
            dtl.othm = _overtimeApp.othm;
            dtl.othr = 0;
            dtl.otcd = _overtimeApp.otcd;
            dtl.rfno = _overtimeApp.apno;
            dtl.sttm = _overtimeApp.frtm;
            dtl.istr = _hoursToTTLV == 0 ? "N" : "Y";
            dtl.ttlv = _hoursToTTLV == 0 ? "" : _overtimeApp.tottype.otcd;
            dtl.tlhr = _hoursToTTLV;
            dtl.tlrf = _lvdefseq.ToString();

            DoInsert<totdetail>(dtl);
        }

        public string CalcOTTime(totaplctn _otApp)
        {
            try
            {
                //只分析请假
                otanaovtBll bll = new otanaovtBll();

                List<ColumnInfo> dateParameters = new List<ColumnInfo>();
                dateParameters.Add(new ColumnInfo() { ColumnName = "otstart", ColumnValue = UtilDatetime.FormateDateTime1(_otApp.frtm), ColumnType = "datetime" });
                dateParameters.Add(new ColumnInfo() { ColumnName = "otend", ColumnValue = UtilDatetime.FormateDateTime1(_otApp.totm), ColumnType = "datetime" });

                List<ColumnInfo> personParameters = new List<ColumnInfo>();
                personParameters.Add(new ColumnInfo() { ColumnName = "emp.emno", ColumnValue = _otApp.emno });

                bll.AnalyzeOvertime(dateParameters, personParameters, null, false);

                return "totalothr:'" + bll.TotalOTHours.ToString() + "',otcd:'" + bll.otcd + "'";
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

        public LvSettingInfo GetEmpOTSettings(string _emno, string _otcd, DateTime _otdt)
        {
            try
            {
                otanaovtBll bll = new otanaovtBll();
                return bll.GetEmpOTSettings(_emno, _otcd, _otdt);
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

    }
}
