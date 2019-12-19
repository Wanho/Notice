using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Property, Inherited=true, AllowMultiple=false)]
	public sealed class ColumnAliasAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public ColumnAliasAttribute(string name)
		{
			this.Name = name;
		}
	}
}