using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.Model.HRMS;
using GotWell.Utility;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atpvtcldBll : BaseBll
    {
        atpvtcldDal dal = null;

         public atpvtcldBll()
         {
             dal = new atpvtcldDal();
             baseDal = dal;
         }
         public void ApplyTo(List<vw_employment> _emps, tatpricld obj)
         {
             try
             {
                 for (int i = 0; i < _emps.Count; i++)
                 {
                     if (_emps[i].emno != obj.emno)
                     {
                         List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emps[i].emno }, 
                                                                                new ColumnInfo(){ColumnName="cddt",ColumnValue=UtilDatetime.FormatDate1(obj.cddt),ColumnType="datetime"}};

                         tatpricld oldobj = GetSelectedObject<tatpricld>(parameters);

                         if (oldobj == null)
                         {
                             //新增
                             tatpricld newobj = new tatpricld();
                             newobj.emno = _emps[i].emno;
                             newobj.cddt = obj.cddt;
                             newobj.htcd = obj.htcd;
                             newobj.otcd = obj.otcd;
                             newobj.lmtm = obj.lmtm;
                             newobj.lmur = obj.lmur;

                             DoInsert<tatpricld>(newobj);
                         }
                         else
                         {
                             //更新
                             oldobj.htcd = obj.htcd;
                             oldobj.otcd = obj.otcd;
                             oldobj.lmtm = obj.lmtm;
                             oldobj.lmur = obj.lmur;

                             DoUpdate<tatpricld>(oldobj);
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
