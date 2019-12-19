using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.Model.HRMS;
using GotWell.Model.Common;
using GotWell.HRMS.HRMSData.Personal;
using GotWell.Common;
using System.Transactions;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class pshctvalBll : BaseBll
    {
        pshctvalDal dal = null;

        public pshctvalBll()
        {
            dal = new pshctvalDal();
            baseDal = dal;
        }

        public List<List<ColumnInfo>> GetHeadCountCfgValue(string hccd, tpshctcfg cfg,List<ValueInfo> lstX, List<ValueInfo> lstY)
        {
            List<List<ColumnInfo>> lstRet = new List<List<ColumnInfo>>();

            List<tpshctval> lstHct = dal.GetHeadCountCfgValue(hccd);

            for (int y = 0; y < lstY.Count; y++)
            {
                List<ColumnInfo> lstColumn = new List<ColumnInfo>();

                ColumnInfo col = new ColumnInfo() { ColumnName = "Item", ColumnValue = lstY[y].DisplayField };
                lstColumn.Add(col);

                col = new ColumnInfo() { ColumnName = "ItemValue", ColumnValue = lstY[y].ValueField,ColumnDisplayName="Hidden" };
                lstColumn.Add(col);

                //默认全部指定为0
                for (int x = 0; x < lstX.Count; x++)
                {
                    ColumnInfo col1 = new ColumnInfo() { ColumnName = lstX[x].DisplayField, ColumnValue = "0" ,ColumnDisplayName=lstX[x].ValueField};
                    lstColumn.Add(col1);
                }

                //填充已指定的值
                var q = (from p in lstHct
                        where p.yval == lstY[y].ValueField
                        orderby p.xval ascending
                        select p).ToList();

                foreach (tpshctval val in q)
                {
                    for (int i = 0; i < lstColumn.Count; i++)
                    {
                        if (val.xval == lstColumn[i].ColumnDisplayName)
                            lstColumn[i].ColumnValue = val.hcnt.ToString();
                    }
                }

                lstRet.Add(lstColumn);
            }

            return lstRet;
        }

        /// <summary>
        /// Updates the specified LST parameter.
        /// </summary>
        /// <param name="lstParameter">The LST parameter.</param>
        /// <param name="lstDtl">The LST DTL.</param>
        /// <Remarks>
        /// Created Time: 2009-1-19 15:24
        /// Created By: Administrator
        /// Last Modified Time:  
        /// Last Modified By: 
        /// </Remarks>
        public void Update(List<ColumnInfo> lstParameter, List<CoordinateInfo> lstDtl)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //delete old
                    DoMultiDelete<tpshctval>(lstParameter);

                    //Insert
                    for (int i = 0; i < lstDtl.Count; i++)
                    {
                        tpshctval val = new tpshctval();
                        val.hccd = lstParameter[0].ColumnValue;
                        val.xval = lstDtl[i].X;
                        val.yval = lstDtl[i].Y;
                        val.hcnt = Convert.ToDouble(lstDtl[i].V);
                        val.lmtm = DateTime.Now;
                        val.lmur = Function.GetCurrentUser();

                        DoInsert<tpshctval>(val);
                    }

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(string hccd)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<ColumnInfo> lstParameter = new List<ColumnInfo>()
                    {
                        new ColumnInfo(){ColumnName="hccd",ColumnValue=hccd}
                    };

                    //delete old
                    DoMultiDelete<tpshctval>(lstParameter);

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
