using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false)]
	public sealed class MapperProviderAttribute : Attribute
	{
		public string Name
		{
			get;
			private set;
		}

		public MapperProviderAttribute(string name)
		{
			this.Name = name;
		}
	}
}