using GotWell.Model.HRMS;
using GotWell.Model.Common;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Data.Linq;
using System.Data;
using GotWell.Utility;
using System;
using System.Reflection;
using GotWell.Common;

namespace GotWell.HRMS.HRMSCore
{
    public class BaseDal
    {
        public string ConnectionString = GotWell.Common.Parameter.HRMS_CONNECTION_STRING;
        public HRMSDataContext gDB = null;

        public BaseDal()
        {
            gDB = new HRMSDataContext(ConnectionString);
        }

        #region Other Methods
        protected string GetColumnValue(string _columnName, List<ColumnInfo> _parameter)
        {
            var q = (from p in _parameter
                    where p.ColumnName == _columnName
                    select p.ColumnValue).ToList();

            string s = string.Empty;
            for (int i = 0; i < q.Count; i++)
            {
                s += (s == string.Empty ? "" : "|") + q[i];
            }

            return s;
        }

        public string BuildWhereSql(string _criterias, List<ColumnInfo> _parameter)
        {
            string sSql = "(1=1)";
            string[] arrPara = _criterias.Split(',');
            for (int i = 0; i < arrPara.Length; i++)
            {
                string value = GetColumnValue(arrPara[i], _parameter);
                if (value.Trim() != string.Empty)
                {
                    if (arrPara[i].IndexOf("|") > 0)
                    {
                        string[] arrField = arrPara[i].Split('|');

                        if (arrField[0] == "from")
                        {
                            sSql += sSql.Trim() == string.Empty ? "" : " and ";
                            if (arrField[1]=="lmtm")
                                sSql += "(" + arrField[1] + ".Value >=DateTime.Parse(\"" + value + "\"))";
                            else
                                sSql += "(" + arrField[1] + " >=DateTime.Parse(\"" + value + "\"))";
                        }
                        else if (arrField[0] == "to")
                        {
                            sSql += sSql.Trim() == string.Empty ? "" : " and ";
                            if (arrField[1] == "lmtm")
                                sSql += "(" + arrField[1] + ".Value <DateTime.Parse(\"" + value + "\").AddDays(1))";
                            else
                                sSql += "(" + arrField[1] + " <DateTime.Parse(\"" + value + "\").AddDays(1))";
                        }
                    }
                    else
                    {

                        string[] arrOriValue = value.Split('|');
                        for (int n = 0; n < arrOriValue.Length; n++)
                        {
                            string[] arrValue = arrOriValue[n].Split(',');

                            sSql += sSql.Trim() == string.Empty ? "" : " and (";
                            for (int j = 0; j < arrValue.Length; j++)
                            {
                                if (arrPara[i] == "emno")
                                {
                                    sSql += (j == 0 ? "" : " or ") + "(" + arrPara[i] + ".Equals(\"" + arrValue[j] + "\"))";
                                }
                                else
                                {
                                    sSql += (j == 0 ? "" : " or ") + "(" + (arrPara[i] == "stfn" ? "ntnm" : arrPara[i]) + ".Contains(\"" + arrValue[j] + "\"))";
                                }
                            }
                            sSql += ") ";
                        }
                    }

                }
            }

            return sSql;
        }

        protected DateTime GetDateTime(string _type, string _value)
        {
            DateTime d = DateTime.Now;
            if (_type == "from")
            {
                d = _value.Trim() == string.Empty ? Function.GetNullDateTime() : Convert.ToDateTime(_value);
            }

            if (_type == "to")
            {
                d = _value.Trim() == string.Empty ? Function.GetMaxDateTime() : Convert.ToDateTime(_value);
            }

            return d;
        }

        private Type GetDynamicType(string _type)
        {
            return Type.GetType("GotWell.Model.HRMS." + _type + ",GotWell.Model");
        }

        public virtual int? GetMaxsqno(string tableName, string emno)
        {
            IEnumerable<int?> ret = gDB.ExecuteQuery<int?>("select max(sqno) as sqno from " + tableName + " where emno=" + emno);

            return ret.Single<int?>();
        }

