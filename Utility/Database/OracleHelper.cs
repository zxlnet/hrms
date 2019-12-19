using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using GotWell.Common;


namespace GotWell.Utility
{

    /// <summary>
    /// A helper class used to execute queries against an Oracle database
    /// </summary>
    public abstract class OracleHelper {

        // Read the connection strings from the configuration file
        public static string ConnectionString = "";

        //Create a hashtable for the parameter cached
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());


        public static SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);

            connection.Open();

            return connection;
        }
        /// <summary>
        /// Execute a database query which does not include a select
        /// </summary>
        /// <param name="connString">Connection string to database</param>
        /// <param name="cmdType">Command type either stored procedure or SQL</param>
        /// <param name="cmdText">Acutall SQL Command</param>
        /// <param name="commandParameters">Parameters to bind to the command</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters) {
            // Create a new Oracle command
            SqlCommand cmd = new SqlCommand();

            //Create a connection
            using (SqlConnection connection = new SqlConnection(ConnectionString)) {

                //Prepare the command
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);

                //Execute the command
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a database query which does not include a select
        /// </summary>
        /// <param name="connString">Connection string to database</param>
        /// <param name="cmdType">Command type either stored procedure or SQL</param>
        /// <param name="cmdText">Acutall SQL Command</param>
        /// <param name="commandParameters">Parameters to bind to the command</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string cmdText, params SqlParameter[] commandParameters)
        {
            // Create a new Oracle command
            SqlCommand cmd = new SqlCommand();

            //Create a connection
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {

                //Prepare the command
                PrepareCommand(cmd, connection, null, CommandType.Text, cmdText, commandParameters);

                //Execute the command
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }


        /// <summary>
        /// Execute an SqlCommand (that returns no resultset) against an existing database transaction 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders", new SqlParameter(":prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing database transaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters) {

            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
            catch
            {
                throw;
            }
            finally
            {
                //trans.Connection.Close();
            }

        }

        /// <summary>
        /// Execute an SqlCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter(":prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters) {

            try
            {
                SqlCommand cmd = new SqlCommand();

                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
            catch
            {
                throw;
            }
            finally
            {
                //connection.Close();
            }
        }

        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        /// <param name="connString">Connection string</param>
        //// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(UtilLog log,string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters) {

            //Create the command and connection
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            log.LogInfoWithLevel(connectionString, Log_LoggingLevel.Admin);
            try {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                //Execute the query, stating that the connection should close when the resulting datareader has been read
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                conn.Close();
                throw;
            }
        }

        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            //Create the command and connection
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                //Execute the query, stating that the connection should close when the resulting datareader has been read
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string cmdText, params SqlParameter[] commandParameters)
        {

            //Create the command and connection
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(ConnectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);

                //Execute the query, stating that the connection should close when the resulting datareader has been read
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(string cmdText, params SqlParameter[] commandParameters)
        {

            //Create the command and connection
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(ConnectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                //Execute the query, fill dataset
                DataSet ds = new DataSet();
                adp.Fill(ds);
                cmd.Parameters.Clear();
                return ds;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Executes the query within the connection you assign
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <Remarks>
        public static DataSet ExecuteQuery(string connectionString,CommandType cmdType,string cmdText, params SqlParameter[] commandParameters)
        {

            //Create the command and connection
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn,null, cmdType, cmdText, commandParameters);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                //Execute the query, fill dataset
                DataSet ds = new DataSet();
                adp.Fill(ds);
                cmd.Parameters.Clear();
                return ds;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="cmdType">Type of the CMD.</param>
        /// <param name="cmdText">The CMD text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <Remarks>
        public static DataSet ExecuteQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {

            //Create the command and connection
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(ConnectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                //Execute the query, fill dataset
                DataSet ds = new DataSet();
                adp.Fill(ds);
                cmd.Parameters.Clear();
                return ds;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                throw;
            }
            finally
            {
                conn.Close();
            }
        }
        
        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an hashtable of OracleParamters used to execute the command</param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(string cmdText, ParameterItem[] commandParameters)
        {

            //Create the command and connection
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(ConnectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                //Execute the query, fill dataset
                DataSet ds = new DataSet();
                adp.Fill(ds);
                cmd.Parameters.Clear();
                return ds;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an hashtable of OracleParamters used to execute the command</param>
        /// <returns></returns>
        public static DataSet ExecuteQuery(SqlConnection connection,SqlTransaction transaction, string cmdText, SqlParameter[] commandParameters)
        {

            //Create the command and connection
            SqlCommand cmd = new SqlCommand();

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, connection, transaction, CommandType.Text, cmdText, commandParameters);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);

                //Execute the query, fill dataset
                DataSet ds = new DataSet();
                adp.Fill(ds);
                cmd.Parameters.Clear();
                return ds;

            }
            catch
            {

                //If an error occurs close the connection as the reader will not be used and we expect it to close the connection
                throw;
            }
            finally
            {
                //connection.Close();
            }
        }

        /// <summary>
        /// Execute an SqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter(":prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connectionString)) {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

		///	<summary>
		///	Execute	a SqlCommand (that returns a 1x1 resultset)	against	the	specified SqlTransaction
		///	using the provided parameters.
		///	</summary>
		///	<param name="transaction">A	valid SqlTransaction</param>
		///	<param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		///	<param name="commandText">The stored procedure name	or PL/SQL command</param>
		///	<param name="commandParameters">An array of	OracleParamters used to execute the command</param>
		///	<returns>An	object containing the value	in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters) {
			if(transaction == null)
				throw new ArgumentNullException("transaction");
			if(transaction != null && transaction.Connection == null)
				throw new ArgumentException("The transaction was rollbacked	or commited, please	provide	an open	transaction.", "transaction");

			// Create a	command	and	prepare	it for execution
			SqlCommand cmd = new SqlCommand();

			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

			// Execute the command & return	the	results
			object retval = cmd.ExecuteScalar();

			// Detach the SqlParameters	from the command object, so	they can be	used again
			cmd.Parameters.Clear();
			return retval;
		}

        /// <summary>
        /// Execute an SqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter(":prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or PL/SQL command</param>
        /// <param name="commandParameters">an array of OracleParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(SqlConnection connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters) {
            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, connectionString, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Add a set of parameters to the cached
        /// </summary>
        /// <param name="cacheKey">Key value to look up the parameters</param>
        /// <param name="commandParameters">Actual parameters to cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters) {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// Fetch parameters from the cache
        /// </summary>
        /// <param name="cacheKey">Key to look up the parameters</param>
        /// <returns></returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey) {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            // If the parameters are in the cache
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            // return a copy of the parameters
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// Internal function to prepare a command for execution by the database
        /// </summary>
        /// <param name="cmd">Existing command object</param>
        /// <param name="conn">Database connection object</param>
        /// <param name="trans">Optional transaction object</param>
        /// <param name="cmdType">Command type, e.g. stored procedure</param>
        /// <param name="cmdText">Command test</param>
        /// <param name="commandParameters">Parameters for the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters) {

            //Open the connection if required
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //Set up the command
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            //Bind it to the transaction if it exists
            if (trans != null)
                cmd.Transaction = trans;

            // Bind the parameters passed in
            if (commandParameters != null) {
                foreach (SqlParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// Internal function to prepare a command for execution by the database
        /// </summary>
        /// <param name="cmd">Existing command object</param>
        /// <param name="conn">Database connection object</param>
        /// <param name="trans">Optional transaction object</param>
        /// <param name="cmdType">Command type, e.g. stored procedure</param>
        /// <param name="cmdText">Command test</param>
        /// <param name="commandParameters">Parameters for the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, ParameterItem[] commandParameters)
        {

            //Open the connection if required
            if (conn.State != ConnectionState.Open)
                conn.Open();

            //Set up the command
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;

            //Bind it to the transaction if it exists
            if (trans != null)
                cmd.Transaction = trans;

            // Bind the parameters passed in
            if (commandParameters != null)
            {
                foreach (ParameterItem item in commandParameters)
                {
                    if (item != null)   //ADD BY Andy_liu  2007-3-12 14:33:48 
                    {
                        if (item.ParameterName != null && item.ParameterValue != null)
                        {
                            SqlParameter parm = new SqlParameter(item.ParameterName, item.ParameterValue);
                            cmd.Parameters.Add(parm);
                        }
                    }

                }
            }
        }
        
        
        /// <summary>
		/// Converter to use boolean data type with Oracle
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <returns></returns>
		public static string OraBit(bool value) {
			if(value)
				return "Y";
			else
				return "N";
		}

		/// <summary>
		/// Converter to use boolean data type with Oracle
		/// </summary>
		/// <param name="value">Value to convert</param>
		/// <returns></returns>
		public static bool OraBool(string value) {
			if(value.Equals("Y"))
				return true;
			else
				return false;
		} 
    }
}
