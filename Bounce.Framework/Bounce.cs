using System;
using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    class Bounce : ITargetBuilderBounce {
        public ITaskLogFactory LogFactory { get; set; }

        public ILog Log { get; private set; }
        public IShellCommandExecutor ShellCommand { get; private set; }
        private TargetInvoker TargetInvoker;

        public LogOptions LogOptions { get; set; }

        public Bounce() {
            LogFactory = new TaskLogFactory();
            LogOptions = new LogOptions {CommandOutput = false, LogLevel = LogLevel.Warning, ReportTargetEnd = true};
            ShellCommand = new ShellCommandExecutor(() => Log);
            TargetInvoker = new TargetInvoker(this);
        }

        public ITaskScope TaskScope(ITask task, IBounceCommand command, string targetName) {
            return CreateTaskScope(task, command, targetName);
        }

        private ITaskScope CreateTaskScope(ITask task, IBounceCommand command, string targetName) {
            ILog previousLogger = Log;
            Log = LogFactory.CreateLogForTask(task, LogOptions.StdOut, LogOptions.StdErr, LogOptions);
            if (targetName != null) {
                Log.TaskLog.BeginTarget(task, targetName, command);
            } else {
                Log.TaskLog.BeginTask(task, command);
            }
            return new TaskScope(
                outerLogger => EndTaskLog(task, command, TaskResult.Success, targetName, outerLogger),
                outerLogger => EndTaskLog(task, command, TaskResult.Failure, targetName, outerLogger),
                previousLogger);
        }

        private void EndTaskLog(ITask task, IBounceCommand command, TaskResult result, string targetName, ILog outerLogger) {
            if (targetName != null) {
                Log.TaskLog.EndTarget(task, targetName, command, result);
            } else {
                Log.TaskLog.EndTask(task, command, result);
            }
            Log = outerLogger;
        }

        public TextWriter DescriptionOutput {
            get
            {
                if (LogOptions.DescribeTasks)
                {
                    return LogOptions.StdOut;
                } else
                {
                    return TextWriter.Null;
                }
            }
        }

        public IEnumerable<IParameter> ParametersGiven { get; set; }

        public void Invoke(IBounceCommand command, ITask task) {
            TargetInvoker.Invoke(command, task);
        }

        public void CleanAfterBuild(IBounceCommand command) {
            TargetInvoker.CleanAfterBuild(command);
        }
    }

    public interface ITaskScope : IDisposable {
        void TaskSucceeded();
    }
}