using System;
using System.IO;

namespace Bounce.Framework {
    class Bounce : ITargetBuilderBounce {
        private readonly TextWriter stdout;
        private readonly TextWriter stderr;
        public ILog Log { get; private set; }

        public Bounce(TextWriter stdout, TextWriter stderr) {
            this.stdout = stdout;
            this.stderr = stderr;
        }

        public IDisposable LogForTask(ITask task) {
            Log = new TaskLog(task, stdout, stderr);
            return new TaskLogRemover(this);
        }

        class TaskLogRemover : IDisposable {
            private readonly Bounce bounce;

            public TaskLogRemover(Bounce bounce) {
                this.bounce = bounce;
            }

            public void Dispose() {
                bounce.Log = null;
            }
        }
    }
}