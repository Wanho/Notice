using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public static class ValueTableExtension
	{
		internal static ValueRow FirstRow(this ValueTable dt)
		{
			if (dt == null)
			{
				return null;
			}
			if (dt.Rows.Count == 0)
			{
				return null;
			}
			return dt.Rows[0];
		}

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this ValueTable dt)
		where TValue : ValueObject
		{
			Dictionary<TKey, TValue> tKeys = new Dictionary<TKey, TValue>(dt.Rows.Count);
			Type type = typeof(TValue);
			PropertyInfo propertyInfo = null;
			string empty = string.Empty;
			PropertyInfo[] properties = type.GetProperties();
			for (int i = 0; i < (int)properties.Length; i++)
			{
				PropertyInfo propertyInfo1 = properties[i];
				ColumnAttribute customAttribute = propertyInfo1.GetCustomAttribute<ColumnAttribute>();
				if (customAttribute != null && (customAttribute.ColumnType & SqlColumnType.PrimaryKey) == SqlColumnType.PrimaryKey)
				{
					propertyInfo = propertyInfo1;
					empty = customAttribute.Name;
				}
			}
			if (propertyInfo == null)
			{
				throw new Exception(string.Concat(type.ToString(), " not exist primary key property"));
			}
			string columnName = string.Empty;
			foreach (ValueColumn column in dt.Columns)
			{
				if (!column.ColumnName.MatchDataName(empty))
				{
					continue;
				}
				columnName = column.ColumnName;
			}
			if (string.IsNullOrEmpty(columnName))
			{
				throw new Exception(string.Concat(type.ToString(), " not exist primary key column"));
			}
			if (dt != null)
			{
				foreach (ValueRow row in dt.Rows)
				{
					TKey item = (TKey)row[columnName];
					if (tKeys.ContainsKey(item))
					{
						throw new Exception(string.Concat(type.ToString(), " duplicate key:", tKeys[item]));
					}
					TValue tValue = default(TValue);
					tKeys.Add(item, row.ToVo<TValue>(tValue, false));
				}
			}
			return tKeys;
		}

		public static T ToFirstVo<T>(this ValueTable dt)
		where T : ValueObject
		{
			return dt.FirstRow().ToVo<T>(false);
		}

		public static T ToFirstVo<T>(this ValueTable dt, T vo)
		where T : ValueObject
		{
			return dt.FirstRow().ToVo<T>(vo, false);
		}

		public static List<T> ToList<T>(this ValueTable dt)
		where T : ValueObject
		{
			List<T> ts = new List<T>(dt.Rows.Count);
			if (dt != null)
			{
				foreach (ValueRow row in dt.Rows)
				{
					ts.Add(row.ToVo<T>(default(T), false));
				}
			}
			return ts;
		}
	}
}