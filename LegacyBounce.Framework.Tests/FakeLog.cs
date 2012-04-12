using System;

namespace LegacyBounce.Framework.Tests {
    public class FakeLog : ILog {
        public virtual void Debug(string format, params object[] args) {
        }

        public virtual void Debug(object message) {
        }

        public virtual void Info(string format, params object[] args) {
        }

        public virtual void Info(object message) {
        }

        public virtual void Warning(string format, params object[] args) {
        }

        public virtual void Warning(Exception exception, string format, params object[] args) {
        }

        public virtual void Warning(object message) {
        }

        public virtual void Warning(Exception exception, object message) {
        }

        public virtual void Error(string format, params object[] args) {
        }

        public virtual void Error(Exception exception, string format, params object[] args) {
        }

        public virtual void Error(object message) {
        }

        public virtual void Error(Exception exception, object message) {
        }

        public virtual ICommandLog BeginExecutingCommand(string command, string args) {
            return new FakeCommandLog(command, args);
        }

        public ITaskLog TaskLog { get; set; }
    }

    public class FakeTaskLog : ITaskLog
    {
        public void BeginTask(IObsoleteTask task, IBounceCommand command)
        {
        }

        public void EndTask(IObsoleteTask task, IBounceCommand command, TaskResult result)
        {
        }

        public void BeginTarget(IObsoleteTask task, string name, IBounceCommand command)
        {
        }

        public void EndTarget(IObsoleteTask task, string name, IBounceCommand command, TaskResult result)
        {
        }
    }

    public class FakeCommandLog : ICommandLog {
        private readonly string Args;

        public FakeCommandLog(string command, string args) {
            Args = args;
        }

        public void CommandOutput(string output) {
        }

        public void CommandError(string error) {
        }

        public void CommandComplete(int exitCode) {
        }

        public string CommandArgumentsForLogging {
            get { return Args; }
        }
    }
}