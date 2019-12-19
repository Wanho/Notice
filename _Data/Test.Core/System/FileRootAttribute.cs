using System.Runtime.CompilerServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public class FileRootAttribute : Attribute
	{
		public string Name
		{
			get;
			private set;
		}

		public FileRootAttribute(string rootName)
		{
			this.Name = rootName;
		}
	}
}