using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Reporting;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Reporting
{
    public class rpexrpdfBll : BaseBll
    {
        rpexrpdfDal localDal = null;
        public rpexrpdfBll()
        {
            localDal = new rpexrpdfDal();
            baseDal = localDal;
        }

        public List<trpexrpdd> GetReportDetails(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            //return dal.GetReportDetails(_parameter, paging, start, num, ref totalRecordCount);
            return new BaseBll().GetSelectedRecords<trpexrpdd>(_parameter, paging, start, num, ref totalRecordCount);
        }

        public void InsertReport(trpexrpdf obj, List<trpexrpdd> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //insert
                    DoInsert<trpexrpdf>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<trpexrpdd>(list[i]);
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

        public void UpdateReport(trpexrpdf obj, List<trpexrpdd> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rpcd", ColumnValue = obj.rpcd } };

                    //DoMultiDelete<tatrosdtl>(parameters);
                    List<trpexrpdd> oldList = GetSelectedRecords<trpexrpdd>(parameters);

                    for (int i = 0; i < oldList.Count; i++)
                    {
                        DoDelete<trpexrpdd>(oldList[i]);
                    }

                    //update
                    DoUpdate<trpexrpdf>(obj, parameters);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<trpexrpdd>(list[i]);
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


        public void DeleteReport(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    trpexrpdf obj = GetSelectedObject<trpexrpdf>(_parameters);

                    DoMultiDelete<trpexrpdd>(_parameters);

                    DoDelete<trpexrpdf>(obj);

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