using System.Runtime.CompilerServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FileTypeCodeAttribute : Attribute
	{
		public string DateFormat
		{
			get;
			set;
		}

		public string DirectoryName
		{
			get;
			set;
		}

		public bool Private
		{
			get;
			set;
		}

		public FileTypeCodeAttribute(string dateFormat)
		{
			this.DateFormat = dateFormat;
		}

		public FileTypeCodeAttribute(string dateFormat, string directoryName)
		{
			this.DateFormat = dateFormat;
			this.DirectoryName = directoryName;
		}

		public FileTypeCodeAttribute(string dateFormat, string directoryName, bool isPrivate)
		{
			this.DateFormat = dateFormat;
			this.DirectoryName = directoryName;
			this.Private = isPrivate;
		}
	}
}