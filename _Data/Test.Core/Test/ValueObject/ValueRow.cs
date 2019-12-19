using System;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public class ValueRow : DbDataRecord
	{
		protected ValueTable _Table;

		public override int FieldCount
		{
			get
			{
				return (int)this.ItemArray.Length;
			}
		}

		public object this[ValueColumn column]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override object this[int i]
		{
			get
			{
				return this.ItemArray[i];
			}
		}

		public override object this[string name]
		{
			get
			{
				object[] itemArray = this.ItemArray;
				int? columnOrdinal = this._Table.Columns[name].ColumnOrdinal;
				return itemArray[columnOrdinal.Value];
			}
		}

		public object[] ItemArray
		{
			get;
			set;
		}

		public ValueTable Table
		{
			get
			{
				return this._Table;
			}
			internal set
			{
				this._Table = value;
				this.ItemArray = new object[this._Table.Columns.Count];
			}
		}

		public ValueRow()
		{
		}

		public override bool GetBoolean(int i)
		{
			throw new NotImplementedException();
		}

		public override byte GetByte(int i)
		{
			throw new NotImplementedException();
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			throw new NotImplementedException();
		}

		public override char GetChar(int i)
		{
			throw new NotImplementedException();
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			throw new NotImplementedException();
		}

		public override string GetDataTypeName(int i)
		{
			throw new NotImplementedException();
		}

		public override DateTime GetDateTime(int i)
		{
			throw new NotImplementedException();
		}

		public override decimal GetDecimal(int i)
		{
			throw new NotImplementedException();
		}

		public override double GetDouble(int i)
		{
			throw new NotImplementedException();
		}

		public override Type GetFieldType(int i)
		{
			throw new NotImplementedException();
		}

		public override float GetFloat(int i)
		{
			throw new NotImplementedException();
		}

		public override Guid GetGuid(int i)
		{
			throw new NotImplementedException();
		}

		public override short GetInt16(int i)
		{
			throw new NotImplementedException();
		}

		public override int GetInt32(int i)
		{
			throw new NotImplementedException();
		}

		public override long GetInt64(int i)
		{
			throw new NotImplementedException();
		}

		public override string GetName(int i)
		{
			throw new NotImplementedException();
		}

		public override int GetOrdinal(string name)
		{
			throw new NotImplementedException();
		}

		public override string GetString(int i)
		{
			throw new NotImplementedException();
		}

		public override object GetValue(int i)
		{
			throw new NotImplementedException();
		}

		public override int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		public override bool IsDBNull(int i)
		{
			throw new NotImplementedException();
		}
	}
}