using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace RestAPIs.Providers
{
    public class DataProvider
    {
        private static string _sqlConn = Models.ApplicationGlobalVariables.Instance.ConnectionString;

        public static string ConnectionString
        {
            get
            {
                return _sqlConn;
            }
            set
            {
                _sqlConn = value;
            }
        }


        public static DataTable ExecuteTable(string cmdText)
        {
            var dtTable = new DataTable();
            try
            {
                using (var sqlConnection = new SqlConnection())
                {
                    string sCon = ConnectionString;
                    sqlConnection.ConnectionString = sCon;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                    sqlAdapter.SelectCommand = sqlCommand;
                    sqlAdapter.Fill(dtTable);
                    sqlConnection.Close();
                    sqlAdapter.Dispose();
                    sqlConnection.Dispose();
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return dtTable;
        }


        public static object ExecuteScalar(string cmdText)
        {
            try
            {

                using (var sqlConnection = new SqlConnection())
                {
                    sqlConnection.ConnectionString = ConnectionString;
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                    object value = sqlCommand.ExecuteScalar();
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                    return value;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }


        public static DataTable GetDataTable(string spName)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {

                        da.SelectCommand = new SqlCommand(spName, conn);
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.Fill(dt);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return dt;
        }


        public static DataTable GetDataTable(string spName, IDictionary<string, string> Params)
        {
            var datatable = new DataTable();
            try
            {
                using (var objConnection = new SqlConnection(ConnectionString))
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = objConnection;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = spName;

                        foreach (var parameterName in Params.Keys)
                        {
                            string strParamName;
                            if (!parameterName.StartsWith("@"))
                                strParamName = "@" + parameterName;
                            else
                                strParamName = parameterName;
                            cmd.Parameters.AddWithValue(strParamName, Params[parameterName]);
                        }

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            // Fill the DataSet using default values for DataTable names, etc
                            da.Fill(datatable);
                            
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return datatable;
        }




        public static DataSet GetDataSet(string spName)
        {
            var ds = new DataSet();
            using (var conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    using (var da = new SqlDataAdapter())
                    {
                        da.SelectCommand = new SqlCommand(spName, conn);
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        da.Fill(ds);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return ds;
        }


        public static DataSet GetDataSet(string spName, IDictionary<string, string> Params)
        {
            var ds = new DataSet();
            try
            {
                using (var objConnection = new SqlConnection(ConnectionString))
                {
                    // objConnection.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = objConnection;

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = spName;

                        foreach (var parameterName in Params.Keys)
                        {
                            string strParamName;
                            if (!parameterName.StartsWith("@"))
                                strParamName = "@" + parameterName;
                            else
                                strParamName = parameterName;
                            cmd.Parameters.AddWithValue(strParamName, Params[parameterName]);
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // Fill the DataSet using default values for DataTable names, etc
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return ds;
        }
    }
}