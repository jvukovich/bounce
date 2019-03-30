using System;
using System.IO;
//using Bounce.Framework.TeamCity;

namespace Bounce.Framework {
    public class Log : ILog {
        private readonly TextWriter Stdout;
        private readonly TextWriter Stderr;
        private readonly LogLevel LogLevel;
        private readonly ILogMessageFormatter LogMessageFormatter;

        public Log(LogLevel logLevel)
            : this(logLevel, new DefaultLogFormatter())
        {}

        public Log(LogLevel logLevel, ILogMessageFormatter logMessageFormatter)
        {
            Stdout = Console.Out;
            Stderr = Console.Error;
            LogLevel = logLevel;
            LogMessageFormatter = logMessageFormatter;
        }

        public virtual void Debug(string format, params object[] args)
        {
            WriteLogMessage(Stdout, LogLevel.Debug, String.Format(format, args));
        }

        public virtual void Debug(object message)
        {
            WriteLogMessage(Stdout, LogLevel.Debug, message);
        }

        public virtual void Info(string format, params object[] args)
        {
            WriteLogMessage(Stdout, LogLevel.Info, String.Format(format, args));
        }

        public virtual void Info(object message)
        {
            WriteLogMessage(Stdout, LogLevel.Info, message);
        }

        public virtual void Warning(string format, params object[] args)
        {
            WriteLogMessage(Stdout, LogLevel.Warning, String.Format(format, args));
        }

        public virtual void Warning(object message)
        {
            WriteLogMessage(Stdout, LogLevel.Warning, message);
        }

        public virtual void Warning(Exception exception, string format, params object[] args)
        {
            LogException(Stdout, LogLevel.Warning, String.Format(format, args), exception);
        }

        public virtual void Warning(Exception exception, object message)
        {
            LogException(Stdout, LogLevel.Warning, message, exception);
        }

        public virtual void Error(string format, params object[] args) {
            WriteLogMessage(Stderr, LogLevel.Error, String.Format(format, args));
        }

        public virtual void Error(object message)
        {
            WriteLogMessage(Stderr, LogLevel.Error, message);
        }

        public virtual void Error(Exception exception, string format, params object[] args)
        {
            LogException(Stderr, LogLevel.Error, String.Format(format, args), exception);
        }

        public virtual void Error(Exception exception, object message)
        {
            LogException(Stderr, LogLevel.Error, message, exception);
        }

        private void WriteLogMessage(TextWriter output, LogLevel logLevel, object message) {
            if (logLevel <= LogLevel) {
                output.WriteLine(LogMessageFormatter.FormatLogMessage(DateTime.Now, logLevel, message));
            }
        }

        private void LogException(TextWriter output, LogLevel logLevel, object message, Exception exception) {
            WriteLogMessage(output, logLevel, message);
            WriteLogMessage(output, logLevel, exception);
        }

        public static ILog Default {
            get {
                //if (TeamCityFormatter.IsActive) {
                //    return new TeamCityLog(DefaultLogLevel);
                //} else {
                    return new Log(DefaultLogLevel);
                //}
            }
        }

        public static LogLevel DefaultLogLevel {
            get { return (LogLevel)Enum.Parse(typeof(LogLevel), Parameters.Main.Parameter("loglevel", "info"), true); }
        }
    }
}