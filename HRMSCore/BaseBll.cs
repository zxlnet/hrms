using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using GotWell.Model.Common;
using System.Collections.Generic;
using GotWell.Utility;
using GotWell.Model.HRMS;
using System.Transactions;
using GotWell.Common;
using GotWell.HRMS.HRMSCore.DataControl;
using GotWell.HRMS.HRMSCore.MessageControl;
using System.Reflection;
using GotWell.HRMS.HRMSCore.LogControl;

namespace GotWell.HRMS.HRMSCore
{
    public class BaseBll
    {
        public BaseDal baseDal = null;
        public static List<tstreccfg> lstRecCfg = null;

        public BaseBll()
        {
            baseDal = new BaseDal();

            if (lstRecCfg == null)
            {
                lstRecCfg = GetSelectedRecords<tstreccfg>(new List<ColumnInfo>() { });
            }
        }

        #region Get Selected Record
        public virtual List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount) where T : class
        {
            try
            {
                return baseDal.GetSelectedRecords<T>(_parameter, paging, start, num, ref totalRecordCount);
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

        public virtual List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount, bool isQueryExact) where T : class
        {
            try
            {
                return baseDal.GetSelectedRecords<T>(_parameter, paging, start, num, ref totalRecordCount,isQueryExact);
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

        public virtual List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter) where T : class
        {
            try
            {
                return baseDal.GetSelectedRecords<T>(_parameter);
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

        public virtual List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool isQueryExact) where T : class
        {
            try
            {
                return baseDal.GetSelectedRecords<T>(_parameter, isQueryExact);
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

        #region For Master Data
        public virtual List<object> GetSelectedRecords(string _type, List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount, bool isQueryExact)
        {
            try
            {
                return baseDal.GetSelectedRecords(_type, _parameter, paging, start, num, ref totalRecordCount,isQueryExact);
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

        public virtual List<object> GetSelectedRecords(string _type, List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            try
            {
                return baseDal.GetSelectedRecords(_type, _parameter, paging, start, num, ref totalRecordCount);
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

        public virtual List<object> GetSelectedRecords(string _type, List<ColumnInfo> _parameter)
        {
            try
            {
                return baseDal.GetSelectedRecords(_type, _parameter);
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

        public virtual List<object> GetSelectedRecords(string _type, List<ColumnInfo> _parameter, bool isQueryExact)
        {
            try
            {
                return baseDal.GetSelectedRecords(_type, _parameter, isQueryExact);
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

        #endregion
        #endregion

        #region Get Selected Object
        public virtual T GetSelectedObject<T>(List<ColumnInfo> _parameter) where T : class
        {
            try
            {
                return baseDal.GetSelectedObject<T>(_parameter);
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

        public virtual object GetSelectedObject(string _type, List<ColumnInfo> _parameter)
        {
            try
            {
                return baseDal.GetSelectedObject(_type, _parameter);
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

        #endregion

        #region Delete
        public virtual void DoDelete<T>(List<ColumnInfo> _parameter) where T : class
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    T obj = GetSelectedObject<T>(_parameter);
                    if (obj!=null)
                    {
                        PropertyInfo prop = obj.GetType().GetProperty("tstrecst");

                        if (prop != null)
                        {
                            tstrecst recst = prop.GetValue(obj, null) as tstrecst;
                            if (recst.ownr!=Function.GetCurrentUser())
                            {
                                throw new UtilException("You have no permission to delete it.", null);
                            }

                            //DoDelete<tstrecst>(recst);
                        }

                        DoDelete<T>(obj);

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
            finally
            {
            }
        }

        public virtual void DoDelete<T>(T obj) where T : class
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    string lgtx = new stactlogBll().WriteLog("Delete", obj, null);

                    if (lstRecCfg.Where(p => p.tbnm == typeof(T).Name).ToList().Count > 0)
                    {
                        //需要管理记录状态
                        tstalarm alarmMdl = null;
                        new strecstsBll().UpdateRecStatusToObject(obj, "Delete", lgtx, out alarmMdl);
                        baseDal.DoInsert<tstalarm>(alarmMdl);
                    }

                    baseDal.DoDelete<T>(obj);
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
            finally
            {
            }
        }

        public virtual void DoMultiDelete<T>(List<ColumnInfo> _parameter) where T : class
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //baseDal.DoMultiDelete<T>(_parameter);
                    List<T> lstObj = GetSelectedRecords<T>(_parameter,true);
                    for (int i=0;i<lstObj.Count;i++)
                    {
                        T obj = lstObj[i];
                        PropertyInfo prop = obj.GetType().GetProperty("tstrecst");

                        if (prop != null)
                        {
                            tstrecst recst = prop.GetValue(obj, null) as tstrecst;
                            if (recst.ownr != Function.GetCurrentUser())
                            {
                                throw new UtilException("You have no permission to delete it.", null);
                            }

                            //DoDelete<tstrecst>(recst);
                        }

                        DoDelete<T>(obj);

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
            finally
            {
            }
        }

        public virtual void DoDelete(string _type, List<ColumnInfo> _parameter)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<object> lstObj = GetSelectedRecords(_type, _parameter);
                    if (lstObj.Count > 0)
                    {
                        object obj = lstObj.Single<object>();
                        PropertyInfo prop = obj.GetType().GetProperty("tstrecst");

                        if (prop != null)
                        {
                            tstrecst recst = prop.GetValue(obj, null) as tstrecst;
                            if (recst.ownr != Function.GetCurrentUser())
                            {
                                throw new UtilException("You have no permission to delete it.", null);
                            }

                            DoDelete<tstrecst>(recst);
                        }

                        //DoDelete(_type, obj);
                        baseDal.DoDelete(obj);

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
            finally
            {
            }
        }
        #endregion

        #region Insert
        public virtual void DoInsert<T>(T obj) where T : class
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    PropertyInfo prop = obj.GetType().GetProperty("rfid");
                    if (prop != null)
                        prop.SetValue(obj, Function.GetGUID(), null);

                    string lgtx = new stactlogBll().WriteLog("Add", obj, null);

                    if (lstRecCfg.Where(p => p.tbnm == typeof(T).Name).ToList().Count > 0)
                    {
                        //需要管理记录状态
                        tstalarm alarmMdl = null;
                        new strecstsBll().AddRecStatusToObject(obj, "Add",lgtx, out alarmMdl);
                        baseDal.DoInsert<tstalarm>(alarmMdl);
                    }

                    ReplaceEmptyWithNull(obj);

                    baseDal.DoInsert<T>(obj);
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
            finally
            {
            }
        }

        public virtual void DoInsert(string _type, object obj)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    PropertyInfo prop = obj.GetType().GetProperty("rfid");
                    if (prop != null)
                        prop.SetValue(obj, Function.GetGUID(), null);

                    string lgtx = new stactlogBll().WriteLog("Add", obj, null);

                    if (lstRecCfg.Where(p => p.tbnm == obj.GetType().Name).ToList().Count > 0)
                    {
                        //需要管理记录状态
                        tstalarm alarmMdl = null;
                        new strecstsBll().AddRecStatusToObject(obj, "Add", lgtx, out alarmMdl);
                        baseDal.DoInsert<tstalarm>(alarmMdl);
                    }

                    ReplaceEmptyWithNull(obj);

                    baseDal.DoInsert(_type, obj);
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
            finally
            {
            }
        }

        #endregion

        #region Update
        public virtual void DoUpdate<T>(T obj, List<ColumnInfo> _parameter) where T : class
        {
            try
            {
                bool updateRecStatus = false;

                using (TransactionScope scope = new TransactionScope())
                {
                    if (lstRecCfg.Where(p => p.tbnm == typeof(T).Name).ToList().Count > 0)
                    {
                        //需要管理记录状态
                        updateRecStatus = true;
                    }

                    T oldObj = GetSelectedObject<T>(_parameter);

                    string lgtx = new stactlogBll().WriteLog("Update", obj, oldObj);

                    foreach (PropertyInfo prop in typeof(T).GetProperties())
                    {
                        if ((prop.PropertyType.IsValueType) || (prop.PropertyType.FullName == "System.String"))
                        {
                            object oldValue = typeof(T).GetProperty(prop.Name).GetValue(oldObj, null);
                            object newValue = typeof(T).GetProperty(prop.Name).GetValue(obj, null);

                            if (oldValue == null)
                            {
                                prop.SetValue(oldObj, newValue, null);
                            }
                            else
                            {
                                if (!oldValue.Equals(newValue) && newValue != null)
                                    prop.SetValue(oldObj, newValue, null);
                            }
                        }
                    }

                    ReplaceEmptyWithNull(oldObj);

                    if (updateRecStatus)
                    {
                        PropertyInfo prop = oldObj.GetType().GetProperty("tstrecst");
                        if (prop != null)
                        {
                            tstalarm alarmMdl = null;
                            new strecstsBll().UpdateRecStatusToObject(oldObj, "Update",lgtx, out alarmMdl);
                            baseDal.DoInsert<tstalarm>(alarmMdl);
                        }
                    }

                    baseDal.DoUpdate<T>(obj);
                    scope.Complete();
                }
            }
            catch (UtilException ex)
            {
                throw new UtilException(ex.Message, ex.Code, ex);
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
            finally
            {
            }
        }

        public virtual void DoUpdate<T>(T obj) where T : class
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    baseDal.DoUpdate<T>(obj);
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
            finally
            {
            }
        }

        public virtual void DoUpdate(string _type, object obj, List<ColumnInfo> _parameter)
        {
            try
            {
                bool updateRecStatus = false;

                using (TransactionScope scope = new TransactionScope())
                {
                    if (lstRecCfg.Where(p => p.tbnm == obj.GetType().Name).ToList().Count > 0)
                    {
                        //需要管理记录状态
                        updateRecStatus = true;
                    }

                    object oldObj = GetSelectedObject(_type, _parameter);

                    new stactlogBll().WriteLog("Update", obj, oldObj);

                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if ((prop.PropertyType.IsValueType) || (prop.PropertyType.FullName == "System.String"))
                        {
                            object oldValue = obj.GetType().GetProperty(prop.Name).GetValue(oldObj, null);
                            object newValue = obj.GetType().GetProperty(prop.Name).GetValue(obj, null);

                            if (oldValue == null)
                            {
                                prop.SetValue(oldObj, newValue, null);
                            }
                            else
                            {
                                if (!oldValue.Equals(newValue) && newValue != null)
                                    prop.SetValue(oldObj, newValue, null);
                            }
                        }
                    }

                    ReplaceEmptyWithNull(oldObj);

                    if (updateRecStatus)
                    {
                        PropertyInfo prop = oldObj.GetType().GetProperty("tstrecst");
                        if (prop != null)
                        {
                            tstrecst rec = prop.GetValue(oldObj, null) as tstrecst;
                            rec.actn = "Update";
                            rec.attm = DateTime.Now;
                            rec.atur = Function.GetCurrentUser();
                            rec.stus = "Unconfirmed";
                        }
                    }

                    DoUpdate<object>(obj);
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
            finally
            {
            }
        }

        #endregion

        #region Other Methods
        public virtual void ReplaceEmptyWithNull(object _obj)
        {
            foreach (PropertyInfo props in _obj.GetType().GetProperties())
            {
                if ((props.PropertyType.IsValueType) || (props.PropertyType.FullName == "System.String"))
                {
                    object v = props.GetValue(_obj, null);
                    if ((v != null) && (v.ToString() == string.Empty))
                    {
                        v = null;
                        props.SetValue(_obj, v, null);
                    }
                }
            }

        }

        public virtual int? GetMaxsqno(string tableName, string emno)
        {
            return baseDal.GetMaxsqno(tableName, emno);
        }

        public virtual int? GetMaxsqno(List<ColumnInfo> _parameters)
        {
            return baseDal.GetMaxsqno(_parameters);
        }

        public virtual int? GetMaxNo(string tableName, string field)
        {
            return baseDal.GetMaxNo(tableName, field);
        }

        public virtual void ApplyTo(List<vw_employment> _emps, object obj)
        {
            //Not implementation
        }

        public vw_employment GetEmploymee(string _emno)
        {
            List<ColumnInfo> paras = new List<ColumnInfo>() { new ColumnInfo() { ColumnName = "emno", ColumnValue = _emno } };
            return GetSelectedObject<vw_employment>(paras);
        }

        #endregion

        #region Execute Raw SQL
        public List<T> ExecuteRawSQLQuery<T>(string sSql) where T : class
        {
            return baseDal.ExecuteRawSQLQuery<T>(sSql);
        }

        public DataSet ExecuteRawSQLQuery(string sSql) 
        {
            return baseDal.ExecuteRawSQLQuery(sSql);
        }

        #endregion
    }
}
