using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace GotWell.Utility
{
    public class DBInstance
    {
        public SqlConnection Connection
        {
            get;
            set;
        }

        public SqlTransaction Transaction
        {
            get;
            set;
        }


        public DBInstance()
        {
            try
            {
                Connection = OracleHelper.OpenConnection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }   

        public SqlTransaction BeginTransaction()
        {
            try
            {
                if (!this.IsConnectionOpen())
                {
                    this.Connection.Open();
                }
                Transaction = Connection.BeginTransaction();

                return Transaction;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Commit()
        {
            if (Transaction != null)
                Transaction.Commit();
        }

        public void Rollback()
        {
            if (Transaction !=null)
                Transaction.Rollback();
        }

        public void CloseConnection()
        {
            try
            {
                if (IsConnectionOpen())
                    Connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsConnectionOpen()
        {
            return (Connection.State == ConnectionState.Open);
        }
    }
}
