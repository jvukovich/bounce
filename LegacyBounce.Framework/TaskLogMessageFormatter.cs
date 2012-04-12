using System;

namespace LegacyBounce.Framework {
    public class TaskLogMessageFormatter : ILogMessageFormatter {
        private readonly IObsoleteTask task;

        public TaskLogMessageFormatter(IObsoleteTask task) {
            this.task = task;
        }

        public string FormatLogMessage(DateTime now, LogLevel logLevel, object message) {
            return String.Format("{1} {2} {3}", now, task.GetType().Name, logLevel.ToString().ToLower(), message);
        }
    }
}