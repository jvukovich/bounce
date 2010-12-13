using System;
using System.IO;

namespace Bounce.Framework {
    class Bounce : ITargetBuilderBounce {
        private readonly TextWriter stdout;
        private readonly TextWriter stderr;
        public ITaskLogFactory LogFactory { get; set; }

        public ILog Log { get; private set; }
        public IShellCommandExecutor ShellCommand { get; private set; }
        private TargetInvoker TargetInvoker;

        public LogOptions LogOptions { get; set; }

        public Bounce(TextWriter stdout, TextWriter stderr) {
            this.stdout = stdout;
            this.stderr = stderr;
            LogFactory = new TaskLogFactory();
            LogOptions = new LogOptions {CommandOutput = false, LogLevel = LogLevel.Warning, ReportTargetEnd = true};
            ShellCommand = new ShellCommandExecutor(() => Log);
            TargetInvoker = new TargetInvoker(this);
        }

        public ITaskScope TaskScope(ITask task, BounceCommand command, string targetName) {
            return CreateTaskScope(task, command, targetName);
        }

        private ITaskScope CreateTaskScope(ITask task, BounceCommand command, string targetName) {
            ILog previousLogger = Log;
            Log = LogFactory.CreateLogForTask(task, stdout, stderr, LogOptions);
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

        private void EndTaskLog(ITask task, BounceCommand command, TaskResult result, string targetName, ILog outerLogger) {
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
                    return stdout;
                } else
                {
                    return TextWriter.Null;
                }
            }
        }

        public void Invoke(BounceCommand command, ITask task) {
            TargetInvoker.Invoke(command, task);
        }
    }

    public interface ITaskScope : IDisposable {
        void TaskSucceeded();
    }
}