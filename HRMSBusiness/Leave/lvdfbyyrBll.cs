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
    public class lvdfbyyearsBll : BaseBll
    {
         lvdfbyyearsDal dal = null;

         public lvdfbyyearsBll()
         {
             dal = new lvdfbyyearsDal();
             baseDal = dal;
         }

         public List<object> GetDefDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
         {
             return dal.GetDefDetails(_parameter, paging, start, num, ref totalRecordCount);
         }

         public void InsertDef(List<tlvdfbyyr> list)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {

                     for (int i = 0; i < list.Count; i++)
                     {
                         DoInsert<tlvdfbyyr>(list[i]);
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

         public void UpdateDef(string ltcd,List<tlvdfbyyr> list)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     //delete first
                     List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "ltcd", ColumnValue = ltcd } };
                     dal.DoMultiDelete<tlvdfbyyr>(parameters);

                     for (int i = 0; i < list.Count; i++)
                     {
                         DoInsert<tlvdfbyyr>(list[i]);
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


         public void DeleteDef(List<ColumnInfo> _parameters)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     dal.DoMultiDelete<tlvdfbyyr>(_parameters);

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
