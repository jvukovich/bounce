using System.IO;

namespace Bounce.Framework {
    class TaskLog : Log {
        public TaskLog(ITask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions) : base(stdout, stderr, logOptions, new TaskLogMessageFormatter(task)) {}
    }
}