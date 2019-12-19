using Test.Core;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Xml;

namespace System.Data
{
	public sealed class SqlHelper
	{
		public static int CommandTimeout;

		static SqlHelper()
		{
			SqlHelper.CommandTimeout = 30;
		}

		private SqlHelper()
		{
		}

		private static void AssignParameterValues(SqlParameter[] commandParameters, ValueRow dataRow)
		{
			if (commandParameters == null || dataRow == null)
			{
				return;
			}
			int num = 0;
			SqlParameter[] sqlParameterArray = commandParameters;
			for (int i = 0; i < (int)sqlParameterArray.Length; i++)
			{
				SqlParameter item = sqlParameterArray[i];
				if (item.ParameterName == null || item.ParameterName.Length <= 1)
				{
					throw new Exception(string.Format("Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.", num, item.ParameterName));
				}
				if (dataRow.Table.Columns.IndexOf(item.ParameterName.Substring(1)) != -1)
				{
					item.Value = dataRow[item.ParameterName.Substring(1)];
				}
				num++;
			}
		}

		private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
		{
			if (commandParameters == null || parameterValues == null)
			{
				return;
			}
			if ((int)commandParameters.Length != (int)parameterValues.Length)
			{
				throw new ArgumentException("Parameter count does not match Parameter Value count.");
			}
			int num = 0;
			int length = (int)commandParameters.Length;
			while (num < length)
			{
				if (parameterValues[num] is IDbDataParameter)
				{
					IDbDataParameter dbDataParameter = (IDbDataParameter)parameterValues[num];
					if (dbDataParameter.Value != null)
					{
						commandParameters[num].Value = dbDataParameter.Value;
					}
					else
					{
						commandParameters[num].Value = DBNull.Value;
					}
				}
				else if (parameterValues[num] != null)
				{
					commandParameters[num].Value = parameterValues[num];
				}
				else
				{
					commandParameters[num].Value = DBNull.Value;
				}
				num++;
			}
		}

		private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			if (commandParameters != null)
			{
				SqlParameter[] sqlParameterArray = commandParameters;
				for (int i = 0; i < (int)sqlParameterArray.Length; i++)
				{
					SqlParameter value = sqlParameterArray[i];
					if (value != null)
					{
						if ((value.Direction == ParameterDirection.InputOutput || value.Direction == ParameterDirection.Input) && value.Value == null)
						{
							value.Value = DBNull.Value;
						}
						command.Parameters.Add(value);
					}
				}
			}
		}

