namespace Bounce.Framework {
    internal class TaskException : BounceException {
        public ITask Task { get; private set; }

        public TaskException(ITask task, string message) : base(message) {
            Task = task;
        }
    }
}