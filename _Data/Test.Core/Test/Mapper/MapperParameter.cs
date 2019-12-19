using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Test.Core
{
	public class MapperParameter : Dictionary<string, MapperParameterItem>
	{
		public MapperParameterItem this[string name]
		{
			get
			{
				if (base.ContainsKey(name))
				{
					return base[name];
				}
				return this.Add(name);
			}
		}

		public MapperParameter()
		{
		}

		public void Add(MapperParameterItem parameter)
		{
			if (!base.ContainsKey(parameter.ParameterName))
			{
				base.Add(parameter.ParameterName, parameter);
			}
		}

		public MapperParameterItem Add(string name)
		{
			MapperParameterItem mapperParameterItem = new MapperParameterItem()
			{
				ParameterName = name
			};
			this.Add(mapperParameterItem);
			return mapperParameterItem;
		}

		public MapperParameterItem Add(string name, object value)
		{
			MapperParameterItem mapperParameterItem = new MapperParameterItem()
			{
				ParameterName = name,
				Value = value
			};
			this.Add(mapperParameterItem);
			return mapperParameterItem;
		}

		public MapperParameterItem Add(string name, SqlDataType? type, object value)
		{
			MapperParameterItem mapperParameterItem = new MapperParameterItem()
			{
				ParameterName = name,
				SqlType = type,
				Value = value
			};
			this.Add(mapperParameterItem);
			return mapperParameterItem;
		}

		protected static void ChangeToSqlValue(DbType type, ref object val)
		{
			if (val != null)
			{
				if (val is INumericCode)
				{
					val = (val as INumericCode).Value;
					return;
				}
				if (val is ICode)
				{
					val = val.ToString();
					return;
				}
				if (val is Sequence)
				{
					val = val.ToString();
					return;
				}
				DateTime? nullable = (DateTime?)(val as DateTime?);
				if (!nullable.HasValue)
				{
					if (type == DbType.DateTime)
					{
						val = DateTime.Parse(val.ToString());
						return;
					}
				}
				else if (!nullable.Value.IsValid())
				{
					val = null;
				}
			}
		}

		public T[] ToDbParameter<T>()
		where T : DbParameter, new()
		{
			int num = 0;
			T[] tArray = new T[base.Count];
			foreach (MapperParameterItem value in base.Values)
			{
				T parameterName = Activator.CreateInstance<T>();
				parameterName.ParameterName = value.ParameterName;
				parameterName.Direction = value.Direction;
				object obj = value.Value;
				if (value.SqlType.HasValue)
				{
					object dbType = parameterName;
					SqlDataType? sqlType = value.SqlType;
					((DbParameter)dbType).DbType = MapperParameter.ToDbType(sqlType.Value, ref obj);
				}
				else if (value.Type != null)
				{
					parameterName.DbType = MapperParameter.ToDbType(value.Type, ref obj);
				}
				if (value.Size > 0)
				{
					parameterName.Size = value.Size;
				}
				else if (parameterName.DbType != DbType.String && parameterName.DbType != DbType.StringFixedLength)
				{
					if (parameterName.DbType == DbType.AnsiString || parameterName.DbType == DbType.AnsiStringFixedLength)
					{
						if (parameterName.Direction == ParameterDirection.Input)
						{
							((DbParameter)parameterName).Size = (obj != null ? obj.ToString().Length : 1);
						}
						else
						{
							parameterName.Size = 4000;
						}
					}
				}
				else if (parameterName.Direction == ParameterDirection.Input)
				{
					((DbParameter)parameterName).Size = (obj != null ? obj.ToString().Length : 1);
				}
				else
				{
					parameterName.Size = 8000;
				}
				parameterName.Value = obj;
				tArray[num] = parameterName;
				num++;
			}
			return tArray;
		}

		public static DbType ToDbType(SqlDataType type, ref object realValue)
		{
			DbType dbType = DbType.AnsiString;
			if (type == SqlDataType.NVarChar)
			{
				dbType = DbType.String;
			}
			else if (type == SqlDataType.Char)
			{
				dbType = DbType.AnsiStringFixedLength;
				if (realValue is bool)
				{
					realValue = ((bool)realValue ? "Y" : "N");
				}
			}
			else if (type == SqlDataType.NChar)
			{
				dbType = DbType.StringFixedLength;
			}
			else if (type == SqlDataType.Int)
			{
				dbType = DbType.Int32;
			}
			else if (type == SqlDataType.SmallInt)
			{
				dbType = DbType.Int16;
			}
			else if (type == SqlDataType.BigInt)
			{
				dbType = DbType.Int64;
			}
			else if (type == SqlDataType.DateTime)
			{
				dbType = DbType.DateTime;
			}
			else if (type == SqlDataType.Bit)
			{
				dbType = DbType.Boolean;
				realValue = (bool.Parse(realValue.ToString()) ? 1 : 0);
			}
			else if (type == SqlDataType.Decimal)
			{
				dbType = DbType.Decimal;
			}
			else if (type == SqlDataType.Binary)
			{
				dbType = DbType.Binary;
			}
			MapperParameter.ChangeToSqlValue(dbType, ref realValue);
			return dbType;
		}

		public static DbType ToDbType(Type type, ref object realValue)
		{
			DbType dbType = DbType.AnsiString;
			if (type.Equals(typeof(string)))
			{
				dbType = DbType.AnsiString;
			}
			else if (type.Equals(typeof(short)))
			{
				dbType = DbType.Int16;
			}
			else if (type.Equals(typeof(int)))
			{
				dbType = DbType.Int32;
			}
			else if (type.Equals(typeof(long)))
			{
				dbType = DbType.Int64;
			}
			else if (type.Equals(typeof(DateTime)))
			{
				dbType = DbType.DateTime;
			}
			else if (type.Equals(typeof(bool)))
			{
				dbType = DbType.Boolean;
				realValue = (bool.Parse(realValue.ToString()) ? 1 : 0);
			}
			else if (type.Equals(typeof(double)))
			{
				dbType = DbType.Double;
			}
			else if (type.Equals(typeof(float)))
			{
				dbType = DbType.Single;
			}
			else if (type.Equals(typeof(byte)))
			{
				dbType = DbType.Byte;
			}
			else if (type.Equals(typeof(byte[])))
			{
				dbType = DbType.Binary;
			}
			else if (type.GetTypeInfo().IsSubclassOf(typeof(ICode)))
			{
				dbType = DbType.AnsiString;
			}
			else if (type.GetTypeInfo().IsSubclassOf(typeof(INumericCode)))
			{
				dbType = DbType.Int32;
			}
			MapperParameter.ChangeToSqlValue(dbType, ref realValue);
			return dbType;
		}
	}
}