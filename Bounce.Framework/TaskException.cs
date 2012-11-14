using System;

namespace Bounce.Framework {
    public class TaskException : BounceException {
        public TaskException(TaskMethod task, Exception innerException) :
            base(string.Format("task {0} threw an exception", task.FullName), innerException) {
        }

        public override void Explain(System.IO.TextWriter stderr) {
            stderr.WriteLine(InnerException);
        }
    }
}