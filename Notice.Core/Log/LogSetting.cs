using System;
using System.Runtime.CompilerServices;

namespace Notice.Core
{
	public class LogSetting
	{
		public string LogPath;

		public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

		public LogLevel Level { get; set; } = LogLevel.All;
		public string Name { get; set; }

		public string Namespace { get; set; } = string.Empty;
		public LoggingType Type { get; set; } = LoggingType.Console;
		public bool Utc { get; set; }
		public string Version { get; set; }

		public LogSetting()
		{
		}
	}
}