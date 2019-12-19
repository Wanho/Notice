using System;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class MapperParameterItem : DbParameter
	{
		public override System.Data.DbType DbType
		{
			get;
			set;
		}

		public override ParameterDirection Direction
		{
			get;
			set;
		}

		public override bool IsNullable
		{
			get;
			set;
		}

		public bool IsTable
		{
			get;
			set;
		}

		public override string ParameterName
		{
			get;
			set;
		}

		public override int Size
		{
			get;
			set;
		}

		public override string SourceColumn
		{
			get;
			set;
		}

		public override bool SourceColumnNullMapping
		{
			get;
			set;
		}

		public SqlDataType? SqlType
		{
			get;
			set;
		}

		public System.Type Type
		{
			get;
			set;
		}

		public override object Value
		{
			get;
			set;
		}

		internal MapperParameterItem()
		{
		}

		public override void ResetDbType()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}