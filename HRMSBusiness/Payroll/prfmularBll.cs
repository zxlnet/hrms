using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prfmularBll : BaseBll
    {
        prfmularDal dal = null;

        public prfmularBll()
        {
            dal = new prfmularDal();
            baseDal = dal;
        }

        public List<tprfmudtl> GetFormulaDtl(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount) 
        {
            try
            {
                return dal.GetFormulaDtl(_parameter, paging, start, num, ref totalRecordCount);
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

        public void InsertFormula(tprfmular obj, List<tprfmudtl> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //insert
                    DoInsert<tprfmular>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tprfmudtl>(list[i]);
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

        public void UpdateFormula(tprfmular obj, List<tprfmudtl> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "frcd", ColumnValue = obj.frcd } };

                    DoMultiDelete<tprfmudtl>(parameters);

                    //update
                    dal.DoUpdate<tprfmular>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        dal.DoInsert<tprfmudtl>(list[i]);
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


        public void DeleteFormula(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    tprfmular obj = GetSelectedObject<tprfmular>(_parameters);

                    DoMultiDelete<tprfmudtl>(_parameters);

                    //delete
                    dal.DoDelete<tprfmular>(obj);

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
