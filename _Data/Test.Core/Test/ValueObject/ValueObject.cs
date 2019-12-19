using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	public abstract class ValueObject
	{
		private static ILog logger;

		public static Dictionary<Type, Dictionary<string, PropertyInfo>> DataMapping
		{
			get;
		}

		public static Dictionary<Type, Dictionary<string, PropertyInfo>> DataMappingAlias
		{
			get;
		}

		public static Dictionary<Type, Dictionary<string, PropertyInfo>> ObjectProperty
		{
			get;
		}

		static ValueObject()
		{
			ValueObject.ObjectProperty = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
			ValueObject.DataMapping = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
			ValueObject.DataMappingAlias = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
			ValueObject.logger = LogManager.GetLogger("Test.ValueObject");
		}

		protected ValueObject()
		{
		}

		public static void Initialize(IEnumerable<Assembly> assemblies)
		{
			object name;
			ValueObject.logger.Debug("Initialize start", (LogInfo)null, null);
			foreach (Assembly assembly in assemblies)
			{
				foreach (Type type1 in 
					from type in (IEnumerable<Type>)assembly.GetTypes()
					let typeInfo = type.GetTypeInfo()
					where typeInfo.IsSubclassOf(typeof(ValueObject))
					select type)
				{
					ValueObject.ObjectProperty.Add(type1, new Dictionary<string, PropertyInfo>());
					ValueObject.DataMapping.Add(type1, new Dictionary<string, PropertyInfo>());
					ValueObject.DataMappingAlias.Add(type1, new Dictionary<string, PropertyInfo>());
					ValueObject.logger.Debug(string.Concat("Loading  ", type1.FullName), (LogInfo)null, null);
					PropertyInfo[] properties = type1.GetProperties();
					for (int i = 0; i < (int)properties.Length; i++)
					{
						PropertyInfo propertyInfo = properties[i];
						MethodInfo setMethod = propertyInfo.GetSetMethod();
						if (!(setMethod == null) && (setMethod.Attributes & MethodAttributes.Public) == MethodAttributes.Public)
						{
							ColumnAttribute customAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
							ColumnAliasAttribute columnAliasAttribute = propertyInfo.GetCustomAttribute<ColumnAliasAttribute>();
							string str = propertyInfo.Name;
							if (customAttribute != null)
							{
								name = customAttribute.Name;
							}
							else
							{
								name = null;
							}
							if (name == null)
							{
								name = str.ToDataName();
							}
							string str1 = (string)name;
							try
							{
								if (columnAliasAttribute != null)
								{
									ValueObject.DataMappingAlias[type1].Add(columnAliasAttribute.Name, propertyInfo);
								}
								else
								{
									ValueObject.DataMappingAlias[type1].Add(str1, propertyInfo);
								}
							}
							catch (Exception exception)
							{
								ValueObject.logger.Fatal(type1.Name, string.Concat("check column name.  ", str), null, null);
								throw;
							}
							ValueObject.ObjectProperty[type1].Add(str, propertyInfo);
							ValueObject.DataMapping[type1].Add(str1, propertyInfo);
						}
					}
				}
			}
		}

		internal void Invoke(object val, PropertyInfo prop)
		{
			int num;
			float single;
			double num1;
			string str;
			Type propertyType;
			Type type;
			MethodInfo setMethod = prop.GetSetMethod();
			object empty = val;
			if (val != null)
			{
				str = val.ToString();
			}
			else
			{
				str = null;
			}
			string str1 = str;
			Type propertyType1 = prop.PropertyType;
			if (val is DBNull)
			{
				if (propertyType1 != typeof(string))
				{
                    if (propertyType1 != typeof(DateTime))
                    {
                        empty = null;
                    }
                    else
                    {
                        empty = DateTime.MinValue;
                    }
				}
				else
				{
					empty = string.Empty;
				}
			}
			else if (val != null)
			{
				if (propertyType1 == typeof(int))
				{
					if (!(val is int) && int.TryParse(str1, out num))
					{
						empty = num;
					}
				}
				else if (propertyType1 == typeof(float))
				{
					if (!(val is float) && float.TryParse(str1, out single))
					{
						empty = single;
					}
				}
				else if (propertyType1 == typeof(double))
				{
					if (!(val is double) && double.TryParse(str1, out num1))
					{
						empty = num1;
					}
				}
				else if (propertyType1 == typeof(string))
				{
					if (!(val is string))
					{
						empty = str1;
					}
				}
				else if (propertyType1 == typeof(bool))
				{
					if (empty is string)
					{
						empty = str1 == "Y";
					}
				}
				else if (propertyType1 != typeof(Sequence))
				{
					PropertyInfo property = propertyType1.GetProperty("Value");
					if (property != null)
					{
						propertyType = property.PropertyType;
					}
					else
					{
						propertyType = null;
					}
					if (propertyType != typeof(string))
					{
						PropertyInfo propertyInfo = propertyType1.GetProperty("Value");
						if (propertyInfo != null)
						{
							type = propertyInfo.PropertyType;
						}
						else
						{
							type = null;
						}
						if (type == typeof(int))
						{
							empty = propertyType1.GetMethod("FindCode_", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, new object[] { val });
						}
					}
					else
					{
						empty = propertyType1.GetMethod("FindCode_", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, new object[] { val });
					}
				}
				else
				{
					empty = (Sequence)str1;
				}
			}
			if (setMethod != null)
			{
				setMethod.Invoke(this, new object[] { empty });
			}
		}
	}
}