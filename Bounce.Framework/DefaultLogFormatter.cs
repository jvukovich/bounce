using System;

namespace Bounce.Framework {
    public class DefaultLogFormatter : ILogMessageFormatter {
        public string FormatLogMessage(DateTime now, LogLevel logLevel, object message) {
            return string.Format("{0}: {1} {2}", now, logLevel, message);
        }
    }
}