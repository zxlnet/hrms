using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using GotWell.HRMS.HRMSData.Personal;
using System.Transactions;
using GotWell.Model.HRMS;
using GotWell.Utility;
using GotWell.Model.Common;
using GotWell.Common;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class prprirstBll : BaseBll
    {
        prprirstDal dal = null;

        public prprirstBll()
        {
            dal = new prprirstDal();
            baseDal = dal;
        }

        public void InsertPriRuleSet(List<tprprirst> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        string rlcd = UtilDatetime.FormatTime4(DateTime.Now);
                        if (list[i].vtyp == "Sum")
                        {
                            list[i].rlcd = rlcd;
                        }
                        DoInsert<tprprirst>(list[i]);

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

        public void UpdatePriRuleSet(string _emno,List<tprprirst> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emno } };
                    BaseBll baseBll = new BaseBll();
                    List<tprprirst> lstDtl = baseBll.GetSelectedRecords<tprprirst>(parameters, true);
                    for (int i = 0; i < lstDtl.Count; i++)
                    {
                        if (lstDtl[i].vtyp == "Sum")
                        {
                            //delete rel item
                            List<ColumnInfo> relparams = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rlcd", ColumnValue = lstDtl[i].rlcd } };
                            DoMultiDelete<tprrstsit>(relparams);
                        }
                        //DoDelete<tprprirst>(lstDtl[i]);
                    }

                    DoMultiDelete<tprprirst>(parameters);

                    for (int i = 0; i < list.Count; i++)
                    {
                        string rlcd = UtilDatetime.FormatTime4(DateTime.Now);
                        if (list[i].vtyp == "Sum")
                        {
                            list[i].rlcd = rlcd;
                        }

                        DoInsert<tprprirst>(list[i]);

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


        public void DeletePriRuleSet(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    BaseBll baseBll = new BaseBll();
                    List<tprprirst> lstDtl = baseBll.GetSelectedRecords<tprprirst>(_parameters);
                    for (int i = 0; i < lstDtl.Count; i++)
                    {
                        if (lstDtl[i].vtyp == "Sum")
                        {
                            //delete rel item
                            List<ColumnInfo> relparams = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rlcd", ColumnValue = lstDtl[i].rlcd } };
                            DoMultiDelete<tprrstsit>(relparams);
                        }

                        DoDelete<tprprirst>(lstDtl[i]);
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

        public void ApplyTo(List<vw_employment> _emps, List<tprprirst> lstObj)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < _emps.Count; i++)
                    {
                        if (_emps[i].emno != lstObj[0].emno)
                        {
                            int? n = GetMaxsqno(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emps[i].emno } });
                            int maxSqno = 0;

                            if (!n.HasValue) maxSqno = 0; else maxSqno = n.Value;

                            for (int j = 0; j < lstObj.Count; j++)
                            {
                                string rlcd = UtilDatetime.FormatTime4(DateTime.Now.AddMilliseconds(100));
                                tprprirst obj = lstObj[j];
                                
                                tprprirst newobj = new tprprirst();

                                newobj.cnfr = obj.cnfr;
                                newobj.cred = obj.cred;
                                newobj.crcd = obj.crcd;
                                newobj.efdt = obj.efdt;
                                newobj.emno = _emps[i].emno;
                                newobj.exdt = obj.exdt;
                                newobj.isca = obj.isca;
                                newobj.itcd = obj.itcd;
                                newobj.lmtm = DateTime.Now;
                                newobj.lmur = Function.GetCurrentUser();
                                newobj.pdcd = obj.pdcd;
                                newobj.sqno = maxSqno + j + 1;
                                newobj.ssty = obj.ssty;
                                newobj.valu = obj.valu;
                                newobj.vtyp = obj.vtyp;
                                newobj.rlcd = rlcd;

                                DoInsert<tprprirst>(newobj);

                                if (newobj.valu=="Sum")
                                {
                                    List<ColumnInfo> relparams = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "rlcd", ColumnValue = obj.rlcd } };
                                    List<tprrstsit> lstRel = GetSelectedRecords<tprrstsit>(relparams);
                                    for (int m=0;m<lstRel.Count;m++)
                                    {
                                        tprrstsit newsit = new tprrstsit();
                                        newsit.itcd = lstRel[m].itcd;
                                        newsit.lmtm = newobj.lmtm;
                                        newsit.lmur = newobj.lmur;
                                        newsit.rlcd = rlcd;

                                        DoInsert<tprrstsit>(newsit);
                                    }
                                }
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
   }
}
