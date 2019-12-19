using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSCore;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using System.Collections;
using GotWell.HRMS.HRMSData.Reporting;

namespace GotWell.HRMS.HRMSBusiness.Reporting
{
    public class rphctctlBll : BaseBll
    {
        rphctctlDal localDal = null;
        public rphctctlBll()
        {
            localDal = new rphctctlDal();
            baseDal = localDal;
        }

        public string GetData(string hccd,tpshctcfg cfg, tstdefcfg xdef, tstdefcfg ydef, List<ValueInfo> lstX, List<ValueInfo> lstY)
        {
            StringBuilder sb = new StringBuilder();

            List<List<ColumnInfo>> lstRet = new List<List<ColumnInfo>>();

            for (int y = 0; y < lstY.Count; y++)
            {
                #region Plan
                List<ColumnInfo> lstColumn = new List<ColumnInfo>();

                ColumnInfo col = new ColumnInfo() { ColumnName = "Item", ColumnValue = lstY[y].DisplayField };
                lstColumn.Add(col);

                col = new ColumnInfo() { ColumnName = "ItemValue", ColumnValue = lstY[y].ValueField, ColumnDisplayName = "Hidden" };
                lstColumn.Add(col);

                col = new ColumnInfo() { ColumnName = "Type", ColumnValue = "Plan" };
                lstColumn.Add(col);
                //默认全部指定为0
                for (int x = 0; x < lstX.Count; x++)
                {
                    ColumnInfo col1 = new ColumnInfo() { ColumnName = lstX[x].DisplayField, ColumnValue = "0", ColumnDisplayName = lstX[x].ValueField };
                    lstColumn.Add(col1);
                }

                lstRet.Add(lstColumn);
                #endregion

                #region Actual
                lstColumn = new List<ColumnInfo>();

                col = new ColumnInfo() { ColumnName = "Item", ColumnValue = lstY[y].DisplayField };
                lstColumn.Add(col);

                col = new ColumnInfo() { ColumnName = "ItemValue", ColumnValue = lstY[y].ValueField, ColumnDisplayName = "Hidden" };
                lstColumn.Add(col);

                col = new ColumnInfo() { ColumnName = "Type", ColumnValue = "Actual" };
                lstColumn.Add(col);
                //默认全部指定为0
                for (int x = 0; x < lstX.Count; x++)
                {
                    ColumnInfo col1 = new ColumnInfo() { ColumnName = lstX[x].DisplayField, ColumnValue = "0", ColumnDisplayName = lstX[x].ValueField };
                    lstColumn.Add(col1);
                }

                lstRet.Add(lstColumn);
                #endregion

                #region Difference
                lstColumn = new List<ColumnInfo>();

                col = new ColumnInfo() { ColumnName = "Item", ColumnValue = lstY[y].DisplayField };
                lstColumn.Add(col);

                col = new ColumnInfo() { ColumnName = "ItemValue", ColumnValue = lstY[y].ValueField, ColumnDisplayName = "Hidden" };
                lstColumn.Add(col);

                col = new ColumnInfo() { ColumnName = "Type", ColumnValue = "Difference" };
                lstColumn.Add(col);
                //默认全部指定为0
                for (int x = 0; x < lstX.Count; x++)
                {
                    ColumnInfo col1 = new ColumnInfo() { ColumnName = lstX[x].DisplayField, ColumnValue = "0", ColumnDisplayName = lstX[x].ValueField };
                    lstColumn.Add(col1);
                }

                lstRet.Add(lstColumn);
                #endregion
            }

            #region Fill actual value
            for (int i = 0; i < lstY.Count; i++)
            {
                List<ValueInfo> lstActualValue = localDal.GetActualValue(xdef, ydef, lstY[i].ValueField);
                List<ValueInfo> lstPlanValue = localDal.GetActualValue(hccd,xdef, ydef, lstY[i].ValueField);

                //fill actual value
                for (int s=0;s<lstActualValue.Count;s++)
                {
                    for (int n = 1; n < lstRet.Count; n = n + 3)
                    {
                        for (int m = 3; m < lstRet[n].Count; m++)
                        {
                            if (lstRet[n][m].ColumnDisplayName == lstActualValue[s].DisplayField &&
                                lstY[i].ValueField == lstRet[n][1].ColumnValue)
                            {
                                //actual
                                lstRet[n][m].ColumnValue = lstActualValue[s].ValueField;
                            }
                        }
                    }
                }

                //fill plan value
                if (lstPlanValue.Count > 0)
                {
                    for (int n = 0; n < lstRet.Count; n = n + 3)
                    {
                        for (int m = 3; m < lstRet[n].Count; m++)
                        {
                            if (lstRet[n][m].ColumnDisplayName == lstPlanValue[m-3].DisplayField &&
                                lstY[i].ValueField == lstRet[n][1].ColumnValue)
                            {
                                //plan
                                lstRet[n][m].ColumnValue = lstPlanValue[m-3].ValueField;


                                //difference
                                lstRet[n + 2][m].ColumnValue = (Convert.ToInt32(lstRet[n][m].ColumnValue) -
                                    Convert.ToInt32(lstRet[n + 1][m].ColumnValue)).ToString();
                            }
                        }
                    }
                }
            }
            #endregion

            #region Build XML
            sb.Append("<Data>");
            for (int i = 0; i < lstRet.Count; i++)
            {
                int total = 0;

                sb.Append("<Row>");
                sb.Append("<Item>").Append(lstRet[i][0].ColumnValue).Append("</Item>");
                sb.Append("<Type>").Append(lstRet[i][2].ColumnValue).Append("</Type>");
                for (int j = 0; j < lstX.Count; j++)
                {
                    sb.Append("<" + lstX[j].DisplayField + ">").Append(lstRet[i][3 + j].ColumnValue).Append("</" + lstX[j].DisplayField + ">");
                    total += Convert.ToInt32(lstRet[i][3 + j].ColumnValue);
                }

                //total
                sb.Append("<Total>" + total.ToString() + "</Total>") ;

                sb.Append("</Row>");
            }
            sb.Append("</Data>");

            return sb.ToString();
            #endregion
        }

    }
}
