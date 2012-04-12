using System.Collections.Generic;
using System.Reflection;
using Bounce.Framework.Obsolete;

namespace Bounce.Framework {
    public class AmbiguousTaskNameException : BounceException {
        private readonly string _taskName;
        private readonly IEnumerable<IObsoleteTask> _tasks;

        public AmbiguousTaskNameException(string taskName, IEnumerable<IObsoleteTask> tasks) {
            _taskName = taskName;
            _tasks = tasks;
        }

        public override void Explain(System.IO.TextWriter output) {
            output.WriteLine("multiple tasks with name `{0}", _taskName);
            output.WriteLine("qualify the task with its class name or namespace");
        }
    }
}