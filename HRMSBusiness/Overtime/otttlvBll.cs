using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Overtime;
using GotWell.Common;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Overtime
{
    public class otttlvBll : BaseBll
    {
         otttlvDal dal = null;

         public otttlvBll()
         {
             dal = new otttlvDal();
             baseDal = dal;
         }

        public void BatchTransferToLeave(List<ColumnInfo> _transferParameters,List<ColumnInfo> _personalParameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DateTime frtm = Convert.ToDateTime(_transferParameters[0].ColumnValue);
                    DateTime totm = Convert.ToDateTime(_transferParameters[1].ColumnValue).AddDays(1);
                    List<totdetail> lstOTDetail = dal.GetOTDetailForTTLV(_personalParameters, frtm, totm);

                    for (int i = 0; i < lstOTDetail.Count; i++)
                    {
                        totdetail dtl = lstOTDetail[i];
                        int lvdefseq = 0;
                        double hoursToTTLV = TransferToLeave(dtl, dtl.tottype, ref lvdefseq);

                        //更新detail的ttlv信息
                        dtl.istr = hoursToTTLV == 0 ? "N" : "Y";
                        dtl.ttlv = hoursToTTLV == 0 ? "" : dtl.tottype.otcd;
                        dtl.tlhr = hoursToTTLV;
                        dtl.tlrf = lvdefseq.ToString();
                        dtl.lmur = Function.GetCurrentUser();
                        dtl.lmtm = DateTime.Now;

                        DoUpdate<totdetail>(dtl);
                    }

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


        public double TransferToLeave(totaplctn _overtimeApp,tottype _ottype, ref int lvdefseq)
        {
            //自动转换成休假，会检查limit
            otanaovtBll bll = new otanaovtBll();
            LvSettingInfo settingInfo = bll.GetEmpOTSettings(_overtimeApp.emno, _overtimeApp.otcd, _overtimeApp.frtm);

            double hoursToTTLV = 0;
            double hoursForTTLV = (((_overtimeApp.othm == 0) || (_overtimeApp.othm.HasValue == false)) ? _overtimeApp.othr : _overtimeApp.othm).Value;

            if (settingInfo.MinBalance >= hoursForTTLV)
                hoursToTTLV = hoursForTTLV;
            else
                hoursToTTLV = settingInfo.MinBalance;

            if (hoursToTTLV > 0)
            {
                tlvdfbyem lvemp = new tlvdfbyem();
                //problem here,转换成天数后怎么统计
                lvemp.days = settingInfo.MinBalance;
                lvemp.emno = _overtimeApp.emno;
                lvemp.exdt = _overtimeApp.frtm.AddYears(1); //default 1 year later
                lvemp.lmtm = DateTime.Now;
                lvemp.lmur = Constant.SYSTEM_USER_ID;
                lvemp.ltcd = _ottype.ttlv;
                lvemp.remk = "Transfer from overtime[appno:" + _overtimeApp.apno + ",frtm:" + UtilDatetime.FormateDateTime1(_overtimeApp.frtm)
                                + ",totm:" + UtilDatetime.FormateDateTime1(_overtimeApp.totm) + "]";
                lvemp.sqno = GetMaxsqno("tlvdfbyem", _overtimeApp.emno).Value;

                DoInsert<tlvdfbyem>(lvemp);

                lvdefseq = lvemp.sqno;
            }

            return hoursToTTLV;
        }

        public double TransferToLeave(totdetail _otdetail,tottype _ottype, ref int lvdefseq)
        {
            otanaovtBll bll = new otanaovtBll();
            LvSettingInfo settingInfo = bll.GetEmpOTSettings(_otdetail.emno, _otdetail.otcd, _otdetail.sttm);

            double hoursToTTLV = 0;
            double hoursForTTLV = (((_otdetail.othm == 0) ) ? _otdetail.othr : _otdetail.othm).Value;

            if (settingInfo.MinBalance >= hoursForTTLV)
                hoursToTTLV = hoursForTTLV;
            else
                hoursToTTLV = settingInfo.MinBalance;

            if (hoursToTTLV > 0)
            {
                tlvdfbyem lvemp = new tlvdfbyem();
                //problem here,转换成天数后怎么统计
                lvemp.days = hoursToTTLV;
                lvemp.emno = _otdetail.emno;
                lvemp.exdt = _otdetail.sttm.AddYears(1); //default 1 year later
                lvemp.lmtm = DateTime.Now;
                lvemp.lmur = Constant.SYSTEM_USER_ID;
                lvemp.ltcd = _ottype.ttlv;
                lvemp.remk = "Transfer from overtime[" + "frtm:" + UtilDatetime.FormateDateTime1(_otdetail.sttm)
                                + ",totm:" + UtilDatetime.FormateDateTime1(_otdetail.edtm) + "]";
                int? maxsqno = GetMaxsqno("tlvdfbyem", _otdetail.emno);
                maxsqno = maxsqno.HasValue == false ? 0 : maxsqno.Value;
                lvemp.sqno = maxsqno.Value + 1;

                DoInsert<tlvdfbyem>(lvemp);

                lvdefseq = lvemp.sqno;
            }

            return hoursToTTLV;
        }
    }
}
