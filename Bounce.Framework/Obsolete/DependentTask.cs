namespace Bounce.Framework.Obsolete
{
    class DependentTask<T> : TaskWithValue<T> {
        [Dependency]
        private readonly ITask DependencyTask;
        private readonly Task<T> Task;
        private T TaskValue;

        public DependentTask(ITask dependencyTask, Task<T> task)
        {
            DependencyTask = dependencyTask;
            Task = task;
        }

        public override void InvokeTask(IBounceCommand command, IBounce bounce)
        {
            bounce.Invoke(command, Task);
            TaskValue = Task.Value;
        }

        protected override T GetValue()
        {
            return TaskValue;
        }
    }

    class DependentTask : Task
    {
        [Dependency]
        private readonly ITask DependencyTask;

        private readonly ITask Task;

        public DependentTask(ITask dependencyTask, ITask task)
        {
            DependencyTask = dependencyTask;
            Task = task;
        }

        public override void Invoke(IBounceCommand command, IBounce bounce)
        {
            bounce.Invoke(command, Task);
        }

        public override bool IsLogged
        {
            get { return false; }
        }
    }
}