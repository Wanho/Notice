using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class ValueTable
	{
		internal List<Tuple<Type, bool>> mappingOrinal;

		public ValueColumnCollection Columns { get; } = new ValueColumnCollection();

		public ValueSet DataSet
		{
			get;
		}

		public ValueColumn[] PrimaryKey
		{
			get;
			set;
		}

		public ValueRowCollection Rows { get; } = new ValueRowCollection();

		public string TableName
		{
			get;
			set;
		}

		public ValueTable()
		{
		}

		public ValueTable(string tableName)
		{
			this.TableName = tableName;
		}

		public void AddMappingOrdinal(Type type, bool useAlias = false)
		{
			if (this.mappingOrinal == null)
			{
				this.mappingOrinal = new List<Tuple<Type, bool>>();
			}
			this.mappingOrinal.Add(new Tuple<Type, bool>(type, useAlias));
		}

		public ValueRow NewRow()
		{
			return new ValueRow()
			{
				Table = this
			};
		}

		public void ResetMappingOrdinal()
		{
			this.mappingOrinal = null;
		}

		public ValueRow[] Select(string filterExpression, string sort)
		{
			throw new NotImplementedException();
		}
	}
}