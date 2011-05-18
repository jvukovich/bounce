using System;
using System.IO;

namespace Bounce.Framework {
    internal class TaskException : BounceException {
        public ITask Task { get; private set; }

        public TaskException(ITask task, string message)
            : base(String.Format("task {0} failed: {1}", task, message))
        {
            Task = task;
        }

        public TaskException(ITask task, Exception innerException)
            : base(String.Format("task {0} failed: {1}", task, innerException.Message), innerException)
        {
            Task = task;
        }

        public override void Explain(TextWriter stderr) {
            stderr.WriteLine(Message);
        }
    }
}