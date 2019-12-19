using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using GotWell.Common;

namespace GotWell.Utility
{
    /// <summary>
    /// A helper class used to execute queries against an Excel database
    /// </summary>
    public class ExcelHelper
    {
        // Read the connection strings from the configuration file
        public string ConnectionString = "";

        public void SetConnectionString(string _connectionString)
        {
            ConnectionString = _connectionString;
        }

        /// <summary>
        /// Execute a select query that will return a result set
        /// </summary>
        public DataSet ExecuteQuery(string cmdText, ParameterItem[] commandParameters)
        {
            //Create the command and connection
            OleDbCommand cmd = new OleDbCommand();
            OleDbConnection conn = new OleDbConnection(ConnectionString);

            try
            {
                //Prepare the command to execute
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);

                OleDbDataAdapter adp = new OleDbDataAdapter(cmd);

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
        /// Internal function to prepare a command for execution by the database
        /// </summary>
        /// <param name="cmd">Existing command object</param>
        /// <param name="conn">Database connection object</param>
        /// <param name="trans">Optional transaction object</param>
        /// <param name="cmdType">Command type, e.g. stored procedure</param>
        /// <param name="cmdText">Command test</param>
        /// <param name="commandParameters">Parameters for the command</param>
        private void PrepareCommand(OleDbCommand cmd, OleDbConnection conn, OleDbTransaction trans, CommandType cmdType, string cmdText, ParameterItem[] commandParameters)
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
                        OleDbParameter parm = new OleDbParameter(item.ParameterName, item.ParameterValue);
                        cmd.Parameters.Add(parm);
                    }

                }
            }
        }

    }
}
