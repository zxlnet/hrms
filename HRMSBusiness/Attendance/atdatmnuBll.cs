using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Attendance;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Utility;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Attendance
{
    public class atdatmnuBll : BaseBll
    {
         atdatmnuDal dal = null;

         public atdatmnuBll()
         {
             dal = new atdatmnuDal();
             baseDal = dal;
         }

         public void InsertDataManu(tatdatmnu obj)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     List<ColumnInfo> lstParameter = new List<ColumnInfo>() { 
                    new ColumnInfo(){ColumnName="emno",ColumnValue=obj.emno},
                    new ColumnInfo(){ColumnName="atdt",ColumnValue=obj.atdt.ToString("yyyy-MM-dd HH:mm:ss"),ColumnType="datetime"}
                 };

                     tatanarst anarst = GetSelectedObject<tatanarst>(lstParameter);
                     if (anarst != null)
                     {
                         anarst.itmm = obj.ittm;
                         anarst.otmm = obj.ottm;
                         anarst.betm = obj.betm;
                         anarst.bstm = obj.bstm;
                         anarst.ifma = "Y";
                         anarst.iscf = "N";

                         DoUpdate<tatanarst>(anarst);
                     }

                     DoInsert<tatdatmnu>(obj);

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

         public void UpdateDataManu(tatdatmnu obj)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     List<ColumnInfo> lstParameter = new List<ColumnInfo>() { 
                    new ColumnInfo(){ColumnName="emno",ColumnValue=obj.emno},
                    new ColumnInfo(){ColumnName="atdt",ColumnValue=obj.atdt.ToString("yyyy-MM-dd HH:mm:ss"),ColumnType="datetime"}
                 };
                     tatdatmnu oldObj = GetSelectedObject<tatdatmnu>(lstParameter);

                     if (oldObj.ittm != obj.ittm ||
                         oldObj.ottm != obj.ottm ||
                         oldObj.betm != obj.betm ||
                         oldObj.bstm != obj.bstm)
                     {
                         tatanarst anarst = GetSelectedObject<tatanarst>(lstParameter);
                         if (anarst != null)
                         {
                             anarst.itmm = obj.ittm;
                             anarst.otmm = obj.ottm;
                             anarst.betm = obj.betm;
                             anarst.bstm = obj.bstm;
                             anarst.ifma = "Y";
                             anarst.iscf = "N";

                             DoUpdate<tatanarst>(anarst);
                         }
                     }

                     DoUpdate<tatdatmnu>(obj, lstParameter);

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

         public void DeleteDataManu(List<ColumnInfo> lstParameter)
         {
             try
             {
                 using (TransactionScope scope = new TransactionScope())
                 {
                     tatanarst anarst = GetSelectedObject<tatanarst>(lstParameter);
                     if (anarst != null)
                     {
                         anarst.itmm = null;
                         anarst.otmm = null;
                         anarst.betm = null;
                         anarst.bstm = null;
                         anarst.ifma = "N";
                         anarst.iscf = "N";

                         DoUpdate<tatanarst>(anarst);
                     }

                     DoDelete<tatdatmnu>(lstParameter);

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

        public void SaveDataManu(List<tatdatmnu> _parameters, List<List<ColumnInfo>> lstIscf)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < _parameters.Count; i++)
                    {
                        List<ColumnInfo> parameters = new List<ColumnInfo>() { 
                                    new ColumnInfo(){ColumnName="emno",ColumnValue=_parameters[i].emno},
                                    new ColumnInfo(){ColumnName="atdt",ColumnValue=UtilDatetime.FormatDate1(_parameters[i].atdt),ColumnType="datetime"}};

                        tatdatmnu obj = GetSelectedObject<tatdatmnu>(parameters);

                        if (obj != null)
                            DoDelete<tatdatmnu>(obj);

                        tatdatmnu newobj = new tatdatmnu();
                        newobj.adam = _parameters[i].adam;
                        newobj.ahrm = _parameters[i].ahrm;
                        newobj.atdt = _parameters[i].atdt;
                        newobj.betm = _parameters[i].betm;
                        newobj.bstm = _parameters[i].bstm;
                        newobj.ectm = _parameters[i].ectm;
                        newobj.emno = _parameters[i].emno;
                        newobj.ittm = _parameters[i].ittm;
                        newobj.lmtm = _parameters[i].lmtm;
                        newobj.lmur = _parameters[i].lmur;
                        newobj.lctm = _parameters[i].lctm;
                        newobj.ottm = _parameters[i].ottm;
                        newobj.remk = _parameters[i].remk;
                        newobj.scdm = _parameters[i].scdm;

                        DoInsert<tatdatmnu>(newobj);

                        tatanarst analResult = GetSelectedObject<tatanarst>(parameters);

                        if (analResult != null)
                        {
                            analResult.betm = newobj.betm;
                            analResult.bstm = newobj.bstm;
                            analResult.itmm = newobj.ittm;
                            analResult.otmm = newobj.ottm;
                            analResult.ifma = "Y";
                            analResult.iscf = "N";
                            analResult.remk += newobj.remk;

                            DoUpdate<tatanarst>(analResult);
                        }
                    }

                    for (int i = 0; i < lstIscf.Count; i++)
                    {
                        List<ColumnInfo> parameters = new List<ColumnInfo>() { 
                                    new ColumnInfo(){ColumnName="emno",ColumnValue=lstIscf[i][0].ColumnValue},
                                    new ColumnInfo(){ColumnName="atdt",ColumnValue=lstIscf[i][1].ColumnValue,ColumnType="datetime"}};

                        tatanarst analResult = GetSelectedObject<tatanarst>(parameters);

                        if (analResult != null)
                        {
                            analResult.iscf = lstIscf[i][2].ColumnValue == "True" ? "Y" : "N";

                            DoUpdate<tatanarst>(analResult);
                        }

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
    }
}
