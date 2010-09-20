using System.Collections.Generic;

namespace Bounce.Framework {
    public class TargetBuilder {
        public TaskWalker Walker;
        public HashSet<ITask> BuiltTasks;
        private readonly ITargetBuilderBounce Bounce;

        public TargetBuilder(ITargetBuilderBounce bounce) {
            BuiltTasks = new HashSet<ITask>();
            Bounce = bounce;
            Walker = new TaskWalker();
        }

        public void Build(ITask task) {
            Walker.Walk(task, BeforeBuildAndLog, BuildIfNotAlreadyBuilt);
        }

        private void BeforeBuildAndLog(ITask task) {
            using (Bounce.LogForTask(task)) {
                task.BeforeBuild(Bounce);
            }
        }

        private void BuildIfNotAlreadyBuilt(ITask task) {
            if (!BuiltTasks.Contains(task)) {
                BuildAndLog(task);
                BuiltTasks.Add(task);
            }
        }

        private void BuildAndLog(ITask task) {
            using (Bounce.LogForTask(task)) {
                task.Build(Bounce);
            }
        }

        public void Clean(ITask task) {
            Walker.Walk(task, CleanAndLog, null);
        }

        private void CleanAndLog(ITask task) {
            using (Bounce.LogForTask(task)) {
                task.Clean(Bounce);
            }
        }
    }
}