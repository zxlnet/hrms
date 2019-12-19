using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atrosterBll : BaseBll
    {
        atrosterDal dal = null;

         public atrosterBll()
         {
             dal = new atrosterDal();
             baseDal = dal;
         }

         public List<object> GetRosterDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
         {
             return dal.GetRosterDetails(_parameter, paging, start, num, ref totalRecordCount);
         }

        public void InsertRoster(tatroster obj,List<tatrosdtl> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //insert
                    DoInsert<tatroster>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tatrosdtl>(list[i]);
                    }
                    scope.Complete();
                }
            }
            catch(UtilException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }

        public void UpdateRoster(tatroster obj, List<tatrosdtl> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rscd", ColumnValue = obj.rscd } };

                    //DoMultiDelete<tatrosdtl>(parameters);
                    List<tatrosdtl> oldList = GetSelectedRecords<tatrosdtl>(parameters);

                    for (int i = 0; i < oldList.Count; i++)
                    {
                        DoDelete<tatrosdtl>(oldList[i]);
                    }

                    //update
                    DoUpdate<tatroster>(obj, parameters);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tatrosdtl>(list[i]);
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


        public void DeleteRoster(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    //List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rscd", ColumnValue = obj.rscd } };

                    tatroster obj = GetSelectedObject<tatroster>(_parameters);

                    DoMultiDelete<tatrosdtl>(_parameters);
                    //List<tatrosdtl> oldList = GetSelectedRecords<tatrosdtl>(_parameters);

                    //for (int i = 0; i < oldList.Count; i++)
                    //{
                    //    dal.DoDelete<tatrosdtl>(oldList[i]);
                    //}

                    //delete
                    dal.DoDelete<tatroster>(obj);

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