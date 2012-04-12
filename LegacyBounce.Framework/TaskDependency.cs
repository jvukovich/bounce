namespace LegacyBounce.Framework {
    public class TaskDependency {
        public IObsoleteTask Task;
        public string Name;
        public bool CleanAfterBuild;

        public TaskDependency(IObsoleteTask task)
        {
            Task = task;
        }
    }
}