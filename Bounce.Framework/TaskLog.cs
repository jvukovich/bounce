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

        public void BeginTask(ITask task, BounceCommand command) {
            if (logOptions.ReportTaskStart && task.IsLogged) {
                StdOut.WriteLine("{0} task: {1}", StartingCommand(command), task);
            }
        }

        public void EndTask(ITask task, BounceCommand command, TaskResult result) {
            if (logOptions.ReportTaskEnd && task.IsLogged) {
                if (result == TaskResult.Success) {
                    StdOut.WriteLine("{0} task: {1}", EndingCommand(command), task);
                } else {
                    StdErr.WriteLine("failed to {0} task: {1}", InfinitiveCommand(command), task);
                }
            }
        }

        public void BeginTarget(ITask task, string name, BounceCommand command) {
            if (logOptions.ReportTargetStart && task.IsLogged) {
                StdOut.WriteLine("{0} target: {1}", StartingCommand(command), name);
            }
        }

        public void EndTarget(ITask task, string name, BounceCommand command, TaskResult result) {
            if (logOptions.ReportTargetEnd && task.IsLogged) {
                if (result == TaskResult.Success) {
                    StdOut.WriteLine("{0} target: {1}", EndingCommand(command), name);
                } else {
                    StdErr.WriteLine("failed to {0} target: {1}", InfinitiveCommand(command), name);
                }
            }
        }

        private string StartingCommand(BounceCommand command) {
            switch (command) {
                case BounceCommand.Build:
                    return "building";
                case BounceCommand.Clean:
                    return "cleaning";
                default:
                    throw new Exception("didn't expect this: " + command);
            }
        }

        private string EndingCommand(BounceCommand command) {
            switch (command) {
                case BounceCommand.Build:
                    return "built";
                case BounceCommand.Clean:
                    return "cleaned";
                default:
                    throw new Exception("didn't expect this: " + command);
            }
        }

        private string InfinitiveCommand(BounceCommand command) {
            return command.ToString().ToLower();
        }
    }
}