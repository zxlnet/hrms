using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using GotWell.Model.HRMS;
using GotWell.Utility;
using System.Transactions;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prpubrstBll : BaseBll
    {
        prpubrstDal dal = null;

        public prpubrstBll()
        {
            dal = new prpubrstDal();
            baseDal = dal;
        }

        public List<object> GetPubRuleSetDtl(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                return dal.GetPubRuleSetDtl(_parameter, paging, start, num, ref totalRecordCount);
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

        public void InsertPubRuleSet(tprpubrst obj, List<tprprsdtl> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //insert
                    DoInsert<tprpubrst>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        string rlcd = UtilDatetime.FormatTime4(DateTime.Now);
                        if (list[i].vtyp == "Sum")
                        {
                            list[i].rlcd = rlcd;
                        }
                        DoInsert<tprprsdtl>(list[i]);

                        if (list[i].vtyp == "Sum")
                        {
                            string[] arr = list[i].valu.Split(',');
                            for (int j = 0; j < arr.Length; j++)
                            {
                                tprrstsit sit = new tprrstsit();
                                sit.itcd = arr[j].Substring(1, arr[j].Length - 2);
                                sit.lmtm = list[i].lmtm;
                                sit.lmur = list[i].lmur;
                                sit.rlcd = rlcd;

                                DoInsert<tprrstsit>(sit);
                            }
                        }

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

        public void UpdatePubRuleSet(tprpubrst obj, List<tprprsdtl> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rscd", ColumnValue = obj.rscd } };

                    List<tprprsdtl> lstDtl = GetSelectedRecords<tprprsdtl>(parameters);
                    for (int i = 0; i < lstDtl.Count;i++ )
                    {
                        if (lstDtl[i].vtyp=="Sum")
                        {
                            //delete rel item
                            List<ColumnInfo> relparams = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rlcd", ColumnValue = lstDtl[i].rlcd } };
                            DoMultiDelete<tprrstsit>(relparams);
                        }

                        DoDelete<tprprsdtl>(lstDtl[i]);
                    }

                    //update
                    dal.DoUpdate<tprpubrst>(obj);

                    for (int i = 0; i < list.Count; i++)
                    {
                        string rlcd = UtilDatetime.FormatTime4(DateTime.Now);
                        if (list[i].vtyp == "Sum")
                        {
                            list[i].rlcd = rlcd;
                        }

                        dal.DoInsert<tprprsdtl>(list[i]);

                        if (list[i].vtyp=="Sum")
                        {
                            string[] arr = list[i].valu.Split(',');
                            for (int j=0;j<arr.Length;j++)
                            {
                                tprrstsit sit = new tprrstsit();
                                sit.itcd = arr[j].Substring(1, arr[j].Length - 2);
                                sit.lmtm = list[i].lmtm;
                                sit.lmur = list[i].lmur;
                                sit.rlcd = rlcd;

                                DoInsert<tprrstsit>(sit);
                            }
                        }
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


        public void DeletePubRuleSet(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<tprprsdtl> lstDtl = GetSelectedRecords<tprprsdtl>(_parameters);
                    for (int i = 0; i < lstDtl.Count; i++)
                    {
                        if (lstDtl[i].vtyp == "Sum")
                        {
                            //delete rel item
                            List<ColumnInfo> relparams = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rlcd", ColumnValue = lstDtl[i].rlcd } };
                            DoMultiDelete<tprrstsit>(relparams);
                        }

                        DoDelete<tprprsdtl>(lstDtl[i]);
                    }


                    //delete
                    DoDelete<tprpubrst>(_parameters);

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

        public List<object> GetRelItem(string rlcd,string itcd)
        {
            try
            {
                return dal.GetRelItem(rlcd, itcd);
            }
            catch (UtilException ex)
            {
            	throw ex;
            }
            catch(Exception ex)
            {
            	throw new UtilException(ex.Message,ex);
            }
        }
    }
}