using System;
using System.Collections.Generic;

namespace Test.Core
{
	public class ValueColumnCollection : ValueCollection<ValueColumn>
	{
		private int sameCount = 1;

		public ValueColumnCollection()
		{
		}

		public ValueColumn Add()
		{
			return this.Add("", typeof(object), "varchar");
		}

		public ValueColumn Add(string name)
		{
			return this.Add(name, typeof(object));
		}

		public ValueColumn Add(string name, Type type)
		{
			return this.Add(name, type, "varchar");
		}

		public ValueColumn Add(string name, Type type, string dataTypeName)
		{
			if (this.Contains(name))
			{
				name = string.Concat("expr", this.sameCount);
				this.sameCount++;
			}
			ValueColumn valueColumn = new ValueColumn(name, type, dataTypeName);
			base.Add(valueColumn);
			valueColumn.SetOrdinal(base.List.Count - 1);
			base.Dic.Add(name, valueColumn);
			return valueColumn;
		}

		public void AddRange(ValueColumn[] columns)
		{
			ValueColumn[] valueColumnArray = columns;
			for (int i = 0; i < (int)valueColumnArray.Length; i++)
			{
				base.Add(valueColumnArray[i]);
			}
		}

		public bool Contains(string name)
		{
			return base.Dic.ContainsKey(name);
		}

		public int IndexOf(ValueColumn column)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(string columnName)
		{
			throw new NotImplementedException();
		}

		public void Remove(string name)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}
	}
}