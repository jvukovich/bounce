using System.Collections.Generic;

namespace Bounce.Framework {
    public class NoMatchingTaskException : BounceException {
        private readonly string _taskName;
        private readonly IEnumerable<ITask> _tasks;

        public NoMatchingTaskException(string taskName, IEnumerable<ITask> tasks) {
            _taskName = taskName;
            _tasks = tasks;
        }

        public override void Explain(System.IO.TextWriter stderr) {
            stderr.WriteLine("task `{0}' not found, try one of the following:", _taskName);
            UsageHelp.WriteAvailableTasks(stderr, _tasks);
        }
    }
}