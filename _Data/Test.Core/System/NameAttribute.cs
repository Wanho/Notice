using System.Runtime.CompilerServices;

namespace System
{
	[AttributeUsage(AttributeTargets.All, Inherited=true, AllowMultiple=false)]
	public sealed class NameAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public NameAttribute(string name)
		{
			this.Name = name;
		}
	}
}