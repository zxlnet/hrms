using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using GotWell.Common;
using GotWell.HRMS.HRMSBusiness.Common;
using GotWell.HRMS.HRMSData;
using GotWell.HRMS.HRMSData.Master;
using GotWell.Model.Common;
using GotWell.Model.HRMS;
using GotWell.Utility;

namespace GotWell.HRMS.HRMSBusiness.Master
{
    public class MasterDataBll : BaseBll
    {
        private string tableName;
        private MasterDataDal dal ;

        public MasterDataBll()
        {
            this.tableName = "";
            dal = new MasterDataDal("");
            baseDal = dal;
        }

        public MasterDataBll(string tableName)
        {
            this.tableName = tableName;
            dal = new MasterDataDal(tableName);
            baseDal = dal;
            //logBll=new TransactionLogBll(dbInstance);
        }

        public List<ColumnMdl> GetColumns()
        {
            return dal.GetColumns();
        }

        public Type GetDynamicType(string _type)
        {
            return Type.GetType("GotWell.Model.HRMS." + _type + ",GotWell.Model");
        }

       public Exception_ErrorMessage DeleteMasterData(string _type,List<List<ColumnInfo>> _PrimaryKeys)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < _PrimaryKeys.Count; i++)
                    {
                        DoDelete(_type, _PrimaryKeys[i]);
                    }
                    scope.Complete();
                }
                return Exception_ErrorMessage.NoError;
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

        public Exception_ErrorMessage InsertMasterData(string _type,List<ColumnInfo> _parameters) 
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Type type = GetDynamicType(_type);

                    object obj = type.GetConstructor(new Type[] { }).Invoke(new object[] { });

                    for (int i = 0; i < _parameters.Count; i++)
                    {
                        PropertyInfo prop = type.GetProperty(_parameters[i].ColumnName);

                        if (prop != null)
                        {
                            prop.SetValue(obj, ConvertByDef(_parameters[i].ColumnType, _parameters[i].ColumnValue),null);

                        }
                    }

                    DoInsert(_type, obj);

                    scope.Complete();
                }
                return Exception_ErrorMessage.NoError;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return Exception_ErrorMessage.NoError;
        }

        public Exception_ErrorMessage InsertFromExcel(string _type, string connectionString, List<ColumnInfo> list)
        {
            try
            {
                Type type = GetDynamicType(_type);

                List<object> lstData = dal.GetRecordsFromExcel(type, connectionString, list);
                using (TransactionScope scope = new TransactionScope())
                {
                    for (int i = 0; i < lstData.Count; i++)
                    {
                        object obj = lstData[i];

                        for (int j=0;j<list.Count;j++)
                        {
                            PropertyInfo props = type.GetProperty(list[j].ColumnName);
                            if (props != null)
                            {
                                object objValue = props.GetValue(obj, null);

                                if ((objValue == null) || (objValue.ToString().Trim() == ""))
                                {
                                    //fill default value
                                    if (list[j].ColumnValue != "")
                                    {
                                        props.SetValue(obj, ConvertByDef(list[j].ColumnType, list[j].ColumnValue), null);
                                    }
                                }
                            }
                        }

                        DoInsert(_type, obj);
                    }

                    scope.Complete();
                    return Exception_ErrorMessage.NoError;
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

        public Exception_ErrorMessage EditMasterData(string _type,List<ColumnInfo> _parameters) 
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Type type = GetDynamicType(_type);

                    object obj = type.GetConstructor(new Type[] { }).Invoke(new object[] { });

                    for (int i = 0; i < _parameters.Count; i++)
                    {
                        PropertyInfo prop = type.GetProperty(_parameters[i].ColumnName);

                        if (prop != null)
                        {
                            prop.SetValue(obj, ConvertByDef(_parameters[i].ColumnType, _parameters[i].ColumnValue),null);
                        }
                    }

                    List<ColumnInfo> keyParameters = new List<ColumnInfo>() { };

                    for (int i = 0; i < _parameters.Count; i++)
                    {
                        if (_parameters[i].IsPrimaryKey == "True")
                        {
                            keyParameters.Add(new ColumnInfo() { ColumnName = _parameters[i].ColumnName, ColumnValue = _parameters[i].ColumnValue });
                        }
                    }

                    DoUpdate(_type, obj, keyParameters);

                    scope.Complete();
                }


                return Exception_ErrorMessage.NoError;
            }
            catch (UtilException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        public static object ConvertByDef(string javascriptType, string value)
        {
            object objValue = null;
            switch (javascriptType)
            {
                case "string":
                    objValue = value;
                    break;
                case "float":
                    objValue = Convert.ToDecimal(value);
                    break;
                case "int":
                    objValue = Convert.ToInt32(value);
                    break;
                case "datetime":
                    objValue = Convert.ToDateTime(value);
                    break;
            }

            return objValue;
        }

 
    }
}
