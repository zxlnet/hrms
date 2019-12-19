using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;
using GotWell.HRMS.HRMSData.Leave;
using GotWell.Model.Common;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Leave
{
    public class lvdfbyotBll : BaseBll
    {

        lvdfbyotDal dal = null;

        public lvdfbyotBll()
        {
            dal = new lvdfbyotDal();
            baseDal = dal;
        }

        public List<object> GetDefDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            return dal.GetDefDetails(_parameter, paging, start, num, ref totalRecordCount);
        }

        public void InsertDef(tlvdfbyot obj, List<tlvdfbyod> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    DoInsert<tlvdfbyot>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tlvdfbyod>(list[i]);
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

        public void UpdateDef(tlvdfbyot obj, List<tlvdfbyod> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "dfno", ColumnValue = obj.dfno.ToString(), ColumnType = "int" } };
                    dal.DoMultiDelete<tlvdfbyod>(parameters);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tlvdfbyod>(list[i]);
                    }

                    dal.DoUpdate<tlvdfbyot>(obj);

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
                    dal.DoMultiDelete<tlvdfbyod>(_parameters);
                    DoDelete<tlvdfbyot>(_parameters);

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
