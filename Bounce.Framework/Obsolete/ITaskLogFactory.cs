using System.IO;

namespace Bounce.Framework.Obsolete {
    public interface ITaskLogFactory {
        ILog CreateLogForTask(IObsoleteTask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions);
    }
}