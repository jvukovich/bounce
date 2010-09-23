using System;
using System.IO;

namespace Bounce.Framework {
    class Bounce : ITargetBuilderBounce {
        private readonly TextWriter stdout;
        private readonly TextWriter stderr;
        public ILog Log { get; private set; }

        public LogOptions LogOptions { get; set; }

        public Bounce(TextWriter stdout, TextWriter stderr) {
            this.stdout = stdout;
            this.stderr = stderr;
            LogOptions = new LogOptions {CommandOutput = false, LogLevel = LogLevel.Warning};
        }

        public IDisposable LogForTask(ITask task) {
            Log = new TaskLog(task, stdout, stderr, LogOptions);
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

    class LogOptions {
        public LogLevel LogLevel;
        public bool CommandOutput;
    }

    enum LogLevel {
        Error,
        Warning,
        Info,
        Debug
    }
}