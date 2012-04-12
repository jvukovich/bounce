using System;

namespace LegacyBounce.Framework {
    public interface ILogMessageFormatter {
        string FormatLogMessage(DateTime now, LogLevel logLevel, object message);
    }
}