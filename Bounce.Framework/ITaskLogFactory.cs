using System.IO;

namespace Bounce.Framework {
    public interface ITaskLogFactory {
        ILog CreateLogForTask(ITask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions);
    }
}