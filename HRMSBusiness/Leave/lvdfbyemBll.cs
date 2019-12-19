using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Leave
{
    public class lvdfbyempBll : BaseBll
    {

         lvdfbyempDal dal = null;

         public lvdfbyempBll()
         {
             dal = new lvdfbyempDal();
             baseDal = dal;
         }

         public void ApplyTo(List<string> _emps, tlvdfbyem obj)
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
                                                                                new ColumnInfo(){ ColumnName="sqno",ColumnValue=obj.sqno.ToString(),ColumnType="int"}
                         };

                            tlvdfbyem oldobj = GetSelectedObject<tlvdfbyem>(parameters);

                            if (oldobj == null)
                            {
                                //新增
                                tlvdfbyem newobj = new tlvdfbyem();
                                newobj.emno = _emps[i];
                                newobj.days = obj.days;
                                newobj.exdt = obj.exdt;
                                newobj.lmtm = obj.lmtm;
                                newobj.lmur = obj.lmur;
                                newobj.ltcd = obj.ltcd;
                                newobj.remk = obj.remk;

                                int? maxsqno = GetMaxsqno("tlvdfbyem", _emps[i]);
                                newobj.sqno = maxsqno.HasValue ? maxsqno.Value : 1;

                                DoInsert<tlvdfbyem>(newobj);
                            }
                            else
                            {
                                //更新
                                oldobj.days = obj.days;
                                oldobj.exdt = obj.exdt;
                                oldobj.lmtm = obj.lmtm;
                                oldobj.lmur = obj.lmur;
                                oldobj.ltcd = obj.ltcd;
                                oldobj.remk = obj.remk;

                                DoUpdate<tlvdfbyem>(oldobj);
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
