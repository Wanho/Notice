using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public static class ValueRowExtension
	{
		private static object _lock;

		static ValueRowExtension()
		{
			ValueRowExtension._lock = new object();
		}

		internal static T _ToVo<T>(ValueRow dr, T vo, bool useAlias)
		where T : ValueObject
		{
			Type reflectedType;
			Type type = typeof(T);
			if (dr == null)
			{
				if (vo != null)
				{
					return vo;
				}
				return default(T);
			}
			if (vo == null)
			{
				vo = Activator.CreateInstance<T>();
			}
			BaseEntity baseEntity = (object)vo as BaseEntity;
			if (baseEntity != null)
			{
				baseEntity.SetValueRow(dr);
			}
			else
			{
			}
			ValueTable table = dr.Table;
			foreach (ValueColumn column in dr.Table.Columns)
			{
				string columnName = column.ColumnName;
				PropertyInfo propertyInfo = null;
				if (table.mappingOrinal != null)
				{
					foreach (Tuple<Type, bool> tuple in table.mappingOrinal)
					{
						Type item1 = tuple.Item1;
						((tuple.Item2 ? ValueObject.DataMappingAlias[item1] : ValueObject.DataMapping[item1])).TryGetValue(columnName, out propertyInfo);
						if (propertyInfo == null)
						{
							continue;
						}
						goto Label0;
					}
				}
				else
				{
					((useAlias ? ValueObject.DataMappingAlias[type] : ValueObject.DataMapping[type])).TryGetValue(columnName, out propertyInfo);
				}
			Label0:
				Type type1 = typeof(T);
				if (propertyInfo != null)
				{
					reflectedType = propertyInfo.ReflectedType;
				}
				else
				{
					reflectedType = null;
				}
				if (type1 != reflectedType)
				{
					continue;
				}
				object item = dr[column.ColumnOrdinal.Value];
				vo.Invoke(item, propertyInfo);
			}
			return vo;
		}

		public static T ToVo<T>(this ValueRow dr, bool useAlias = false)
		where T : ValueObject
		{
			return ValueRowExtension._ToVo<T>(dr, default(T), useAlias);
		}

		public static T ToVo<T>(this ValueRow dr, T vo, bool useAlias = false)
		where T : ValueObject
		{
			return ValueRowExtension._ToVo<T>(dr, vo, useAlias);
		}
	}
}