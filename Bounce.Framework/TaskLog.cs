using System;
using System.IO;

namespace Bounce.Framework {
    class TaskLog : ITaskLog {
        private TextWriter StdOut, StdErr;
        private readonly LogOptions logOptions;

        public TaskLog(TextWriter stdOut, TextWriter stdErr, LogOptions logOptions) {
            StdOut = stdOut;
            StdErr = stdErr;
            this.logOptions = logOptions;
        }

        public void BeginTask(ITask task, IBounceCommand command) {
            if (logOptions.ReportTaskStart && task.IsLogged && command.IsLogged) {
                StdOut.WriteLine("{0} task: {1}", command.PresentTense, task);
            }
        }

        public void EndTask(ITask task, IBounceCommand command, TaskResult result) {
            if (logOptions.ReportTaskEnd && task.IsLogged && command.IsLogged) {
                if (result == TaskResult.Success) {
                    StdOut.WriteLine("{0} task: {1}", command.PastTense, task);
                } else {
                    StdErr.WriteLine("failed to {0} task: {1}", command.InfinitiveTense, task);
                }
            }
        }

        public void BeginTarget(ITask task, string name, IBounceCommand command) {
            if (logOptions.ReportTargetStart) {
                StdOut.WriteLine("{0} target: {1}", command.PresentTense, name);
            }
        }

        public void EndTarget(ITask task, string name, IBounceCommand command, TaskResult result) {
            if (logOptions.ReportTargetEnd) {
                if (result == TaskResult.Success) {
                    StdOut.WriteLine("{0} target: {1}", command.PastTense, name);
                } else {
                    StdErr.WriteLine("failed to {0} target: {1}", command.InfinitiveTense, name);
                }
            }
        }
    }
}