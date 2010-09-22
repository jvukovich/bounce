using System;
using System.IO;

namespace Bounce.Framework {
    internal class TaskException : BounceException {
        public ITask Task { get; private set; }

        public TaskException(ITask task, string message) : base(message) {
            Task = task;
        }

        public override void Explain(TextWriter stderr) {
            stderr.WriteLine("task {0} failed: {1}", Task, Message);
        }
    }
}