namespace System
{
	public static class EnumExtension
	{
		public static T Parse<T>(string s)
		{
			return (T)Enum.Parse(typeof(T), s);
		}
	}
}