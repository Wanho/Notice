using System;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	[AttributeUsage(AttributeTargets.Property, Inherited=true, AllowMultiple=false)]
	public class DescriptionAttribute : Attribute
	{
		public string Note
		{
			get;
			set;
		}

		public DescriptionAttribute()
		{
		}

		public DescriptionAttribute(string note)
		{
			this.Note = note;
		}
	}
}