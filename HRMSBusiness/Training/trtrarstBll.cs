using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Training;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Common;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Training
{
    public class trtrarstBll : BaseBll
    {
        trtrarstDal dal = null;

        public trtrarstBll()
        {
            dal = new trtrarstDal();
            baseDal = dal;
        }

        public List<object> GetTrainingResult(List<ColumnInfo> _parameter)
        {
            return dal.GetTrainingResult(_parameter);
        }

        public void Clear(string trcd)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<ColumnInfo> parameters = new List<ColumnInfo>() {
                            new ColumnInfo() { ColumnName = "trcd", ColumnValue = trcd } 
                        };

                    DoMultiDelete<ttrtrarst>(parameters);

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

        public void UpdateTrainingResult(List<ttrtrarst> list)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {

                    ttrtraing traObj = null;
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].lmtm = DateTime.Now;
                        list[i].lmur = Function.GetCurrentUser();

                        if (traObj == null)
                        {
                            List<ColumnInfo> p1 = new List<ColumnInfo>() {
                                new ColumnInfo() { ColumnName = "trcd", ColumnValue = list[0].trcd } 
                            };
                            traObj = GetSelectedObject<ttrtraing>(p1);
                        }
                        
                        List<ColumnInfo> parameters = new List<ColumnInfo>() {
                            new ColumnInfo() { ColumnName = "trcd", ColumnValue = list[i].trcd } ,
                            new ColumnInfo() { ColumnName = "emno", ColumnValue = list[i].emno } 
                        };

                        ttrtrarst oldObj = GetSelectedObject<ttrtrarst>(parameters);

                        if (oldObj != null)
                        {
                            //update
                            DoUpdate<ttrtrarst>(list[i], parameters);
                        }
                        else
                        {
                            //insert
                            DoInsert<ttrtrarst>(list[i]);
                        }

                        //Certificate
                        if (list[i].isce == "Y")
                        {
                            tpstraing psTraining = new tpstraing();
                            psTraining.bnag = traObj.bnag;
                            psTraining.crcd = traObj.crcd;
                            psTraining.ctpw = traObj.ctpw;
                            psTraining.cufe = traObj.cufe;
                            psTraining.cunm = traObj.cunm;
                            psTraining.emno = list[i].emno;
                            psTraining.endt = traObj.endt;
                            psTraining.incm = traObj.incm;
                            psTraining.lmtm = DateTime.Now;
                            psTraining.lmur = Function.GetCurrentUser();
                            psTraining.loca = traObj.loca;
                            psTraining.natu = traObj.natu;
                            psTraining.orga = traObj.orga;
                            psTraining.paby = traObj.paby;
                            psTraining.redt = traObj.redt;
                            psTraining.remk = traObj.remk;
                            psTraining.sqno = 1;
                            psTraining.stdt = traObj.stdt;
                            psTraining.tmpe = traObj.tmpe;
                            psTraining.trcd = traObj.trcd;
                            psTraining.ttcd = traObj.ttcd;
                            psTraining.tttm = traObj.tttm;

                            tpstraing psOldTraining = GetSelectedObject<tpstraing>(parameters);
                            if (psOldTraining != null)
                            {
                                //Update
                                DoUpdate<tpstraing>(psTraining, parameters);
                            }
                            else
                            {
                                //Insert
                                DoInsert<tpstraing>(psTraining);
                            }
                        }

                        if (list[i].issk == "Y")
                        {
                            //tpsskill.
                        }
                        //Skill
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

        public List<object> GetSkillItems(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            return dal.GetSkillItems(_parameter, paging, start, num, ref totalRecordCount);
        }

    }
}
