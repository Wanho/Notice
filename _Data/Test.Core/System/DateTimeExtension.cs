using System.Runtime.CompilerServices;

namespace System
{
	public static class DateTimeExtension
	{
		public static bool IsValid(this DateTime dt)
		{
			return dt >= new DateTime(1900, 1, 1);
		}
	}
}