using System.IO;

namespace Bounce.Framework {
    class TaskLog : Log {
        public TaskLog(ITask task, TextWriter stdout, TextWriter stderr) : base(stdout, stderr, new TaskLogMessageFormatter(task)) {}
    }
}