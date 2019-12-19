using System;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class ValueColumn : DbColumn
	{
		public bool AutoIncrement
		{
			get;
			set;
		}

		public string Caption
		{
			get;
			set;
		}

		public int MaxLength
		{
			get;
			set;
		}

		public ValueTable Table
		{
			get;
			internal set;
		}

		public bool Unique
		{
			get;
			set;
		}

		public ValueColumn()
		{
		}

		public ValueColumn(string columnName)
		{
			base.ColumnName = columnName;
		}

		public ValueColumn(string columnName, Type type)
		{
			base.ColumnName = columnName;
			base.DataType = type;
		}

		public ValueColumn(string columnName, Type type, string dataTypeName)
		{
			base.ColumnName = columnName;
			base.DataType = type;
			base.DataTypeName = base.DataTypeName;
		}

		public void SetOrdinal(int idx)
		{
			base.ColumnOrdinal = new int?(idx);
		}
	}
}