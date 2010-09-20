using System;
using System.IO;

namespace Bounce.Framework {
    class Log : ILog {
        private const string warningType = "warning";
        private const string errorType = "error";
        private const string infoType = "info";
        private const string debugType = "debug";
        private TextWriter stdout;
        private readonly TextWriter stderr;
        private readonly ILogMessageFormatter logMessageFormatter;

        public Log(TextWriter stdout, TextWriter stderr, ILogMessageFormatter logMessageFormatter) {
            this.stdout = stdout;
            this.stderr = stderr;
            this.logMessageFormatter = logMessageFormatter;
        }

        public void Debug(string format, params object[] args) {
            WriteLogMessage(stdout, debugType, String.Format(format, args));
        }

        public void Debug(object message) {
            WriteLogMessage(stdout, debugType, message);
        }

        public void Info(string format, params object[] args) {
            WriteLogMessage(stdout, infoType, String.Format(format, args));
        }

        public void Info(object message) {
            WriteLogMessage(stdout, infoType, message);
        }

        public void Warning(string format, params object[] args) {
            WriteLogMessage(stdout, warningType, String.Format(format, args));
        }

        public void Warning(object message) {
            WriteLogMessage(stdout, warningType, message);
        }

        public void Warning(Exception exception, string format, params object[] args) {
            LogException(stdout, warningType, String.Format(format, args), exception);
        }

        public void Warning(Exception exception, object message) {
            LogException(stdout, warningType, message, exception);
        }

        public void Error(string format, params object[] args) {
            WriteLogMessage(stderr, errorType, String.Format(format, args));
        }

        public void Error(object message) {
            WriteLogMessage(stderr, errorType, message);
        }

        public void Error(Exception exception, string format, params object[] args) {
            LogException(stderr, errorType, String.Format(format, args), exception);
        }

        public void Error(Exception exception, object message) {
            LogException(stderr, errorType, message, exception);
        }

        private void WriteLogMessage(TextWriter output, string logEntryType, object message) {
            output.WriteLine(logMessageFormatter.FormatLogMessage(DateTime.Now, logEntryType, message));
        }

        private void LogException(TextWriter output, string logEntryType, object message, Exception exception) {
            WriteLogMessage(output, logEntryType, message);
            WriteLogMessage(output, logEntryType, exception);
        }
    }
}