using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSData.Common;
using GotWell.Model.Common;
using GotWell.LanguageResources;
using GotWell.Utility;
using GotWell.Model.HRMS;
using GotWell.Common;
using System.Linq.Dynamic;

namespace GotWell.HRMS.HRMSData.Overtime
{
    public class otlimitDal : BaseDal
    {
        public override List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                string sSql1 = BuildWhereSql("otcd", _parameter);

                var q = from p in gDB.totlimits.Where(sSql1)
                        join s in gDB.tottypes on p.otcd equals s.otcd
                        join o in gDB.tstdefcfgs on p.lmby equals o.dfnm
                        select new
                        {
                            p.otcd,
                            s.otnm,
                            p.lmtm,
                            p.lmur,
                            p.lmby,
                            p.lmno,
                            p.lmsc,
                            p.lmtx,
                            p.lmva,
                            p.mxoh,
                            p.mxth,
                            p.remk,
                            lmbytext = HRMSRes.ResourceManager.GetString(o.dfrs)
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


        public double GetWeeklmbyEmp(vw_employment empInfo, string _otcd, HRMS_Limit_Type _limitType)
        {
            return GetlmbyEmp(empInfo, _otcd, HRMS_Limit_Scope.Week, _limitType);
        }

        public double GetMonthlmbyEmp(vw_employment empInfo, string _otcd, HRMS_Limit_Type _limitType)
        {
            return GetlmbyEmp(empInfo, _otcd, HRMS_Limit_Scope.Month, _limitType);
        }

        public double GetYearlmbyEmp(vw_employment empInfo, string _otcd, HRMS_Limit_Type _limitType)
        {
            return GetlmbyEmp(empInfo, _otcd, HRMS_Limit_Scope.Year, _limitType);
        }

        public double GetlmbyEmp(vw_employment empInfo, string _otcd, HRMS_Limit_Scope _scope, HRMS_Limit_Type _limitType)
        {
            try
            {
                double lmbyEmpHours = -1;
                double lmbyYearHours = -1;
                double lmbyOtherHours = -1;

                double lmbyEmpTTLVHours = -1;
                double lmbyYearTTLVHours = -1;
                double lmbyOtherTTLVHours = -1;

                //针对员工的设定
                var q = (from p in gDB.totlimits
                         where p.lmsc == _scope.ToString()
                         && p.otcd == _otcd
                         && p.lmby == HRMS_Limit_By.E.ToString()
                         && p.lmva == empInfo.emno
                         select p).ToList<totlimit>();

                for (int i = 0; i < q.Count; i++)
                {
                    lmbyEmpHours = q[i].mxoh.Value;
                    lmbyEmpTTLVHours = q[i].mxth.Value;
                    break;
                }

                //针对服务年数的设定
                q = (from p in gDB.totlimits
                     where p.lmsc == _scope.ToString()
                     && p.otcd == _otcd
                     && p.lmby == HRMS_Limit_By.Y.ToString()
                     && p.lmva == empInfo.yearservice.ToString()
                     select p).ToList<totlimit>();

                for (int i = 0; i < q.Count; i++)
                {
                    lmbyYearHours = q[i].mxoh.Value;
                    lmbyYearTTLVHours = q[i].mxth.Value;
                    break;
                }

                //针对其他项目的设定

                q = (from p in gDB.totlimits
                     join s in gDB.tstdefcfgs on p.lmby equals s.dfnm
                     where p.otcd == _otcd
                      && p.lmby != HRMS_Limit_By.E.ToString() && p.lmby != HRMS_Limit_By.Y.ToString()
                      && p.lmsc == _scope.ToString()
                     //&& p.lmva == empInfo.GetType().GetProperty(s.fieldname).GetValue(empInfo, null).ToString()
                     select p).ToList<totlimit>();

                var q1 = (from p in q
                          where p.lmva == empInfo.GetType().GetProperty(p.tstdefcfg.finm).GetValue(empInfo, null).ToString().Trim()
                          select p).ToList();

                for (int i = 0; i < q1.Count; i++)
                {
                    lmbyOtherHours = q1[i].mxoh.Value;
                    lmbyOtherTTLVHours = q1[i].mxth.Value;
                    break;
                }

                double r = 0;
                switch (_limitType)
                {
                    case HRMS_Limit_Type.OvertimeHours:
                        r = ((lmbyEmpHours == -1 ? 0 : lmbyEmpHours) + (lmbyYearHours == -1 ? 0 : lmbyYearHours) + (lmbyOtherHours == -1 ? 0 : lmbyOtherHours));
                        break;
                    case HRMS_Limit_Type.OvertimeTTLVHours:
                        r = ((lmbyEmpTTLVHours == -1 ? 0 : lmbyEmpTTLVHours) + (lmbyYearTTLVHours == -1 ? 0 : lmbyYearTTLVHours) + (lmbyOtherHours == -1 ? 0 : lmbyOtherHours));
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
