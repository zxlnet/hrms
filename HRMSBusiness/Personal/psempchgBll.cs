using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Utility;
using GotWell.Common;
using GotWell.Model.Common;
using System.Reflection;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class psempchgBll:BaseBll
    {
        psempchgDal dal = null;

        public psempchgBll()
        {
            dal = new psempchgDal();
            baseDal = dal;
        }

        public void InsertEmpChange(List<tpsempchg> _list)
        {
            try
            {
                if (_list.Count <= 0) return;

                int? t = GetMaxsqno("tpsempchg", _list[0].emno);
                int seqno = 0;
                if (t.HasValue) seqno = t.Value; else seqno = 0;

                string gpid = UtilDatetime.FormatDateTime4(DateTime.Now);

                List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _list[0].emno } };
                tpsemplym emp = GetSelectedObject<tpsemplym>(parameters);
                bool isUpdateEmployment = false;
                string changeType = _list[0].ctcd;

                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < _list.Count; i++)
                    {
                        tpsempchg obj = _list[i];

                        obj.sqno = seqno + i + 1;
                        obj.lmtm = DateTime.Now;
                        obj.lmur = Function.GetCurrentUser();
                        obj.gpid = gpid;
                        //get old value
                        PropertyInfo props = emp.GetType().GetProperty(obj.chfi);
                        if (props != null)
                        {
                            object v = props.GetValue(emp, null);
                            obj.olva = v == null ? null : v.ToString();
                        }


                        //判断是否立即生效
                        if ((obj.isim=="Y") || (obj.efdt<=DateTime.Now))
                        {
                            props.SetValue(emp, obj.neva, null);

                            obj.isby = Function.GetCurrentUser();
                            obj.isdt = DateTime.Now;
                            obj.issu = "Y";
                            isUpdateEmployment = true;
                        }

                        DoInsert<tpsempchg>(obj);

                    }

                    if (isUpdateEmployment)
                    {
                        DoUpdate<tpsemplym>(emp);
                        //更新雇佣历史
                        psemplymBll empBll = new psemplymBll();
                        empBll.CopyEmpToHistory(emp, changeType);
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

        public void UpdateEmpChange(tpsempchg _obj)
        {
            try
            {
                if (_obj == null) return;

                List<ColumnInfo> dtlParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _obj.emno },
                                                                          new ColumnInfo() { ColumnName = "sqno", ColumnValue = _obj.sqno.ToString(),ColumnType="int" }};
                List<ColumnInfo> empParameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _obj.emno } };
                tpsemplym emp = GetSelectedObject<tpsemplym>(empParameters);
                bool isUpdateEmployment = false;
                string changeType = _obj.ctcd;

                using (TransactionScope scope = new TransactionScope())
                {
                    //get old value
                    PropertyInfo props = emp.GetType().GetProperty(_obj.chfi);
                    if (props != null)
                    {
                        object v = props.GetValue(emp, null);
                        _obj.olva = v == null ? null : v.ToString();
                    }


                    //判断是否立即生效
                    if ((_obj.isim == "Y") || (_obj.efdt <= DateTime.Now))
                    {
                        props.SetValue(emp, _obj.neva, null);

                        _obj.isby = Function.GetCurrentUser();
                        _obj.isdt = DateTime.Now;
                        _obj.issu = "Y";
                        isUpdateEmployment = true;
                    }

                    DoUpdate<tpsempchg>(_obj, dtlParameters);

                    if (isUpdateEmployment)
                    {
                        DoUpdate<tpsemplym>(emp, empParameters);
                        //更新雇佣历史
                        psemplymBll empBll = new psemplymBll();
                        empBll.CopyEmpToHistory(emp, changeType);
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
