using System;

namespace Bounce.Framework {
    internal interface ILogMessageFormatter {
        string FormatLogMessage(DateTime now, LogLevel logLevel, object message);
    }
}