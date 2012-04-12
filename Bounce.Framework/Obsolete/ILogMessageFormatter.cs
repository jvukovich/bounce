using System;

namespace Bounce.Framework.Obsolete {
    public interface ILogMessageFormatter {
        string FormatLogMessage(DateTime now, LogLevel logLevel, object message);
    }
}