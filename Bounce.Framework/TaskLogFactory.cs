using System.IO;

namespace Bounce.Framework {
    class TaskLogFactory : ITaskLogFactory {
        public ILog CreateLogForTask(ITask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions) {
            return new Log(stdout, stderr, logOptions, new TaskLogMessageFormatter(task));
        }
    }
}