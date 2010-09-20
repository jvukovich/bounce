using System;

namespace Bounce.Framework {
    internal interface ILogMessageFormatter {
        string FormatLogMessage(DateTime now, string logEntryType, object message);
    }
}