namespace Bounce.Framework {
    public interface ITaskLog {
        void BeginTask(ITask task, BounceCommand command);
        void EndTask(ITask task, BounceCommand command, TaskResult result);
        void BeginTarget(ITask task, string name, BounceCommand command);
        void EndTarget(ITask task, string name, BounceCommand command, TaskResult result);
    }
}