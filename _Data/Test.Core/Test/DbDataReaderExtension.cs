using System;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public static class DbDataReaderExtension
	{
		public static void Fill(this DbDataReader sdr, ValueSet ds)
		{
			ValueTable valueTable = ds.Tables.Add();
			for (int i = 0; i < sdr.FieldCount; i++)
			{
				valueTable.Columns.Add(sdr.GetName(i), sdr.GetFieldType(i), sdr.GetDataTypeName(i));
			}
			while (sdr.Read())
			{
				ValueRow item = valueTable.NewRow();
				for (int j = 0; j < sdr.FieldCount; j++)
				{
					item.ItemArray[j] = sdr[j];
				}
				valueTable.Rows.Add(item);
			}
			if (sdr.NextResult())
			{
				sdr.Fill(ds);
			}
		}
	}
}