using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface, AllowMultiple=true)]
	public sealed class ProviderParameterAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public ProviderParameterAttribute(string name)
		{
			this.Name = name;
		}
	}
}