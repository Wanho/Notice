using System;
using System.Data;
using System.Data.SqlClient;

namespace Test.Core
{
	[MapperProvider("System.Data.SqlClient")]
	public class MssqlProvider : MapperProvider
	{
		public MssqlProvider()
		{
		}

		public override int Query(string commandText, MapperParameter parameter, Test.Core.CommandType type)
		{
			if (parameter == null)
			{
				parameter = new MapperParameter();
			}
			SqlParameter[] dbParameter = parameter.ToDbParameter<SqlParameter>();
			int num = SqlHelper.ExecuteNonQuery(this.connectionString, (System.Data.CommandType)type, commandText, dbParameter);
			if (type != Test.Core.CommandType.Text)
			{
				this.UpdateParameter(parameter, dbParameter);
			}
			return num;
		}

		public override int Query(MapperCommand command)
		{
			int num;
			SqlParameter[] dbParameter = command.Parameter.ToDbParameter<SqlParameter>();
			using (SqlConnection sqlConnection = new SqlConnection(this.connectionString))
			{
				sqlConnection.Open();
				SqlCommand sqlCommand = new SqlCommand();
				bool flag = false;
				SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, (System.Data.CommandType)command.Type, command.Text, dbParameter, out flag);
				if (command.Timeout > SqlHelper.CommandTimeout)
				{
					sqlCommand.CommandTimeout = command.Timeout;
				}
				int num1 = sqlCommand.ExecuteNonQuery();
				sqlCommand.Parameters.Clear();
				if (flag)
				{
					sqlConnection.Close();
				}
				if (command.Type != Test.Core.CommandType.Text)
				{
					this.UpdateParameter(command.Parameter, dbParameter);
				}
				num = num1;
			}
			return num;
		}

		public override object QueryForScalar(string commandText, MapperParameter parameter, Test.Core.CommandType type)
		{
			if (parameter == null)
			{
				parameter = new MapperParameter();
			}
			SqlParameter[] dbParameter = parameter.ToDbParameter<SqlParameter>();
			object obj = SqlHelper.ExecuteScalar(this.connectionString, (System.Data.CommandType)type, commandText, dbParameter);
			if (type != Test.Core.CommandType.Text)
			{
				this.UpdateParameter(parameter, dbParameter);
			}
			return obj;
		}

		public override object QueryForScalar(MapperCommand command)
		{
			SqlParameter[] dbParameter = command.Parameter.ToDbParameter<SqlParameter>();
			object obj = SqlHelper.ExecuteScalar(this.connectionString, (System.Data.CommandType)command.Type, command.Text, dbParameter);
			if (command.Type != Test.Core.CommandType.Text)
			{
				this.UpdateParameter(command.Parameter, dbParameter);
			}
			return obj;
		}

		public override ValueSet QueryForValueSet(string commandText, MapperParameter parameter, Test.Core.CommandType type)
		{
			if (parameter == null)
			{
				parameter = new MapperParameter();
			}
			SqlParameter[] dbParameter = parameter.ToDbParameter<SqlParameter>();
			ValueSet valueSet = SqlHelper.ExecuteDataset(this.connectionString, (System.Data.CommandType)type, commandText, dbParameter);
			if (type != Test.Core.CommandType.Text)
			{
				this.UpdateParameter(parameter, dbParameter);
			}
			return valueSet;
		}

		public override ValueSet QueryForValueSet(MapperCommand command)
		{
			SqlParameter[] dbParameter = command.Parameter.ToDbParameter<SqlParameter>();
			ValueSet valueSet = SqlHelper.ExecuteDataset(this.connectionString, (System.Data.CommandType)command.Type, command.Text, dbParameter);
			if (command.Type != Test.Core.CommandType.Text)
			{
				this.UpdateParameter(command.Parameter, dbParameter);
			}
			return valueSet;
		}

		private void UpdateParameter(MapperParameter parameter, SqlParameter[] dbParameter)
		{
			SqlParameter[] sqlParameterArray = dbParameter;
			for (int i = 0; i < (int)sqlParameterArray.Length; i++)
			{
				SqlParameter value = sqlParameterArray[i];
				if (value.Direction != ParameterDirection.Input)
				{
					parameter[value.ParameterName].Value = value.Value;
				}
			}
		}
	}
}