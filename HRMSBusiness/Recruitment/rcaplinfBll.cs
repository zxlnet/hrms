using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Recruitment;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Recruitment
{
    public class rcaplinfBll : BaseBll
    {
        rcaplinfDal localDal = null;
        public rcaplinfBll()
        {
            localDal = new rcaplinfDal();
            baseDal = localDal;
        }

        public void CreateApplicant(trcaplinf obj, List<trcaplfml> lstFamily, List<trcapledu> lstEducation, List<trcaplexp> lstExperience,
                                                    List<trcapllan> lstLanguage, List<trcaplref> lstReference)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DoInsert<trcaplinf>(obj);

                    for (int i = 0; i < lstFamily.Count; i++)
                    {
                        DoInsert<trcaplfml>(lstFamily[i]);
                    }

                    for (int i = 0; i < lstEducation.Count; i++)
                    {
                        DoInsert<trcapledu>(lstEducation[i]);
                    }

                    for (int i = 0; i < lstExperience.Count; i++)
                    {
                        DoInsert<trcaplexp>(lstExperience[i]);
                    }

                    for (int i = 0; i < lstLanguage.Count; i++)
                    {
                        DoInsert<trcapllan>(lstLanguage[i]);
                    }

                    for (int i = 0; i < lstReference.Count; i++)
                    {
                        DoInsert<trcaplref>(lstReference[i]);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateApplicant(trcaplinf obj, List<trcaplfml> lstFamily, List<trcapledu> lstEducation, List<trcaplexp> lstExperience,
                                            List<trcapllan> lstLanguage, List<trcaplref> lstReference)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<ColumnInfo> lstParameters = new List<ColumnInfo>()
                    {
                        new ColumnInfo(){ColumnName="aino",ColumnValue=obj.aino}
                    };
                    
                    DoUpdate<trcaplinf>(obj);

                    DoMultiDelete<trcaplfml>(lstParameters);
                    DoMultiDelete<trcapledu>(lstParameters);
                    DoMultiDelete<trcaplexp>(lstParameters);
                    DoMultiDelete<trcapllan>(lstParameters);
                    DoMultiDelete<trcaplref>(lstParameters);

                    for (int i = 0; i < lstFamily.Count; i++)
                    {
                        DoInsert<trcaplfml>(lstFamily[i]);
                    }

                    for (int i = 0; i < lstEducation.Count; i++)
                    {
                        DoInsert<trcapledu>(lstEducation[i]);
                    }

                    for (int i = 0; i < lstExperience.Count; i++)
                    {
                        DoInsert<trcaplexp>(lstExperience[i]);
                    }

                    for (int i = 0; i < lstLanguage.Count; i++)
                    {
                        DoInsert<trcapllan>(lstLanguage[i]);
                    }

                    for (int i = 0; i < lstReference.Count; i++)
                    {
                        DoInsert<trcaplref>(lstReference[i]);
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
                    DoMultiDelete<trcaplfml>(_parameter);
                    DoMultiDelete<trcapledu>(_parameter);
                    DoMultiDelete<trcaplexp>(_parameter);
                    DoMultiDelete<trcapllan>(_parameter);
                    DoMultiDelete<trcaplref>(_parameter);

                    new BaseBll().DoDelete<trcaplinf>(_parameter);

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
