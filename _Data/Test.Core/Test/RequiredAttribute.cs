using System;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Property, Inherited=true, AllowMultiple=false)]
	public class RequiredAttribute : Attribute
	{
		public RequiredAttribute()
		{
		}
	}
}