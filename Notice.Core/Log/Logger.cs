using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Notice.Core
{
    [Flags]
    public enum LoggingType { None = 0, Console = 1, File = 2, WindowEvent = 8 }

    public enum LogLevel { Off, Fatal, Error, Warn, Info, Debug, All }

    public class LogEntity
    {
        public DateTime CreateDate { get; set; }
        public string Exception { get; set; }
        public string LogSeq { get; set; }
        public string Namespace { get; set; }
        public string Note { get; set; }
        public string Parameter { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public string Version { get; set; }

        public LogEntity()
        {
        }
    }

    public class LogInfo
    {
        public string Parameter { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }

        public LogInfo()
        {
        }
    }

    internal static class LoggingState
    {
        internal static int LogNo;

        static LoggingState() { }
    }

    internal class Logger : ILog
    {
		public string Parameter { get; set; }

		public string Prefix { get; set; }

		internal Logger(string prefix)
		{
			this.Prefix = prefix;
		}

		public void Debug(string _namespace, string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Debug, _namespace, text, i, e); }
		public void Debug(string _namespace, string text, Exception e) { this.Write(LogLevel.Debug, _namespace, text, null, e); }
		public void Debug(string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Debug, string.Empty, text, i, e); }
		public void Debug(string text, Exception e) { this.Write(LogLevel.Debug, string.Empty, text, null, e); }

		public void Error(string _namespace, string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Error, _namespace, text, i, e); }
		public void Error(string _namespace, string text, Exception e) { this.Write(LogLevel.Error, _namespace, text, null, e); }
		public void Error(string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Error, string.Empty, text, i, e); }
		public void Error(string text, Exception e) { this.Write(LogLevel.Error, string.Empty, text, null, e); }

		public void Fatal(string _namespace, string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Fatal, _namespace, text, i, e); }
		public void Fatal(string _namespace, string text, Exception e) { this.Write(LogLevel.Fatal, _namespace, text, null, e); }
		public void Fatal(string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Fatal, string.Empty, text, null, e); }
		public void Fatal(string text, Exception e) { this.Write(LogLevel.Fatal, string.Empty, text, null, e); }

		public void Info(string _namespace, string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Info, _namespace, text, i, e); }
		public void Info(string _namespace, string text, Exception e) { this.Write(LogLevel.Info, _namespace, text, null, e); }
		public void Info(string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Info, string.Empty, text, i, e); }
		public void Info(string text, Exception e) { this.Write(LogLevel.Info, string.Empty, text, null, e); }

		public void Warn(string _namespace, string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Warn, _namespace, text, i, e); }
		public void Warn(string _namespace, string text, Exception e) { this.Write(LogLevel.Warn, _namespace, text, null, e); }
		public void Warn(string text, LogInfo i = null, Exception e = null) { this.Write(LogLevel.Warn, string.Empty, text, i, e); }
		public void Warn(string text, Exception e) { this.Write(LogLevel.Warn, string.Empty, text, null, e); }

		protected void Write(LogLevel level, string _namespace, string text, LogInfo info = null, Exception e = null)
		{
			try
			{
				int num = Interlocked.Increment(ref LoggingState.LogNo);
				string str = string.Concat(this.Prefix, ".", _namespace);
				str = str.Trim(new char[] { '.' });
				if (info == null) {
					info = new LogInfo();
				}
				Action<LogInfo> setLogInfo = LogManager.SetLogInfo;
				if (setLogInfo != null) {
					setLogInfo(info);
				}
				if (string.IsNullOrEmpty(info.Parameter)) {
					info.Parameter = this.Parameter;
				}
				string[] strArrays = str.Split(new char[] { '.' });
				for (int i = 0; i <= (int)strArrays.Length; i++)
				{
					string str1 = string.Join(".", strArrays, 0, i).Trim(new char[] { '.' });
					foreach (LogSetting value in LogManager.settings.Values)
					{
						if (value.Level < level || !(value.Namespace == str1))
						{
							continue;
						}
						DateTime now = DateTime.Now;
						LogEntity logEntity = new LogEntity()
						{
							Version = value.Version,
							Namespace = str,
							Type = level.ToString(),
							Url = info.Url,
							Parameter = info.Parameter,
							UserId = info.UserId,
							CreateDate = now
						};
						StringBuilder sb = new StringBuilder();
                        sb.Append(string.Concat("[", level));
						if (!string.IsNullOrEmpty(value.Version))
						{
                            sb.Append(string.Concat(" v:", logEntity.Version));
						}
						sb.Append(string.Concat(" l:", value.Name));
						sb.Append(string.Concat(" n:", str));
						sb.Append(string.Concat(" t:", now.ToString(value.DateTimeFormat)));
						sb.Append(string.Concat(" #:", num));
						sb.Append("]");
                        sb.AppendLine();
						if (!string.IsNullOrEmpty(logEntity.Url))
						{
                            sb.AppendLine(string.Concat(" Url:", logEntity.Url));
						}
						if (!string.IsNullOrEmpty(logEntity.Parameter))
						{
                            sb.AppendLine(string.Concat(" Parameter:", logEntity.Parameter));
						}
						if (!string.IsNullOrEmpty(logEntity.UserId))
						{
                            sb.AppendLine(string.Concat(" UserId:", logEntity.UserId));
						}
                        sb.AppendLine(text);
                        sb.AppendLine();
                        logEntity.Note = sb.ToString();
						if (e != null)
						{
							StringBuilder sb1 = new StringBuilder();
                            sb1.AppendLine("Exception:");
                            sb1.AppendLine(e.ToString());
                            sb1.AppendLine();
							if (e.InnerException != null)
							{
                                sb1.AppendLine("InnerException:");
                                sb1.AppendLine(e.InnerException.ToString());
                                sb1.AppendLine();
							}
                            logEntity.Exception = sb1.ToString();
							sb.Append(sb1);
						}
						if ((value.Type & LoggingType.Console) == LoggingType.Console)
						{
							this.WriteConsole(sb);
						}
						if ((value.Type & LoggingType.File) == LoggingType.File)
						{
							this.WriteFile(value, str, sb);
						}
						
						this.WriteDefault(logEntity);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void WriteConsole(StringBuilder text)
		{
		}

		private void WriteDefault(LogEntity logEntity)
		{
			LogManager.logService.Create(logEntity);
		}

		private void WriteFile(LogSetting setting, string key, StringBuilder text)
		{
			string logPath = setting.LogPath;
			DateTime now = DateTime.Now;
			string str = string.Concat(logPath, now.ToString("yyyy-MM-dd"), "\\");
			string str1 = string.Concat(str, setting.Name, ".log");
			if (!Directory.Exists(str))
			{
				Directory.CreateDirectory(str);
			}
			File.AppendAllText(str1, text.ToString());
		}
	}
}