        public virtual int? GetMaxNo(string tableName, string field)
        {
            IEnumerable<int?> ret = gDB.ExecuteQuery<int?>("select max(" + field + ") as sqno from " + tableName + " where 1=1");

            return ret.Single<int?>();
        }

        public virtual int? GetMaxsqno(List<ColumnInfo> _parameters)
        {
            return 0;
        }

        #endregion

        #region Get Selected Object
        public virtual T GetSelectedObject<T>(List<ColumnInfo> _parameter) where T : class
        {
            try
            {

                Expression expr = BuildExpression<T>(_parameter, true);
                IQueryable<T> query = gDB.GetTable(typeof(T)).AsQueryable()
                    .Provider.CreateQuery<T>(expr);

                T obj = null;

                if (query.ToList<T>().Count > 0)
                    obj = query.ToList<T>().Single<T>();

                return obj;

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

        public virtual object GetSelectedObject(string _type, List<ColumnInfo> _parameter)
        {
            try
            {
                Type type = GetDynamicType(_type);

                Expression expr = BuildExpression(_type, _parameter, true);
                IQueryable query = gDB.GetTable(type).AsQueryable().Provider.CreateQuery(expr);

                object obj = query.Cast<object>().Single<object>();

                return obj;

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
        #endregion

        #region Get Selected Records
        public virtual List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter) where T : class
        {
            //用于模糊查询
            int total = 0;
            return GetSelectedRecords<T>(_parameter, false, 0, 0, ref total, false);
        }

        public virtual List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool isQueryExact) where T : class
        {
            //指定是精确查询还是模糊查询
            int total = 0;
            return GetSelectedRecords<T>(_parameter, false, 0, 0, ref total, isQueryExact);
        }

        public virtual List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount) where T : class
        {
            //模糊查询
            try
            {
                Expression expr = BuildExpression<T>(_parameter, false);
                IQueryable<T> query = gDB.GetTable(typeof(T)).AsQueryable()
                    .Provider.CreateQuery<T>(expr);

                totalRecordCount = query.Count();

                List<T> appList = null;

                if (paging)
                    appList = query.Skip(start).Take(num).ToList<T>();
                else
                    appList = query.ToList<T>();

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

        public virtual List<T> GetSelectedRecords<T>(List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount, bool isQueryExact) where T : class
        {
            try
            {
                Expression expr = BuildExpression<T>(_parameter, isQueryExact);
                IQueryable<T> query = gDB.GetTable(typeof(T)).AsQueryable()
                    .Provider.CreateQuery<T>(expr);

                totalRecordCount = query.Count();

                List<T> appList = null;

                if (paging)
                    appList = query.Skip(start).Take(num).ToList<T>();
                else
                    appList = query.ToList<T>();

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

        public virtual List<object> GetSelectedRecords(string _type, List<ColumnInfo> _parameter)
        {
            int total = 0;
            return GetSelectedRecords(_type, _parameter, false, 0, 0, ref total, false);
        }

        public virtual List<object> GetSelectedRecords(string _type, List<ColumnInfo> _parameter, bool isQueryExact)
        {
            int total = 0;
            return GetSelectedRecords(_type, _parameter, false, 0, 0, ref total, isQueryExact);
        }

        public virtual List<object> GetSelectedRecords(string _type, List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount)
        {
            //模糊查询
            try
            {
                Type type = GetDynamicType(_type);

                Expression expr = BuildExpression(_type, _parameter, false);
                IQueryable query = gDB.GetTable(type).AsQueryable().Provider.CreateQuery(expr);

                totalRecordCount = query.Cast<object>().Count();

                List<object> appList = null;

                if (paging)
                    appList = query.Cast<object>().Skip(start).Take(num).ToList<object>();
                else
                    appList = query.Cast<object>().ToList<object>();

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

        public virtual List<object> GetSelectedRecords(string _type, List<ColumnInfo> _parameter, bool paging, int start, int num, ref int totalRecordCount, bool isQueryExact)
        {
            try
            {

                Type type = GetDynamicType(_type);

                Expression expr = BuildExpression(_type, _parameter, isQueryExact);
                IQueryable query = gDB.GetTable(type).AsQueryable().Provider.CreateQuery(expr);

                totalRecordCount = query.Cast<object>().Count();

                List<object> appList = null;

                if (paging)
                    appList = query.Cast<object>().Skip(start).Take(num).ToList<object>();
                else
                    appList = query.Cast<object>().ToList<object>();

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

        #endregion

        #region BuildExpression
        public virtual Expression BuildExpression<T>(List<ColumnInfo> _parameter, bool isQueryExact) where T : class
        {
            ParameterExpression pExpr = Expression.Parameter(typeof(T), "c");
            string methodName = isQueryExact ? "Equals" : "Contains";
            Expression condition = Expression.Constant(true);
            foreach (ColumnInfo col in _parameter)
            {
                if (col.ColumnValue.Trim().Equals(string.Empty))
                    continue;

                Expression con = null;

                switch (col.ColumnType)
                {
                    case "string":
                        con = Expression.Call(
                             Expression.Property(pExpr, typeof(T).GetProperty(col.ColumnName)),
                             typeof(string).GetMethod(methodName, new Type[] { typeof(string) }),
                             Expression.Constant(col.ColumnValue));
                        break;
                    case "datetime":
                        string[] arr = col.ColumnName.Split('|');
                        if (arr[0] == "from")
                        {
                            Expression left = Expression.Property(pExpr, typeof(T).GetProperty(arr[1]));
                            Expression right = Expression.Constant(Convert.ToDateTime(col.ColumnValue));
                            con = Expression.GreaterThanOrEqual(left, right);
                        }
                        else if (arr[0] == "to")
                        {
                            Expression left = Expression.Property(pExpr, typeof(T).GetProperty(arr[1]));
                            Expression right = Expression.Constant(Convert.ToDateTime(col.ColumnValue));
                            con = Expression.LessThanOrEqual(left, right);
                        }
                        else
                        {
                            Expression left = Expression.Property(pExpr, typeof(T).GetProperty(arr[0]));
                            Expression right = Expression.Constant(Convert.ToDateTime(col.ColumnValue));
                            if (isQueryExact)
                                con = Expression.Equal(left, right);
                            else
                                con = Expression.Equal(left, right);
                        }
                        break;
                    case "float":
                        con = Expression.Call(
                             Expression.Property(pExpr, typeof(T).GetProperty(col.ColumnName)),
                             typeof(decimal).GetMethod("Equals", new Type[] { typeof(decimal) }),
                             Expression.Constant(Convert.ToDecimal(col.ColumnValue)));
                        break;
                    case "int":
                        con = Expression.Call(
                             Expression.Property(pExpr, typeof(T).GetProperty(col.ColumnName)),
                             typeof(Int32).GetMethod("Equals", new Type[] { typeof(Int32) }),
                             Expression.Constant(Convert.ToInt32(col.ColumnValue)));
                        break;
                    default:
                        con = Expression.Call(
                             Expression.Property(pExpr, typeof(T).GetProperty(col.ColumnName)),
                             typeof(string).GetMethod(methodName, new Type[] { typeof(string) }),
                             Expression.Constant(col.ColumnValue));
                        break;
                }
                condition = Expression.And(con, condition);
            }

            var iQry = gDB.GetTable<T>();

            Expression pred =
                Expression.Lambda(condition, new ParameterExpression[] { pExpr });

            Expression whereExpr = Expression.Call(typeof(Queryable), "Where",
                new Type[] { typeof(T) },
                Expression.Constant(iQry), pred);

            return whereExpr;
        }

        public virtual Expression BuildExpression(string _type, List<ColumnInfo> _parameter, bool isQueryExact)
        {
            Type type = GetDynamicType(_type);
            ParameterExpression pExpr = Expression.Parameter(type, "c");
            string methodName = isQueryExact ? "Equals" : "Contains";
            Expression condition = Expression.Constant(true);
            foreach (ColumnInfo col in _parameter)
            {
                if (col.ColumnValue.Trim().Equals(string.Empty))
                    continue;

                Expression con = null;

                switch (col.ColumnType)
                {
                    case "string":
                        con = Expression.Call(
                             Expression.Property(pExpr, type.GetProperty(col.ColumnName)),
                             typeof(string).GetMethod(methodName, new Type[] { typeof(string) }),
                             Expression.Constant(col.ColumnValue));
                        break;
                    case "datetime":
                        string[] arr = col.ColumnName.Split('|');
                        if (arr[0] == "from")
                        {
                            Expression left = Expression.Property(pExpr, type.GetProperty(arr[1]));
                            Expression right = Expression.Constant(Convert.ToDateTime(col.ColumnValue));
                            con = Expression.GreaterThanOrEqual(left, right);
                        }
                        else if (arr[0] == "to")
                        {
                            Expression left = Expression.Property(pExpr, type.GetProperty(arr[1]));
                            Expression right = Expression.Constant(Convert.ToDateTime(col.ColumnValue));
                            con = Expression.LessThanOrEqual(left, right);
                        }
                        else
                        {
                            Expression left = Expression.Property(pExpr, type.GetProperty(arr[0]));
                            Expression right = Expression.Constant(Convert.ToDateTime(col.ColumnValue));
                            if (isQueryExact)
                                con = Expression.Equal(left, right);
                            else
                                con = Expression.GreaterThanOrEqual(left, right);
                        }
                        break;
                    case "float":
                        con = Expression.Call(
                             Expression.Property(pExpr, type.GetProperty(col.ColumnName)),
                             typeof(decimal).GetMethod("Equals", new Type[] { typeof(decimal) }),
                             Expression.Constant(Convert.ToDecimal(col.ColumnValue)));
                        break;
                    case "int":
                        con = Expression.Call(
                             Expression.Property(pExpr, type.GetProperty(col.ColumnName)),
                             typeof(Int32).GetMethod("Equals", new Type[] { typeof(Int32) }),
                             Expression.Constant(Convert.ToInt32(col.ColumnValue)));
                        break;
                    default:
                        con = Expression.Call(
                             Expression.Property(pExpr, type.GetProperty(col.ColumnName)),
                             typeof(string).GetMethod(methodName, new Type[] { typeof(string) }),
                             Expression.Constant(col.ColumnValue));
                        break;
                }
                condition = Expression.And(con, condition);
            }

            var iQry = gDB.GetTable(type);

            Expression pred =
                Expression.Lambda(condition, new ParameterExpression[] { pExpr });

            Expression whereExpr = Expression.Call(typeof(Queryable), "Where",
                new Type[] { type },
                Expression.Constant(iQry), pred);

            return whereExpr;
        }
        #endregion

        #region Update
        public virtual void DoUpdate<T>(T obj) where T : class
        {
            try
            {
                gDB.SubmitChanges();

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

        public virtual void DoUpdate(object obj, List<ColumnInfo> _parameter)
        {
            try
            {
                Type type = obj.GetType();//GetDynamicType(obj);

                object oldObj = GetSelectedObject(type.FullName, _parameter);

                foreach (PropertyInfo prop in type.GetProperties())
                {
                    if ((prop.PropertyType.IsValueType) || (prop.PropertyType.FullName == "System.String"))
                    {
                        object oldValue = type.GetProperty(prop.Name).GetValue(oldObj, null);
                        object newValue = type.GetProperty(prop.Name).GetValue(obj, null);

                        if (oldValue == null)
                        {
                            prop.SetValue(oldObj, newValue, null);
                        }
                        else
                        {
                            if (!oldValue.Equals(newValue))
                                prop.SetValue(oldObj, newValue, null);
                        }
                    }
                }

                gDB.SubmitChanges();

            }
            catch (UtilException ex)
            {
                throw new UtilException(ex.Message, ex.Code, ex);
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }
        #endregion

        #region Insert
        public virtual void DoInsert<T>(T obj) where T : class
        {
            try
            {
                gDB.GetTable<T>().InsertOnSubmit(obj);
                gDB.SubmitChanges();

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

        public virtual void DoInsert(string _type, object obj)
        {
            try
            {
                Type type = GetDynamicType(_type);

                gDB.GetTable(type).InsertOnSubmit(obj);
                gDB.SubmitChanges();

            }
            catch (UtilException ex)
            {
                throw new UtilException(ex.Message, ex.Code, ex);
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }

        #endregion

        #region Delete
        public virtual void DoDelete<T>(T obj) where T : class
        {
            try
            {
                gDB.GetTable<T>().DeleteOnSubmit(obj);
                gDB.SubmitChanges();

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

        public virtual void DoMultiDelete<T>(List<ColumnInfo> _parameter) where T : class
        {
            try
            {
                Expression expr = BuildExpression<T>(_parameter, true);
                IQueryable<T> query = gDB.GetTable(typeof(T)).AsQueryable()
                    .Provider.CreateQuery<T>(expr);

                List<T> objList = query.ToList<T>();

                for (int i = 0; i < objList.Count; i++)
                {
                    gDB.GetTable<T>().DeleteOnSubmit(objList[i]);
                }

                gDB.SubmitChanges();

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

        public virtual void DoDelete(object oldObj)
        {
            try
            {
                gDB.GetTable(oldObj.GetType()).DeleteOnSubmit(oldObj);
                gDB.SubmitChanges();

            }
            catch (UtilException ex)
            {
                throw new UtilException(ex.Message, ex.Code, ex);
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }

        public virtual void DoDelete(string _type, List<ColumnInfo> _parameter)
        {
            try
            {
                //Expression expr = BuildExpression(_type,_parameter, true, new List<ColumnInfo>() { });
                //IQueryable<T> query = gDB.GetTable(typeof(T)).AsQueryable()
                //    .Provider.CreateQuery<T>(expr);

                Type type = GetDynamicType(_type);
                object oldObj = GetSelectedObject(_type, _parameter);

                gDB.GetTable(type).DeleteOnSubmit(oldObj);
                gDB.SubmitChanges();

            }
            catch (UtilException ex)
            {
                throw new UtilException(ex.Message, ex.Code, ex);
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }

        public virtual void DoMultiDelete(string _type, List<ColumnInfo> _parameter)
        {
            try
            {
                //Expression expr = BuildExpression<T>(_parameter, true, new List<ColumnInfo>() { });
                //IQueryable<T> query = gDB.GetTable(typeof(T)).AsQueryable()
                //    .Provider.CreateQuery<T>(expr);
                Type type = GetDynamicType(_type);

                List<object> objList = GetSelectedRecords(_type, _parameter);

                for (int i = 0; i < objList.Count; i++)
                {
                    gDB.GetTable(type).DeleteOnSubmit(objList[i]);
                }

                gDB.SubmitChanges();

            }
            catch (UtilException ex)
            {
                throw new UtilException(ex.Message, ex.Code, ex);
            }
            catch (Exception ex)
            {
                throw new UtilException(ex.Message, ex);
            }
        }

        #endregion

        #region Execute Raw SQL
        public List<T> ExecuteRawSQLQuery<T>(string sSql) where T : class
        {
            IEnumerable<T> lstRet = gDB.ExecuteQuery<T>(sSql);

            return lstRet.ToList();
        }

        public DataSet ExecuteRawSQLQuery(string sSql)
        {
            SqlHelper.ConnectionString = gDB.Connection.ConnectionString;

            return SqlHelper.ExecuteQuery(sSql, new ParameterItem[] { });
        }
        #endregion
    }
}
