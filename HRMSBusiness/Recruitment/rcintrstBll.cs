using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Recruitment;
using System.Transactions;
using GotWell.Model.HRMS;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Recruitment
{
    public class rcintrstBll : BaseBll
    {
        rcintrstDal localDal = null;
        public rcintrstBll()
        {
            localDal = new rcintrstDal();
            baseDal = localDal;
        }

        public void CreateInterviewResult(trcintrst obj, List<trcintrdt> listDtl)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DoInsert<trcintrst>(obj);
                    for (int i = 0; i < listDtl.Count; i++)
                    {
                        DoInsert<trcintrdt>(listDtl[i]);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateInterviewResult(trcintrst obj, List<ColumnInfo> parameters, List<trcintrdt> listDtl)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<ColumnInfo> lstParameter = new List<ColumnInfo>()
                    {
                        new ColumnInfo(){ColumnName ="isno",ColumnValue=obj.isno}
                    };

                    List<trcintrst> lstObj = GetSelectedRecords<trcintrst>(lstParameter);
                    if (lstObj.Count > 0)
                        DoUpdate<trcintrst>(obj, lstParameter);
                    else
                        DoInsert<trcintrst>(obj);

                    for (int i = 0; i < listDtl.Count; i++)
                    {
                        List<ColumnInfo> _parameters = new List<ColumnInfo>()
                            {
                                new ColumnInfo(){ColumnName ="isno",ColumnValue=obj.isno},
                                new ColumnInfo(){ColumnName ="sqno",ColumnValue=listDtl[i].sqno.ToString(),ColumnType="int"}
                            };

                        new BaseBll().DoDelete<trcintrdt>(_parameters);
                        DoInsert<trcintrdt>(listDtl[i]);
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
                    localDal.DoMultiDelete<trcintrdt>(_parameter);

                    DoDelete<trcintrst>(_parameter);

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
