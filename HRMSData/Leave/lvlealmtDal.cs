using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.LanguageResources;
using GotWell.Model.HRMS;
using GotWell.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Leave
{
    public class lvlealmtDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("ltcd", _parameter);

                var q = from p in gDB.tlvlealmts.Where(sSql1)
                        join s in gDB.tlvleatyps on p.ltcd equals s.ltcd
                        join o in gDB.tstdefcfgs on p.lmby equals o.dfnm
                        select new
                        {
                            p.ltcd,
                            s.ltnm,
                            p.lmtm,
                            p.lmur,
                            p.lmby,
                            p.lmno,
                            p.lmsp,
                            p.lmtx,
                            p.lmva,
                            p.mxch,
                            p.mxlh,
                            p.remk,
                            lmbytext=HRMSRes.ResourceManager.GetString(o.dfrs),
                            p.rfid
                        };

                List<T> obj = q.Cast<T>().Distinct().ToList();

                totalRecordCount = obj.Count;

                List<T> appList = null;

                if (paging)
                    appList = obj.Skip(start).Take(num).ToList();
                else
                    appList = obj;

                return appList;

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


        public double GetWeeklmbyEmp(vw_employment empInfo, string _ltcd, HRMS_Limit_Type _limitType)
        {
            return GetlmbyEmp(empInfo, _ltcd, HRMS_Limit_Scope.Week, _limitType);
        }

        public double GetMonthlmbyEmp(vw_employment empInfo, string _ltcd, HRMS_Limit_Type _limitType)
        {
            return GetlmbyEmp(empInfo, _ltcd, HRMS_Limit_Scope.Month, _limitType);
        }

        public double GetYearlmbyEmp(vw_employment empInfo, string _ltcd, HRMS_Limit_Type _limitType)
        {
            return GetlmbyEmp(empInfo, _ltcd, HRMS_Limit_Scope.Year, _limitType);
        }

        public double GetlmbyEmp(vw_employment empInfo, string _ltcd, HRMS_Limit_Scope _scope, HRMS_Limit_Type _limitType)
        {
            try
            {
                double lmbyEmpHours = -1;
                double lmbyYearHours = -1;
                double lmbyOtherHours = -1;

                double lmbyEmpCarryHours = -1;
                double lmbyYearCarryHours = -1;
                double lmbyOtherCarryHours = -1;

                //针对员工的设定
                var q = (from p in gDB.tlvlealmts
                         where p.lmsp == _scope.ToString()
                         && (p.ltcd == _ltcd || (_ltcd == "ALL"))
                         && p.lmby == HRMS_Limit_By.E.ToString()
                         && p.lmva == empInfo.emno
                         select p).ToList<tlvlealmt>();

                for (int i = 0; i < q.Count; i++)
                {
                    lmbyEmpHours = q[i].mxlh.Value;
                    lmbyEmpCarryHours = q[i].mxch.Value;
                    break;
                }

                //针对服务年数的设定
                q = (from p in gDB.tlvlealmts
                     where p.lmsp == _scope.ToString()
                         && (p.ltcd == _ltcd || (_ltcd == "ALL"))
                         && p.lmby == HRMS_Limit_By.Y.ToString()
                         && p.lmva == empInfo.yearservice.ToString()
                         select p).ToList<tlvlealmt>();

                for (int i = 0; i < q.Count; i++)
                {
                    lmbyYearHours = q[i].mxlh.Value;
                    lmbyYearCarryHours = q[i].mxch.Value;
                    break;
                }

                //针对其他项目的设定

                q = (from p in gDB.tlvlealmts
                         join s in gDB.tstdefcfgs on p.lmby equals s.dfnm
                     where (p.ltcd == _ltcd || (_ltcd == "ALL"))
                          && p.lmby != HRMS_Limit_By.E.ToString() && p.lmby != HRMS_Limit_By.Y.ToString()
                          && p.lmsp == _scope.ToString()
                          //&& p.lmva == empInfo.GetType().GetProperty(s.fieldname).GetValue(empInfo, null).ToString()
                         select p).ToList<tlvlealmt>();

                var q1 = (from p in q
                          where p.lmva == empInfo.GetType().GetProperty(p.tstdefcfg.finm).GetValue(empInfo, null).ToString().Trim()
                          select p).ToList();

                for (int i = 0; i < q1.Count; i++)
                {
                    lmbyOtherHours = q1[i].mxlh.Value;
                    lmbyOtherCarryHours = q1[i].mxch.Value;
                    break;
                }

                double r = 0;
                switch (_limitType)
                {
                    case HRMS_Limit_Type.LeaveHours:
                        r = ((lmbyEmpHours == -1 ? 0 : lmbyEmpHours) + (lmbyYearHours == -1 ? 0 : lmbyYearHours) + (lmbyOtherHours == -1 ? 0 : lmbyOtherHours));
                        break;
                    case HRMS_Limit_Type.LeaveCarryforwardHours:
                        r = ((lmbyEmpCarryHours == -1 ? 0 : lmbyEmpCarryHours) + (lmbyYearCarryHours == -1 ? 0 : lmbyYearCarryHours) + (lmbyOtherCarryHours == -1 ? 0 : lmbyOtherCarryHours));
                        break;
                    default:
                        r = 0;
                        break;
                }

                return (r == 0 ? -1 : r);

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
