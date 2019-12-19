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
    public class otdetailBll : BaseBll
    {
         otdetailDal dal = null;

         public otdetailBll()
         {
             dal = new otdetailDal();
             baseDal = dal;
         }

         public void SaveOTDetail(totdetail obj) 
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     //如果自动转换成休假，则新增lvdfbyemp
                     double hoursToTTLV = 0;
                     int lvdefseq = 0;

                     List<ColumnInfo> ottypeParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "otcd", ColumnValue = obj.otcd } };

                     tottype otType = GetSelectedObject<tottype>(ottypeParameters);

                     if (otType.autr == "Y")
                     {
                         otttlvBll ttlvBll = new otttlvBll();
                         hoursToTTLV = ttlvBll.TransferToLeave(obj, otType, ref lvdefseq);
                     }

                     //更新detail的ttlv信息
                     obj.istr = hoursToTTLV == 0 ? "N" : "Y";
                     obj.ttlv = hoursToTTLV == 0 ? "" : obj.tottype.otcd;
                     obj.tlhr = hoursToTTLV;
                     obj.tlrf = lvdefseq.ToString();
                     obj.lmur = Function.GetCurrentUser();
                     obj.lmtm = DateTime.Now;

                     baseDal.DoInsert<totdetail>(obj);

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
             finally
             {
             }
         }

         public void UpdateOTDetail(totdetail obj, List<ColumnInfo> _parameters)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = obj.emno }, 
                                                                                new ColumnInfo(){ColumnName="otdt",ColumnValue=UtilDatetime.FormatDate1(obj.otdt),ColumnType="datetime"},
                                                                                new ColumnInfo(){ColumnName="sttm",ColumnValue=UtilDatetime.FormatDate1(obj.sttm),ColumnType="datetime"}
                         };

                     totdetail oldobj = GetSelectedObject<totdetail>(parameters);
                     //删除lvdfbyemp
                     //如果转换成的休假已经被申请了，则无法控制
                     List<ColumnInfo> lvdefempParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = oldobj.emno },
                                                                                   new ColumnInfo() { ColumnName = "sqno", ColumnValue = oldobj.tlrf.ToString() ,ColumnType="int"}};
                     DoDelete<tlvdfbyem>(lvdefempParameters);


                     //如果自动转换成休假，则新增lvdfbyemp
                     double hoursToTTLV = 0;
                     int lvdefseq = 0;

                     List<ColumnInfo> ottypeParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "otcd", ColumnValue = oldobj.otcd } };

                     tottype otType = GetSelectedObject<tottype>(ottypeParameters);

                     if (otType.autr == "Y")
                     {
                         otttlvBll ttlvBll = new otttlvBll();
                         hoursToTTLV = ttlvBll.TransferToLeave(obj, otType, ref lvdefseq);
                     }

                     //更新detail的ttlv信息
                     obj.istr = hoursToTTLV == 0 ? "N" : "Y";
                     obj.ttlv = hoursToTTLV == 0 ? "" : otType.otcd;
                     obj.tlhr = hoursToTTLV;
                     obj.tlrf = lvdefseq.ToString();
                     obj.lmur = Function.GetCurrentUser();
                     obj.lmtm = DateTime.Now;

                     DoUpdate<totdetail>(obj, _parameters);

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
             finally
             {
             }
         }

         public void DeleteOTDetail(List<ColumnInfo> _parameters)
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

                     }

                     //删除otdetail
                     DoDelete<totdetail>(_parameters);

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

         public void ApplyTo(List<string> _emps,totdetail obj)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     for (int i = 0; i < _emps.Count; i++)
                     {
                         if (_emps[i] != obj.emno)
                         {
                             List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emps[i] }, 
                                                                                new ColumnInfo(){ColumnName="otdt",ColumnValue=UtilDatetime.FormatDate1(obj.otdt),ColumnType="datetime"},
                                                                                new ColumnInfo(){ColumnName="sttm",ColumnValue=UtilDatetime.FormatDate1(obj.sttm),ColumnType="datetime"}
                         };

                             totdetail oldobj = GetSelectedObject<totdetail>(parameters);

                             if (oldobj == null)
                             {
                                 //新增
                                 totdetail newobj = new totdetail();
                                 newobj.emno = _emps[i];
                                 newobj.edtm = obj.edtm;
                                 newobj.lmtm = obj.lmtm;
                                 newobj.lmur = obj.lmur;
                                 newobj.otdt = obj.otdt;
                                 newobj.othr = obj.othr;
                                 newobj.othm = obj.othm;
                                 newobj.oths = obj.oths;
                                 newobj.otcd = obj.otcd;
                                 newobj.sttm = obj.sttm;

                                 DoInsert<totdetail>(newobj);
                             }
                             else
                             {
                                 //更新
                                 oldobj.edtm = obj.edtm;
                                 oldobj.lmtm = obj.lmtm;
                                 oldobj.lmur = obj.lmur;
                                 oldobj.othr = obj.othr;
                                 oldobj.othm = obj.othm;
                                 oldobj.oths = obj.oths;
                                 oldobj.otcd = obj.otcd;
                                 DoUpdate<totdetail>(oldobj);
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
    }
}
