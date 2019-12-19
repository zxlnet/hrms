using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atroshisBll : BaseBll
    {
        atroshisDal dal = null;

         public atroshisBll()
         {
             dal = new atroshisDal();
             baseDal = dal;
         }

        public bool CheckRosterHistory(tatroshi obj)
        {
            return dal.CheckRosterHistory(obj);

        }

        public void ApplyTo(List<vw_employment> _emps, object _obj)
        {
            try
            {
                tatroshi obj = _obj as tatroshi;

                for (int i = 0; i < _emps.Count; i++)
                {
                    if (_emps[i].emno != obj.emno)
                    {
                        List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emps[i].emno }, 
                                                                                new ColumnInfo(){ColumnName="efdt",ColumnValue=UtilDatetime.FormatDate1(obj.efdt.Value),ColumnType="datetime"}};

                        tatroshi oldobj = dal.GetSelectedObject(parameters);

                        if (oldobj == null)
                        {
                            //新增
                            tatroshi newobj = new tatroshi();
                            newobj.emno = _emps[i].emno;
                            newobj.efdt = obj.efdt;
                            newobj.exdt = obj.exdt;
                            newobj.iaod = obj.iaod;
                            newobj.rscd = obj.rscd;
                            newobj.sqno = GetMaxsqno("tatroshis", _emps[i].emno).Value + 1;
                            newobj.lmtm = obj.lmtm;
                            newobj.lmur = obj.lmur;

                            if (CheckRosterHistory(newobj))
                                DoInsert<tatroshi>(newobj);
                        }
                        else
                        {
                            //更新
                            oldobj.exdt = obj.exdt;
                            oldobj.iaod = obj.iaod;
                            oldobj.rscd = obj.rscd;
                            oldobj.lmtm = obj.lmtm;
                            oldobj.lmur = obj.lmur;

                            if (CheckRosterHistory(oldobj))
                                DoUpdate<tatroshi>(oldobj);
                        }
                    }
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
