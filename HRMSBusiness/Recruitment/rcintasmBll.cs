using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Recruitment;
using GotWell.Model.Common;
using System.Transactions;
using GotWell.Model.HRMS;

namespace GotWell.HRMS.HRMSBusiness.Recruitment
{
    public class rcintasmBll : BaseBll
    {
        rcintasmDal localDal = null;
        public rcintasmBll()
        {
            localDal = new rcintasmDal();
            baseDal = localDal;
        }

        public void CreateRecruitmentAssessment(trcintasm obj ,List<trcintadt> listDtl)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DoInsert<trcintasm>(obj);

                    for (int i = 0; i < listDtl.Count; i++)
                    {
                        DoInsert<trcintadt>(listDtl[i]);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateRecruitmentAssessment(trcintasm obj, List<trcintadt> listDtl,List<string> listDeleted)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<ColumnInfo> lstParameters = new List<ColumnInfo>()
                    {
                        new ColumnInfo(){ColumnName="iacd",ColumnValue=obj.iacd}
                    };

                    DoUpdate<trcintasm>(obj,lstParameters);

                    for (int i = 0; i < listDeleted.Count; i++)
                    {
                        List<ColumnInfo> _parameters = new List<ColumnInfo>()
                            {
                                new ColumnInfo(){ColumnName ="iacd",ColumnValue=obj.iacd},
                                new ColumnInfo(){ColumnName ="sqno",ColumnValue=listDeleted[i].ToString(),ColumnType="int"}
                            };

                        new BaseBll().DoDelete<trcintadt>(_parameters);
                    }

                    //updated & inserted
                    for (int i = 0; i < listDtl.Count; i++)
                    {
                        List<ColumnInfo> _parameters = new List<ColumnInfo>()
                            {
                                new ColumnInfo(){ColumnName ="iacd",ColumnValue=obj.iacd},
                                new ColumnInfo(){ColumnName ="sqno",ColumnValue=listDtl[i].sqno.ToString(),ColumnType="int"}
                            };

                        List<trcintadt> lstObj = GetSelectedRecords<trcintadt>(_parameters);

                        if (lstObj.Count > 0)
                            DoUpdate<trcintadt>(listDtl[i], _parameters);
                        else
                            DoInsert<trcintadt>(listDtl[i]);
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
                    DoMultiDelete<trcintadt>(_parameter);

                    new BaseBll().DoDelete<trcintasm>(_parameter);

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
