using System.Collections.Generic;

namespace Bounce.Framework {
    public class AmbiguousTaskNameException : BounceException {
        private readonly string _taskName;
        private readonly IEnumerable<ITask> _tasks;

        public AmbiguousTaskNameException(string taskName, IEnumerable<ITask> tasks) {
            _taskName = taskName;
            _tasks = tasks;
        }

        public override void Explain(System.IO.TextWriter output) {
            output.WriteLine("multiple tasks with name `{0}", _taskName);
            output.WriteLine("qualify the task with its class name or namespace");
        }
    }
}