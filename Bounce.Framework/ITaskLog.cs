namespace Bounce.Framework {
    public interface ITaskLog {
        void BeginTask(ITask task, IBounceCommand command);
        void EndTask(ITask task, IBounceCommand command, TaskResult result);
        void BeginTarget(ITask task, string name, IBounceCommand command);
        void EndTarget(ITask task, string name, IBounceCommand command, TaskResult result);
    }
}