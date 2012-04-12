using System.IO;

namespace LegacyBounce.Framework {
    class TaskLogFactory : ITaskLogFactory {
        public ILog CreateLogForTask(IObsoleteTask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions) {
            return new Log(stdout, stderr, logOptions, new TaskLogMessageFormatter(task));
        }
    }
}