		public static SqlCommand CreateCommand(SqlConnection connection, string spName, params string[] sourceColumns)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			SqlCommand sqlCommand = new SqlCommand(spName, connection)
			{
				CommandType = System.Data.CommandType.StoredProcedure
			};
			if (sourceColumns != null && sourceColumns.Length != 0)
			{
				SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
				for (int i = 0; i < (int)sourceColumns.Length; i++)
				{
					spParameterSet[i].SourceColumn = sourceColumns[i];
				}
				SqlHelper.AttachParameters(sqlCommand, spParameterSet);
			}
			return sqlCommand;
		}

		public static ValueSet ExecuteDataset(string connectionString, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(connectionString, commandType, commandText, null);
		}

		public static ValueSet ExecuteDataset(string connectionString, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			ValueSet valueSet;
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				valueSet = SqlHelper.ExecuteDataset(sqlConnection, commandType, commandText, commandParameters);
			}
			return valueSet;
		}

		public static ValueSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteDataset(connectionString, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteDataset(connectionString, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static ValueSet ExecuteDataset(SqlConnection connection, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(connection, commandType, commandText, null);
		}

		public static ValueSet ExecuteDataset(SqlConnection connection, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out flag);
			ValueSet valueSet = new ValueSet();
			sqlCommand.ExecuteReader().Fill(valueSet);
			sqlCommand.Parameters.Clear();
			if (flag)
			{
				connection.Close();
			}
			return valueSet;
		}

		public static ValueSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteDataset(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteDataset(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static ValueSet ExecuteDataset(SqlTransaction transaction, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteDataset(transaction, commandType, commandText, null);
		}

		public static ValueSet ExecuteDataset(SqlTransaction transaction, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			SqlHelper.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out flag);
			ValueSet valueSet = new ValueSet();
			sqlCommand.Parameters.Clear();
			return valueSet;
		}

		public static ValueSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteDataset(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteDataset(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static ValueSet ExecuteDatasetTypedParams(string connectionString, string spName, ValueRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteDataset(connectionString, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteDataset(connectionString, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static ValueSet ExecuteDatasetTypedParams(SqlConnection connection, string spName, ValueRow dataRow)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteDataset(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteDataset(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static ValueSet ExecuteDatasetTypedParams(SqlTransaction transaction, string spName, ValueRow dataRow)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteDataset(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteDataset(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static int ExecuteNonQuery(string connectionString, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteNonQuery(connectionString, commandType, commandText, null);
		}

		public static int ExecuteNonQuery(string connectionString, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			int num;
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				num = SqlHelper.ExecuteNonQuery(sqlConnection, commandType, commandText, commandParameters);
			}
			return num;
		}

		public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteNonQuery(connectionString, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteNonQuery(connectionString, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static int ExecuteNonQuery(SqlConnection connection, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteNonQuery(connection, commandType, commandText, null);
		}

		public static int ExecuteNonQuery(SqlConnection connection, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out flag);
			int num = sqlCommand.ExecuteNonQuery();
			sqlCommand.Parameters.Clear();
			if (flag)
			{
				connection.Close();
			}
			return num;
		}

		public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteNonQuery(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteNonQuery(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static int ExecuteNonQuery(SqlTransaction transaction, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteNonQuery(transaction, commandType, commandText, null);
		}

		public static int ExecuteNonQuery(SqlTransaction transaction, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			SqlHelper.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out flag);
			int num = sqlCommand.ExecuteNonQuery();
			sqlCommand.Parameters.Clear();
			return num;
		}

		public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteNonQuery(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteNonQuery(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static int ExecuteNonQueryTypedParams(string connectionString, string spName, ValueRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteNonQuery(connectionString, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteNonQuery(connectionString, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static int ExecuteNonQueryTypedParams(SqlConnection connection, string spName, ValueRow dataRow)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteNonQuery(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteNonQuery(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, string spName, ValueRow dataRow)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteNonQuery(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteNonQuery(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, System.Data.CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlHelper.SqlConnectionOwnership connectionOwnership)
		{
			SqlDataReader sqlDataReader;
			SqlDataReader sqlDataReader1;
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			bool flag = false;
			SqlCommand sqlCommand = new SqlCommand();
			try
			{
				SqlHelper.PrepareCommand(sqlCommand, connection, transaction, commandType, commandText, commandParameters, out flag);
				sqlDataReader = (connectionOwnership != SqlHelper.SqlConnectionOwnership.External ? sqlCommand.ExecuteReader(CommandBehavior.CloseConnection) : sqlCommand.ExecuteReader());
				bool flag1 = true;
				foreach (object parameter in sqlCommand.Parameters)
				{
					if (((SqlParameter)parameter).Direction == ParameterDirection.Input)
					{
						continue;
					}
					flag1 = false;
				}
				if (flag1)
				{
					sqlCommand.Parameters.Clear();
				}
				sqlDataReader1 = sqlDataReader;
			}
			catch
			{
				if (flag)
				{
					connection.Close();
				}
				throw;
			}
			return sqlDataReader1;
		}

		public static SqlDataReader ExecuteReader(string connectionString, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(connectionString, commandType, commandText, null);
		}

		public static SqlDataReader ExecuteReader(string connectionString, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			SqlDataReader sqlDataReader;
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			SqlConnection sqlConnection = null;
			try
			{
				sqlConnection = new SqlConnection(connectionString);
				sqlConnection.Open();
				sqlDataReader = SqlHelper.ExecuteReader(sqlConnection, null, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.Internal);
			}
			catch
			{
				if (sqlConnection != null)
				{
					sqlConnection.Close();
				}
				throw;
			}
			return sqlDataReader;
		}

		public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteReader(connectionString, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteReader(connectionString, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static SqlDataReader ExecuteReader(SqlConnection connection, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(connection, commandType, commandText, null);
		}

		public static SqlDataReader ExecuteReader(SqlConnection connection, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			return SqlHelper.ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.External);
		}

		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteReader(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteReader(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static SqlDataReader ExecuteReader(SqlTransaction transaction, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteReader(transaction, commandType, commandText, null);
		}

		public static SqlDataReader ExecuteReader(SqlTransaction transaction, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			return SqlHelper.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlHelper.SqlConnectionOwnership.External);
		}

		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteReader(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteReader(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static SqlDataReader ExecuteReaderTypedParams(string connectionString, string spName, ValueRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteReader(connectionString, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteReader(connectionString, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, string spName, ValueRow dataRow)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteReader(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteReader(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, string spName, ValueRow dataRow)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteReader(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteReader(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static object ExecuteScalar(string connectionString, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(connectionString, commandType, commandText, null);
		}

		public static object ExecuteScalar(string connectionString, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			object obj;
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				obj = SqlHelper.ExecuteScalar(sqlConnection, commandType, commandText, commandParameters);
			}
			return obj;
		}

		public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteScalar(connectionString, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteScalar(connectionString, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static object ExecuteScalar(SqlConnection connection, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(connection, commandType, commandText, null);
		}

		public static object ExecuteScalar(SqlConnection connection, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out flag);
			object obj = sqlCommand.ExecuteScalar();
			sqlCommand.Parameters.Clear();
			if (flag)
			{
				connection.Close();
			}
			return obj;
		}

		public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteScalar(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteScalar(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static object ExecuteScalar(SqlTransaction transaction, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteScalar(transaction, commandType, commandText, null);
		}

		public static object ExecuteScalar(SqlTransaction transaction, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			SqlHelper.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out flag);
			object obj = sqlCommand.ExecuteScalar();
			sqlCommand.Parameters.Clear();
			return obj;
		}

		public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteScalar(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteScalar(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static object ExecuteScalarTypedParams(string connectionString, string spName, ValueRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteScalar(connectionString, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteScalar(connectionString, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static object ExecuteScalarTypedParams(SqlConnection connection, string spName, ValueRow dataRow)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteScalar(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteScalar(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static object ExecuteScalarTypedParams(SqlTransaction transaction, string spName, ValueRow dataRow)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteScalar(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteScalar(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static XmlReader ExecuteXmlReader(SqlConnection connection, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteXmlReader(connection, commandType, commandText, null);
		}

		public static XmlReader ExecuteXmlReader(SqlConnection connection, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			XmlReader xmlReader;
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			bool flag = false;
			SqlCommand sqlCommand = new SqlCommand();
			try
			{
				SqlHelper.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out flag);
				XmlReader xmlReader1 = sqlCommand.ExecuteXmlReader();
				sqlCommand.Parameters.Clear();
				xmlReader = xmlReader1;
			}
			catch
			{
				if (flag)
				{
					connection.Close();
				}
				throw;
			}
			return xmlReader;
		}

		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteXmlReader(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteXmlReader(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, System.Data.CommandType commandType, string commandText)
		{
			return SqlHelper.ExecuteXmlReader(transaction, commandType, commandText, null);
		}

		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, System.Data.CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			SqlHelper.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out flag);
			XmlReader xmlReader = sqlCommand.ExecuteXmlReader();
			sqlCommand.Parameters.Clear();
			return xmlReader;
		}

		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				return SqlHelper.ExecuteXmlReader(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			return SqlHelper.ExecuteXmlReader(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, string spName, ValueRow dataRow)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteXmlReader(connection, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteXmlReader(connection, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, string spName, ValueRow dataRow)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow == null || dataRow.ItemArray.Length == 0)
			{
				return SqlHelper.ExecuteXmlReader(transaction, System.Data.CommandType.StoredProcedure, spName);
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, dataRow);
			return SqlHelper.ExecuteXmlReader(transaction, System.Data.CommandType.StoredProcedure, spName, spParameterSet);
		}

		public static void FillDataset(string connectionString, System.Data.CommandType commandType, string commandText, ValueSet dataSet, string[] tableNames)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				SqlHelper.FillDataset(sqlConnection, commandType, commandText, dataSet, tableNames);
			}
		}

		public static void FillDataset(string connectionString, System.Data.CommandType commandType, string commandText, ValueSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				SqlHelper.FillDataset(sqlConnection, commandType, commandText, dataSet, tableNames, commandParameters);
			}
		}

		public static void FillDataset(string connectionString, string spName, ValueSet dataSet, string[] tableNames, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
				SqlHelper.FillDataset(sqlConnection, spName, dataSet, tableNames, parameterValues);
			}
		}

		public static void FillDataset(SqlConnection connection, System.Data.CommandType commandType, string commandText, ValueSet dataSet, string[] tableNames)
		{
			SqlHelper.FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
		}

		public static void FillDataset(SqlConnection connection, System.Data.CommandType commandType, string commandText, ValueSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
		{
			SqlHelper.FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
		}

		public static void FillDataset(SqlConnection connection, string spName, ValueSet dataSet, string[] tableNames, params object[] parameterValues)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				SqlHelper.FillDataset(connection, System.Data.CommandType.StoredProcedure, spName, dataSet, tableNames);
				return;
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			SqlHelper.FillDataset(connection, System.Data.CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
		}

		public static void FillDataset(SqlTransaction transaction, System.Data.CommandType commandType, string commandText, ValueSet dataSet, string[] tableNames)
		{
			SqlHelper.FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
		}

		public static void FillDataset(SqlTransaction transaction, System.Data.CommandType commandType, string commandText, ValueSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
		{
			SqlHelper.FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
		}

		public static void FillDataset(SqlTransaction transaction, string spName, ValueSet dataSet, string[] tableNames, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues == null || parameterValues.Length == 0)
			{
				SqlHelper.FillDataset(transaction, System.Data.CommandType.StoredProcedure, spName, dataSet, tableNames);
				return;
			}
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
			SqlHelper.AssignParameterValues(spParameterSet, parameterValues);
			SqlHelper.FillDataset(transaction, System.Data.CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
		}

		private static void FillDataset(SqlConnection connection, SqlTransaction transaction, System.Data.CommandType commandType, string commandText, ValueSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			SqlHelper.PrepareCommand(sqlCommand, connection, transaction, commandType, commandText, commandParameters, out flag);
			if (tableNames != null && tableNames.Length != 0)
			{
				string str = "Table";
				for (int i = 0; i < (int)tableNames.Length; i++)
				{
					if (tableNames[i] == null || tableNames[i].Length == 0)
					{
						throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
					}
					int num = i + 1;
					str = string.Concat(str, num.ToString());
				}
			}
			sqlCommand.Parameters.Clear();
			if (flag)
			{
				connection.Close();
			}
		}

		public static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, System.Data.CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			if (commandText == null || commandText.Length == 0)
			{
				throw new ArgumentNullException("commandText");
			}
			if (connection.State == ConnectionState.Open)
			{
				mustCloseConnection = false;
			}
			else
			{
				mustCloseConnection = true;
				connection.Open();
			}
			command.Connection = connection;
			command.CommandText = commandText;
			if (transaction != null)
			{
				if (transaction.Connection == null)
				{
					throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
				}
				command.Transaction = transaction;
			}
			command.CommandTimeout = SqlHelper.CommandTimeout;
			command.CommandType = commandType;
			if (commandParameters != null)
			{
				SqlHelper.AttachParameters(command, commandParameters);
			}
		}

		private enum SqlConnectionOwnership
		{
			Internal,
			External
		}
	}
}