using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services.SQL
{
    class SqlHelper
    {
        public static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\users.mdf;Integrated Security=True";

        public static bool ColumnExists(string tableName, string columnName)
        {
            object single = GetSingle("select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'");
            if (single == null)
                return false;
            return Convert.ToInt32(single) > 0;
        }

        public static int GetMaxId(string fieldName, string tableName)
        {
            object single = GetSingle("select max(" + fieldName + ")+1 from " + tableName);
            if (single == null)
                return 1;
            return int.Parse(single.ToString());
        }

        public static bool Exists(string strSql)
        {
            object single = GetSingle(strSql);
            return (Equals(single, null) || Equals(single, DBNull.Value) ? 0 : int.Parse(single.ToString())) != 0;
        }

        public static bool TabExists(string tableName)
        {
            object single = GetSingle("select count(*) from sysobjects where id = object_id(N'[" + tableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1");
            return (Equals(single, null) || Equals(single, DBNull.Value) ? 0 : int.Parse(single.ToString())) != 0;
        }

        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            object single = GetSingle(strSql, cmdParms);
            return (Equals(single, null) || Equals(single, DBNull.Value) ? 0 : int.Parse(single.ToString())) != 0;
        }

        public static int ExecuteSql(string sqlString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        return sqlCommand.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        connection.Close();
                        throw;
                    }
                }
            }
        }

        public static int ExecuteSqlByTime(string sqlString, int times)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        sqlCommand.CommandTimeout = times;
                        return sqlCommand.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                        connection.Close();
                        throw;
                    }
                }
            }
        }

        public static int ExecuteSqlTran(List<string> sqlStringList)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
                sqlCommand.Transaction = sqlTransaction;
                try
                {
                    int num = 0;
                    for (int index = 0; index < sqlStringList.Count; ++index)
                    {
                        string sqlString = sqlStringList[index];
                        if (sqlString.Trim().Length > 1)
                        {
                            sqlCommand.CommandText = sqlString;
                            num += sqlCommand.ExecuteNonQuery();
                        }
                    }
                    sqlTransaction.Commit();
                    return num;
                }
                catch
                {
                    sqlTransaction.Rollback();
                    return 0;
                }
            }
        }

        public static int ExecuteSql(string sqlString, string content)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(sqlString, connection);
                SqlParameter sqlParameter = new SqlParameter("@content", SqlDbType.NText);
                sqlParameter.Value = content;
                sqlCommand.Parameters.Add(sqlParameter);
                try
                {
                    connection.Open();
                    return sqlCommand.ExecuteNonQuery();
                }
                finally
                {
                    sqlCommand.Dispose();
                    connection.Close();
                }
            }
        }

        public static object ExecuteSqlGet(string sqlString, string content)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(sqlString, connection);
                SqlParameter sqlParameter = new SqlParameter("@content", SqlDbType.NText);
                sqlParameter.Value = content;
                sqlCommand.Parameters.Add(sqlParameter);
                try
                {
                    connection.Open();
                    object objA = sqlCommand.ExecuteScalar();
                    if (Equals(objA, null) || Equals(objA, DBNull.Value))
                        return null;
                    return objA;
                }
                finally
                {
                    sqlCommand.Dispose();
                    connection.Close();
                }
            }
        }

        public static int ExecuteSqlInsertImg(string strSql, byte[] fs)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(strSql, connection);
                SqlParameter sqlParameter = new SqlParameter("@fs", SqlDbType.Image);
                sqlParameter.Value = fs;
                sqlCommand.Parameters.Add(sqlParameter);
                try
                {
                    connection.Open();
                    return sqlCommand.ExecuteNonQuery();
                }
                finally
                {
                    sqlCommand.Dispose();
                    connection.Close();
                }
            }
        }

        public static object GetSingle(string sqlString)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        object objA = sqlCommand.ExecuteScalar();
                        if (Equals(objA, null) || Equals(objA, DBNull.Value))
                            return null;
                        return objA;
                    }
                    catch (SqlException)
                    {
                        connection.Close();
                        throw;
                    }
                }
            }
        }

        public static object GetSingle(string sqlString, int times)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        sqlCommand.CommandTimeout = times;
                        object objA = sqlCommand.ExecuteScalar();
                        if (Equals(objA, null) || Equals(objA, DBNull.Value))
                            return null;
                        return objA;
                    }
                    catch (SqlException)
                    {
                        connection.Close();
                        throw;
                    }
                }
            }
        }

        public static SqlDataReader ExecuteReader(string strSql)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand sqlCommand = new SqlCommand(strSql, connection);
            connection.Open();
            return sqlCommand.ExecuteReader();
        }

        public static DataSet Query(string sqlString)
        {
            using (SqlConnection selectConnection = new SqlConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                try
                {
                    selectConnection.Open();
                    new SqlDataAdapter(sqlString, selectConnection).Fill(dataSet, "ds");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dataSet;
            }
        }

        public static DataSet Query(string sqlString, int times)
        {
            using (SqlConnection selectConnection = new SqlConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                try
                {
                    selectConnection.Open();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlString, selectConnection);
                    sqlDataAdapter.SelectCommand.CommandTimeout = times;
                    sqlDataAdapter.Fill(dataSet, "ds");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dataSet;
            }
        }

        public static int ExecuteSql(string sqlString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, conn, null, sqlString, cmdParms);
                    int num = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return num;
                }
            }
        }

        public static void ExecuteSqlTran(Hashtable sqlStringList)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        foreach (DictionaryEntry sqlString in sqlStringList)
                        {
                            string cmdText = sqlString.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])sqlString.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public static int ExecuteSqlTran(List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd1 = new SqlCommand();
                    try
                    {
                        int num1 = 0;
                        foreach (CommandInfo cmd2 in cmdList)
                        {
                            string commandText = cmd2.CommandText;
                            SqlParameter[] parameters = (SqlParameter[])cmd2.Parameters;
                            PrepareCommand(cmd1, conn, trans, commandText, parameters);
                            if (cmd2.EffentNextType == EffentNextType.WhenHaveContine || cmd2.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                if (cmd2.CommandText.ToLower().IndexOf("count(", StringComparison.Ordinal) == -1)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                object obj = cmd1.ExecuteScalar();
                                bool flag2 = Convert.ToInt32(obj) > 0;
                                if (cmd2.EffentNextType == EffentNextType.WhenHaveContine && !flag2)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                if (cmd2.EffentNextType == EffentNextType.WhenNoHaveContine && flag2)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                            }
                            else
                            {
                                int num2 = cmd1.ExecuteNonQuery();
                                num1 += num2;
                                if (cmd2.EffentNextType == EffentNextType.ExcuteEffectRows && num2 == 0)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                cmd1.Parameters.Clear();
                            }
                        }
                        trans.Commit();
                        return num1;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void ExecuteSqlTranWithIndentity(List<CommandInfo> sqlStringList)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int num = 0;
                        foreach (CommandInfo sqlString in sqlStringList)
                        {
                            string commandText = sqlString.CommandText;
                            SqlParameter[] parameters = (SqlParameter[])sqlString.Parameters;
                            foreach (SqlParameter sqlParameter in parameters)
                            {
                                if (sqlParameter.Direction == ParameterDirection.InputOutput)
                                    sqlParameter.Value = num;
                            }
                            PrepareCommand(cmd, conn, trans, commandText, parameters);
                            cmd.ExecuteNonQuery();
                            foreach (SqlParameter sqlParameter in parameters)
                            {
                                if (sqlParameter.Direction == ParameterDirection.Output)
                                    num = Convert.ToInt32(sqlParameter.Value);
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void ExecuteSqlTranWithIndentity(Hashtable sqlStringList)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    try
                    {
                        int num = 0;
                        foreach (DictionaryEntry sqlString in sqlStringList)
                        {
                            string cmdText = sqlString.Key.ToString();
                            SqlParameter[] cmdParms = (SqlParameter[])sqlString.Value;
                            foreach (SqlParameter sqlParameter in cmdParms)
                            {
                                if (sqlParameter.Direction == ParameterDirection.InputOutput)
                                    sqlParameter.Value = num;
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            cmd.ExecuteNonQuery();
                            foreach (SqlParameter sqlParameter in cmdParms)
                            {
                                if (sqlParameter.Direction == ParameterDirection.Output)
                                    num = Convert.ToInt32(sqlParameter.Value);
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public static object GetSingle(string sqlString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, conn, null, sqlString, cmdParms);
                    object objA = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if (Equals(objA, null) || Equals(objA, DBNull.Value))
                        return null;
                    return objA;
                }
            }
        }

        public static SqlDataReader ExecuteReader(string sqlString, params SqlParameter[] cmdParms)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, conn, null, sqlString, cmdParms);
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return sqlDataReader;
        }

        public static DataSet Query(string sqlString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                PrepareCommand(sqlCommand, conn, null, sqlString, cmdParms);
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                {
                    DataSet dataSet = new DataSet();
                    try
                    {
                        sqlDataAdapter.Fill(dataSet, "ds");
                        sqlCommand.Parameters.Clear();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return dataSet;
                }
            }
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms == null)
                return;
            foreach (SqlParameter cmdParm in cmdParms)
            {
                if ((cmdParm.Direction == ParameterDirection.InputOutput || cmdParm.Direction == ParameterDirection.Input) && cmdParm.Value == null)
                    cmdParm.Value = DBNull.Value;
                cmd.Parameters.Add(cmdParm);
            }
        }

        public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                return sqlCommand.ExecuteReader();
            }
        }

        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                new SqlDataAdapter
                {
                    SelectCommand = BuildQueryCommand(connection, storedProcName, parameters)
                }.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int times)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDataAdapter.SelectCommand.CommandTimeout = times;
                sqlDataAdapter.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand sqlCommand = new SqlCommand(storedProcName, connection);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            foreach (var dataParameter in parameters)
            {
                var parameter = (SqlParameter)dataParameter;
                if (parameter != null)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
                        parameter.Value = DBNull.Value;
                    sqlCommand.Parameters.Add(parameter);
                }
            }
            return sqlCommand;
        }

        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand sqlCommand = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = sqlCommand.ExecuteNonQuery();
                return (int)sqlCommand.Parameters["ReturnValue"].Value;
            }
        }

        private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand sqlCommand = BuildQueryCommand(connection, storedProcName, parameters);
            sqlCommand.Parameters.Add(new SqlParameter("ReturnValue", SqlDbType.Int, 4, ParameterDirection.ReturnValue, false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return sqlCommand;
        }
    }
}