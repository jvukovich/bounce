using System;

namespace Bounce.Framework
{
    public interface ILogMessageFormatter
    {
        string FormatLogMessage(DateTime now, LogLevel logLevel, object message);
    }
}