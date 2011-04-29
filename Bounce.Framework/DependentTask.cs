namespace Bounce.Framework
{
    public class DependentTask<T> : TaskWithValue<T> {
        [Dependency]
        private readonly ITask DependencyTask;
        private readonly Task<T> Task;
        private T TaskValue;

        public DependentTask(ITask dependencyTask, Task<T> task)
        {
            DependencyTask = dependencyTask;
            Task = task;
        }

        public override void InvokeFuture(IBounceCommand command, IBounce bounce)
        {
            bounce.Invoke(command, Task);
            TaskValue = Task.Value;
        }

        protected override T GetValue()
        {
            return TaskValue;
        }
    }
}