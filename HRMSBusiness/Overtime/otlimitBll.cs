using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Overtime;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Utility;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Overtime
{
    public class otlimitBll : BaseBll
    {
        otlimitDal dal = null;

        public otlimitBll()
        {
            dal = new otlimitDal();
            baseDal = dal;
        }

        public void ApplyTo(List<vw_employment> _emps, totlimit obj)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int maxlmno = GetMaxNo("totlimit", "lmno").Value;
                    for (int i = 0; i < _emps.Count; i++)
                    {
                        if (_emps[i].emno != obj.lmva)
                        {
                            List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "lmby", ColumnValue = _emps[i].emno }, 
                                                                                new ColumnInfo(){ColumnName="lmva",ColumnValue=obj.lmva},
                                                                                new ColumnInfo(){ColumnName="lmsc",ColumnValue=obj.lmsc},
                                                                                new ColumnInfo(){ColumnName="otcd",ColumnValue=obj.otcd}
                         };

                            totlimit oldobj = GetSelectedObject<totlimit>(parameters);

                            if (oldobj == null)
                            {
                                //新增
                                maxlmno++;

                                totlimit newobj = new totlimit();
                                newobj.lmtm = obj.lmtm;
                                newobj.lmur = obj.lmur;
                                newobj.lmby = _emps[i].emno;
                                newobj.lmno = maxlmno;
                                newobj.lmsc = obj.lmsc;
                                newobj.lmtx = _emps[i].sfid + " - " + _emps[i].ntnm;
                                newobj.lmva = obj.lmva;
                                newobj.mxoh = obj.mxoh;
                                newobj.mxth = obj.mxth;
                                newobj.otcd = obj.otcd;

                                DoInsert<totlimit>(newobj);
                            }
                            else
                            {
                                //更新
                                oldobj.lmtm = obj.lmtm;
                                oldobj.lmur = obj.lmur;
                                oldobj.mxoh = obj.mxoh;
                                oldobj.mxth = obj.mxth;
                                DoUpdate<totlimit>(oldobj);
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
