using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Master;
using System.Transactions;
using GotWell.Model.HRMS;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Master
{
    public class bsratingBll : BaseBll
    {
        bsratingDal localDal = null;
        public bsratingBll()
        {
            localDal = new bsratingDal();
            baseDal = localDal;
        }

        public void CreateRating(tbsrating obj, List<tbsratdtl> listDtl)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DoInsert<tbsrating>(obj);

                    for (int i = 0; i < listDtl.Count; i++)
                    {
                        DoInsert<tbsratdtl>(listDtl[i]);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateRating(tbsrating obj, List<tbsratdtl> listDtl,List<string> lstDeleted)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<ColumnInfo> lstParameters = new List<ColumnInfo>()
                    {
                        new ColumnInfo(){ColumnName="racd",ColumnValue=obj.racd}
                    };

                    DoUpdate<tbsrating>(obj,lstParameters);

                    //deleted 
                    for (int i = 0; i < lstDeleted.Count; i++)
                    {
                        List<ColumnInfo> _parameters = new List<ColumnInfo>()
                            {
                                new ColumnInfo(){ColumnName ="racd",ColumnValue=obj.racd},
                                new ColumnInfo(){ColumnName ="rtcd",ColumnValue=lstDeleted[i]}
                            };

                        new BaseBll().DoDelete<tbsratdtl>(_parameters);

                    }

                    //updated & inserted
                    for (int i = 0; i < listDtl.Count; i++)
                    {
                        List<ColumnInfo> _parameters = new List<ColumnInfo>()
                            {
                                new ColumnInfo(){ColumnName ="racd",ColumnValue=obj.racd},
                                new ColumnInfo(){ColumnName ="rtcd",ColumnValue=listDtl[i].rtcd}
                            };

                        List<tbsratdtl> lstObj = GetSelectedRecords<tbsratdtl>(_parameters);

                        if (lstObj.Count > 0)
                            DoUpdate<tbsratdtl>(listDtl[i], _parameters);
                        else
                            DoInsert<tbsratdtl>(listDtl[i]);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void DoDelete<T>(List<ColumnInfo> _parameter)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DoMultiDelete<tbsratdtl>(_parameter);

                    new BaseBll().DoDelete<tbsrating>(_parameter);

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
