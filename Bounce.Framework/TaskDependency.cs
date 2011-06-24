namespace Bounce.Framework {
    public class TaskDependency {
        public ITask Task;
        public string Name;
        public bool CleanAfterBuild;

        public TaskDependency(ITask task)
        {
            Task = task;
        }
    }
}