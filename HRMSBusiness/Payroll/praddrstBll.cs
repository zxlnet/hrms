using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Payroll;
using System.Transactions;
using GotWell.Utility;
using GotWell.Model.HRMS;
using GotWell.Common;
using GotWell.Model.Common;

namespace GotWell.HRMS.HRMSBusiness.Payroll
{
    public class praddrstBll : BaseBll
    {
        praddrstDal dal = null;

        public praddrstBll()
        {
            dal = new praddrstDal();
            baseDal = dal;
        }

        public void InsertAddRuleSet(List<tpraddrst> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tpraddrst>(list[i]);

                        if (list[i].rlcd.Trim() != string.Empty)
                        {
                            //Insert into tprrstsit
                            tprrstsit sit = new tprrstsit();
                            sit.itcd = list[i].itcd;
                            sit.lmtm = DateTime.Now;
                            sit.lmur = Function.GetCurrentUser();
                            sit.rlcd = list[i].rlcd;

                            DoInsert<tprrstsit>(sit);
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

        public void UpdateAddRuleSet(string _emno, List<tpraddrst> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete first
                    List<ColumnInfo> parameters = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emno } };

                    //delete summary relationship
                    List<tpraddrst> lstOld = GetSelectedRecords<tpraddrst>(parameters);
                    for (int i = 0; i < lstOld.Count; i++)
                    {
                        DoDelete<tprrstsit>(new List<ColumnInfo>() { 
                            new ColumnInfo() { ColumnName = "rlcd", ColumnValue = lstOld[i].rlcd },
                            new ColumnInfo() { ColumnName = "itcd", ColumnValue = lstOld[i].itcd }
                        });
                    }

                    DoMultiDelete<tpraddrst>(parameters);

                    for (int i = 0; i < list.Count; i++)
                    {
                        DoInsert<tpraddrst>(list[i]);

                        if (list[i].rlcd.Trim() != string.Empty)
                        {
                            //Insert into tprrstsit
                            tprrstsit sit = new tprrstsit();
                            sit.itcd = list[i].itcd;
                            sit.lmtm = DateTime.Now;
                            sit.lmur = Function.GetCurrentUser();
                            sit.rlcd = list[i].rlcd;

                            DoInsert<tprrstsit>(sit);
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


        public void DeleteAddRuleSet(List<ColumnInfo> _parameters)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    tpraddrst obj = GetSelectedObject<tpraddrst>(_parameters);

                    //delete summary relationship
                    List<tpraddrst> lstOld = GetSelectedRecords<tpraddrst>(_parameters);
                    for (int i = 0; i < lstOld.Count; i++)
                    {
                        DoDelete<tprrstsit>(new List<ColumnInfo>() { 
                            new ColumnInfo() { ColumnName = "rlcd", ColumnValue = lstOld[i].rlcd },
                            new ColumnInfo() { ColumnName = "itcd", ColumnValue = lstOld[i].itcd }
                        });
                    }

                    DoMultiDelete<tprprirst>(_parameters);

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

        public void ApplyTo(List<vw_employment> _emps, List<tpraddrst> lstObj)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < _emps.Count; i++)
                    {
                        if (_emps[i].emno != lstObj[0].emno)
                        {
                            int? n = GetMaxsqno(new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emps[i].emno }});
                            int maxSqno = 0;

                            if (!n.HasValue) maxSqno = 0; else maxSqno = n.Value;

                            for (int j = 0; j < lstObj.Count; j++)
                            {
                                tpraddrst obj = lstObj[j];

                                tpraddrst newobj = new tpraddrst();

                                newobj.crcd = obj.crcd;
                                newobj.ctfr = obj.ctfr;
                                newobj.emno = _emps[i].emno;
                                newobj.isca = obj.isca;
                                newobj.itcd = obj.itcd;
                                newobj.lmtm = DateTime.Now;
                                newobj.lmur = Function.GetCurrentUser();
                                newobj.pdcd = obj.pdcd;
                                newobj.perd = obj.perd;
                                newobj.remk = obj.remk;
                                newobj.rlcd = obj.rlcd;
                                newobj.sqno = maxSqno + j + 1;
                                newobj.valu = obj.valu;

                                DoInsert<tpraddrst>(newobj);
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