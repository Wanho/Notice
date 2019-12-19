using System;
using System.Collections;
using System.Data.SqlClient;

namespace System.Data
{
	public sealed class SqlHelperParameterCache
	{
		private static Hashtable paramCache;

		static SqlHelperParameterCache()
		{
			SqlHelperParameterCache.paramCache = Hashtable.Synchronized(new Hashtable());
		}

		private SqlHelperParameterCache()
		{
		}

		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (commandText == null || commandText.Length == 0)
			{
				throw new ArgumentNullException("commandText");
			}
			string str = string.Concat(connectionString, ":", commandText);
			SqlHelperParameterCache.paramCache[str] = commandParameters;
		}

		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
		{
			SqlParameter[] sqlParameterArray = new SqlParameter[(int)originalParameters.Length];
			int num = 0;
			int length = (int)originalParameters.Length;
			while (num < length)
			{
				num++;
			}
			return sqlParameterArray;
		}

		private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
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
				CommandType = CommandType.StoredProcedure
			};
			connection.Open();
			connection.Close();
			if (!includeReturnValueParameter)
			{
				sqlCommand.Parameters.RemoveAt(0);
			}
			SqlParameter[] sqlParameterArray = new SqlParameter[sqlCommand.Parameters.Count];
			sqlCommand.Parameters.CopyTo(sqlParameterArray, 0);
			SqlParameter[] value = sqlParameterArray;
			for (int i = 0; i < (int)value.Length; i++)
			{
				value[i].Value = DBNull.Value;
			}
			return sqlParameterArray;
		}

		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (commandText == null || commandText.Length == 0)
			{
				throw new ArgumentNullException("commandText");
			}
			string str = string.Concat(connectionString, ":", commandText);
			SqlParameter[] item = SqlHelperParameterCache.paramCache[str] as SqlParameter[];
			if (item == null)
			{
				return null;
			}
			return SqlHelperParameterCache.CloneParameters(item);
		}

		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return SqlHelperParameterCache.GetSpParameterSet(connectionString, spName, false);
		}

		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			SqlParameter[] spParameterSetInternal;
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				spParameterSetInternal = SqlHelperParameterCache.GetSpParameterSetInternal(sqlConnection, spName, includeReturnValueParameter);
			}
			return spParameterSetInternal;
		}

		internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
		{
			return SqlHelperParameterCache.GetSpParameterSet(connection, spName, false);
		}

		internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
		{
			return null;
		}

		private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			string str = string.Concat(connection.ConnectionString, ":", spName, (includeReturnValueParameter ? ":include ReturnValue Parameter" : ""));
			SqlParameter[] item = SqlHelperParameterCache.paramCache[str] as SqlParameter[];
			if (item == null)
			{
				SqlParameter[] sqlParameterArray = SqlHelperParameterCache.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
				SqlHelperParameterCache.paramCache[str] = sqlParameterArray;
				item = sqlParameterArray;
			}
			return SqlHelperParameterCache.CloneParameters(item);
		}
	}
}