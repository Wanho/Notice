using System;

namespace Notice.Core
{
    public interface ILog
    {
        string Parameter { get; set; }

        void Debug(string _namespace, string text, LogInfo i = null, Exception e = null);
        void Debug(string _namespace, string text, Exception e);
        void Debug(string text, LogInfo i = null, Exception e = null);
        void Debug(string text, Exception e);

        void Error(string _namespace, string text, LogInfo i = null, Exception e = null);
        void Error(string _namespace, string text, Exception e);
        void Error(string text, LogInfo i = null, Exception e = null);
        void Error(string text, Exception e);

        void Fatal(string _namespace, string text, LogInfo i = null, Exception e = null);
        void Fatal(string _namespace, string text, Exception e);
        void Fatal(string text, LogInfo i = null, Exception e = null);
        void Fatal(string text, Exception e);

        void Info(string _namespace, string text, LogInfo i = null, Exception e = null);
        void Info(string _namespace, string text, Exception e);
        void Info(string text, LogInfo i = null, Exception e = null);
        void Info(string text, Exception e);

        void Warn(string _namespace, string text, LogInfo i = null, Exception e = null);
        void Warn(string _namespace, string text, Exception e);
        void Warn(string text, LogInfo i = null, Exception e = null);
        void Warn(string text, Exception e);
    }

    public interface ILogService
    {
        void Create(LogEntity logEntity);
    }
}
