using System;
using System.Collections.Generic;

namespace Notice.Core
{
	public class LogManager
	{
		internal static Dictionary<string, LogSetting> settings;

		internal static ILogService logService;

		public static Action<LogInfo> SetLogInfo;

		static LogManager()
		{
			LogManager.settings = new Dictionary<string, LogSetting>();
		}

		public LogManager()
		{
		}

		public static void AddSetting(LogSetting setting)
		{
			LogManager.settings.Add(setting.Name, setting);
		}

		public static ILog GetLogger(string prefix = null)
		{
			return new Logger(prefix);
		}

		public static LogSetting GetSetting(string name)
		{
			return LogManager.settings[name];
		}

		public static void SetLoggerModel(ILogService service)
		{
			LogManager.logService = service;
		}
	}
}