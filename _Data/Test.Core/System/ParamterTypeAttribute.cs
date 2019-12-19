using System.Runtime.CompilerServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
	public class ParamterTypeAttribute : Attribute
	{
		public Type ParamterType
		{
			get;
			protected set;
		}

		public ParamterTypeAttribute(Type type)
		{
			this.ParamterType = type;
		}
	}
}