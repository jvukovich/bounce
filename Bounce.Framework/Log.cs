using System;
using System.IO;

namespace Bounce.Framework {
    class Log : ILog {
        private const string warningType = "warning";
        private const string errorType = "error";
        private const string infoType = "info";
        private const string debugType = "debug";
        private readonly TextWriter Stdout;
        private readonly TextWriter Stderr;
        private readonly ILogMessageFormatter logMessageFormatter;

        public Log(TextWriter stdout, TextWriter stderr, ILogMessageFormatter logMessageFormatter) {
            Stdout = stdout;
            Stderr = stderr;
            this.logMessageFormatter = logMessageFormatter;
        }

        public void Debug(string format, params object[] args) {
            WriteLogMessage(Stdout, debugType, String.Format(format, args));
        }

        public void Debug(object message) {
            WriteLogMessage(Stdout, debugType, message);
        }

        public ICommandLog BeginExecutingCommand(string command, string args) {
            return new CommandLog(command, args, Stdout, Stderr);
        }

        public void Info(string format, params object[] args) {
            WriteLogMessage(Stdout, infoType, String.Format(format, args));
        }

        public void Info(object message) {
            WriteLogMessage(Stdout, infoType, message);
        }

        public void Warning(string format, params object[] args) {
            WriteLogMessage(Stdout, warningType, String.Format(format, args));
        }

        public void Warning(object message) {
            WriteLogMessage(Stdout, warningType, message);
        }

        public void Warning(Exception exception, string format, params object[] args) {
            LogException(Stdout, warningType, String.Format(format, args), exception);
        }

        public void Warning(Exception exception, object message) {
            LogException(Stdout, warningType, message, exception);
        }

        public void Error(string format, params object[] args) {
            WriteLogMessage(Stderr, errorType, String.Format(format, args));
        }

        public void Error(object message) {
            WriteLogMessage(Stderr, errorType, message);
        }

        public void Error(Exception exception, string format, params object[] args) {
            LogException(Stderr, errorType, String.Format(format, args), exception);
        }

        public void Error(Exception exception, object message) {
            LogException(Stderr, errorType, message, exception);
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