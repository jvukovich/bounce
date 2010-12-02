using System.IO;

namespace Bounce.Framework {
    internal class TeamCityLogFactory : ITaskLogFactory {
        public ILog CreateLogForTask(ITask task, TextWriter stdout, TextWriter stderr, LogOptions logOptions) {
            return new TeamCityLog(stdout, logOptions);
        }
    }
}