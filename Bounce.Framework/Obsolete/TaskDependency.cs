namespace Bounce.Framework.Obsolete {
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