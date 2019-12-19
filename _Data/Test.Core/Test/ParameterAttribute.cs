using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Method, Inherited=true, AllowMultiple=true)]
	public sealed class ParameterAttribute : Attribute
	{
		public MapperParameterItem Parameter { get; set; } = new MapperParameterItem();

		public ParameterAttribute(string name, object val, SqlDataType type = SqlDataType.VarChar)
		{
			this.Parameter.ParameterName = name;
			this.Parameter.Value = val;
			this.Parameter.SqlType = new SqlDataType?(type);
		}
	}
}