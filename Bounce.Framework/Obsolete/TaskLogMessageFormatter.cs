using System;

namespace Bounce.Framework.Obsolete {
    public class TaskLogMessageFormatter : ILogMessageFormatter {
        private readonly ITask task;

        public TaskLogMessageFormatter(ITask task) {
            this.task = task;
        }

        public string FormatLogMessage(DateTime now, LogLevel logLevel, object message) {
            return String.Format("{1} {2} {3}", now, task.GetType().Name, logLevel.ToString().ToLower(), message);
        }
    }
}