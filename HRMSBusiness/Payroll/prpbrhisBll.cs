using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using System.Transactions;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prpbrhisBll : BaseBll
    {
        prpbrhisDal dal = null;

        public prpbrhisBll()
        {
            dal = new prpbrhisDal();
            baseDal = dal;
        }

        public void ApplyTo(List<vw_employment> _emps, tprpbrhi obj)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < _emps.Count; i++)
                    {
                        if (_emps[i].emno != obj.emno)
                        {
                            List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emps[i].emno }, 
                                                                                new ColumnInfo(){ColumnName="rscd",ColumnValue=obj.rscd},
                                                                                new ColumnInfo(){ColumnName="efdt",ColumnValue=UtilDatetime.FormatDate1(obj.efdt),ColumnType="datetime"}
                         };

                            tprpbrhi oldobj = GetSelectedObject<tprpbrhi>(parameters);

                            if (oldobj == null)
                            {
                                //新增
                                tprpbrhi newobj = new tprpbrhi();
                                newobj.efdt = obj.efdt;
                                newobj.emno = _emps[i].emno;
                                newobj.exdt = obj.exdt;
                                newobj.lmtm = obj.lmtm;
                                newobj.lmur = obj.lmur;
                                newobj.rscd = obj.rscd;

                                DoInsert<tprpbrhi>(newobj);
                            }
                            else
                            {
                                //更新
                                oldobj.exdt = obj.exdt;
                                oldobj.lmtm = obj.lmtm;
                                oldobj.lmur = obj.lmur;

                                DoUpdate<tprpbrhi>(oldobj);
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
