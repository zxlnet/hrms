using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.Model.HRMS;
using System.Runtime.InteropServices;
using GotWell.Model.Common;
using System.Transactions;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atoridatBll : BaseBll
    {
         atoridatDal dal = null;

         public atoridatBll()
         {
             dal = new atoridatDal();
             baseDal = dal;
         }

         public override void DoUpdate<T>(T obj, List<ColumnInfo> _parameter) 
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     DoMultiDelete<T>(_parameter);
                     DoInsert<T>(obj);
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

    }
}
