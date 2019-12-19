using System;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Property, Inherited=true, AllowMultiple=false)]
	public sealed class PrimaryKeyAttribute : Attribute
	{
		public PrimaryKeyAttribute()
		{
		}
	}
}