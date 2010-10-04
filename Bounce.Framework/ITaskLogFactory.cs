using System.IO;

namespace Bounce.Framework {
    internal interface ITaskLogFactory {
        ILog CreateLogForTask(ITask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions);
    }
}