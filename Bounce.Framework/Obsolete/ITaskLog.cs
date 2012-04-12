namespace Bounce.Framework.Obsolete {
    public interface ITaskLog {
        void BeginTask(IObsoleteTask task, IBounceCommand command);
        void EndTask(IObsoleteTask task, IBounceCommand command, TaskResult result);
        void BeginTarget(IObsoleteTask task, string name, IBounceCommand command);
        void EndTarget(IObsoleteTask task, string name, IBounceCommand command, TaskResult result);
    }
}