using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class psskillBll : BaseBll
    {
        psskillDal dal = null;

        public psskillBll()
        {
            dal = new psskillDal();
            baseDal = dal;
        }

        public List<object> GetSkillItems(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            return dal.GetSkillItems(_parameter, paging, start, num, ref totalRecordCount);
        }

        public void InsertSkill(tpsskill obj, List<tpsskiitm> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //insert
                    DoInsert<tpsskill>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tpsskiitm>(list[i]);
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

        public void UpdateSkill(tpsskill obj, List<tpsskiitm> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = obj.emno },
                                                    new ColumnInfo() { ColumnName = "sqno", ColumnValue = obj.sqno.ToString(),ColumnType="int" }};

                    DoMultiDelete<tpsskiitm>(parameters);

                    //update
                    dal.DoUpdate<tpsskill>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        dal.DoInsert<tpsskiitm>(list[i]);
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


        public void DeleteSkill(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DoMultiDelete<tpsskiitm>(_parameters);

                    //delete
                    DoDelete<tpsskill>(_parameters);

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
