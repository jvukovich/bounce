using System.IO;

namespace LegacyBounce.Framework {
    public interface ITaskLogFactory {
        ILog CreateLogForTask(IObsoleteTask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions);
    }
}