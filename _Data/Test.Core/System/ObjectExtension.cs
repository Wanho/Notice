using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
	public static class ObjectExtension
	{
		private readonly static HashSet<Type> NumericTypes;

		static ObjectExtension()
		{
			HashSet<Type> types = new HashSet<Type>();
			types.Add(typeof(int));
			types.Add(typeof(double));
			types.Add(typeof(decimal));
			types.Add(typeof(long));
			types.Add(typeof(short));
			types.Add(typeof(sbyte));
			types.Add(typeof(byte));
			types.Add(typeof(ulong));
			types.Add(typeof(ushort));
			types.Add(typeof(uint));
			types.Add(typeof(float));
			ObjectExtension.NumericTypes = types;
		}

		private static bool IsNumeric(Type myType)
		{
			return ObjectExtension.NumericTypes.Contains(Nullable.GetUnderlyingType(myType) ?? myType);
		}

		public static bool IsNumericType(this object o)
		{
			return ObjectExtension.IsNumeric(o.GetType());
		}
	}
}