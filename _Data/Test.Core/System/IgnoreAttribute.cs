namespace System
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class IgnoreAttribute : Attribute
	{
		public IgnoreAttribute()
		{
		}
	}
}