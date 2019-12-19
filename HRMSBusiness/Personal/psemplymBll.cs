using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData.Personal;
using GotWell.Model.Common;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Reflection;
using System.Transactions;
using GotWell.Common;

namespace GotWell.HRMS.HRMSBusiness.Personal
{
    public class psemplymBll : BaseBll
    {
        psemplymDal localDal = null;
        public psemplymBll()
        {
            localDal = new psemplymDal();
            baseDal = localDal;
        }

        public List<object> GetSelectedRecordsForAdvQry(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount) 
        {
            try
            {
                return localDal.GetSelectedRecordsForAdvQry(_parameter, paging, start, num, ref totalRecordCount);
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

        public List<vw_employment> GetValidEmployment()
        {
            return localDal.GetValidEmployment();
        }

        public void CopyEmpToHistory(tpsemplym _emp,string _changeType)
        {
            tpsemphi his = new tpsemphi();

            foreach (PropertyInfo prop in typeof(tpsemphi).GetProperties())
            {
                if ((prop.PropertyType.IsValueType) || (prop.PropertyType.FullName == "System.String"))
                {
                    PropertyInfo empProps = typeof(tpsemplym).GetProperty(prop.Name);
                    if (empProps != null)
                    {
                        object value = empProps.GetValue(_emp, null);

                        prop.SetValue(his, value, null);
                    }
                }
            }

            int? t = GetMaxsqno("tpsemphis",his.emno);
            int seqno = 0;
            if (t.HasValue) seqno = t.Value; else seqno = 0;

            his.sqno = seqno + 1;
            his.ctcd = _changeType;
            his.isdt = DateTime.Now;
            his.isby = Function.GetCurrentUser();

            DoInsert<tpsemphi>(his);
        }

        public void InsertEmployment(tpsemplym _emp)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                DoInsert<tpsemplym>(_emp);

                //保存历史,New Join
                CopyEmpToHistory(_emp, "sys$0");

                scope.Complete();
            }
        }

        public void UpdateEmployment(tpsemplym _emp,List<ColumnInfo> _parameter,string _action)
        {
            string changeType = _action;

            using (TransactionScope scope = new TransactionScope())
            {
                DoUpdate<tpsemplym>(_emp, _parameter);

                //保存历史,New Join
                if ((changeType != "unknown") && (changeType!=string.Empty)) 
                    CopyEmpToHistory(_emp, changeType);

                scope.Complete();
            }
        }

    }
}
