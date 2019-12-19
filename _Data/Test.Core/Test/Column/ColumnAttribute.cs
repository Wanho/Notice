using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Property, Inherited=true, AllowMultiple=false)]
	public sealed class ColumnAttribute : Attribute
	{
		public SqlColumnType ColumnType
		{
			get;
			private set;
		}

		public SqlDataType DataType
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public ColumnAttribute(string name, SqlDataType dataType = SqlDataType.VarChar, SqlColumnType columnType = 0)
		{
			this.Name = name;
			this.DataType = dataType;
			this.ColumnType = columnType;
		}
	}
}