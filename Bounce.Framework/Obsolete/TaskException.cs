using System;

namespace Bounce.Framework.Obsolete {
    internal class TaskException : BounceException {
        public IObsoleteTask Task { get; private set; }

        public TaskException(IObsoleteTask task, string message)
            : base(String.Format("task {0} failed: {1}", task, message))
        {
            Task = task;
        }

        public TaskException(IObsoleteTask task, Exception innerException)
            : base(String.Format("task {0} failed: {1}", task, innerException.Message), innerException)
        {
            Task = task;
        }
    }
}