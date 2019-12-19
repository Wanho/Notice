using System;

namespace Test.Core
{
	public class ValueTableCollection : ValueCollection<ValueTable>
	{
		public ValueTableCollection()
		{
		}

		public ValueTable Add()
		{
			if (base.Count == 0)
			{
				return this.Add("Table");
			}
			return this.Add(string.Concat("Table", base.Count));
		}

		public ValueTable Add(string name)
		{
			ValueTable valueTable = new ValueTable();
			base.Add(valueTable);
			valueTable.TableName = name;
			return valueTable;
		}

		public bool Contains(string name)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(ValueTable table)
		{
			throw new NotImplementedException();
		}

		public int IndexOf(string tableName)
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