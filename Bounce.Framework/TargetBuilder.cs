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
            using (var taskScope = Bounce.TaskScope(task, BounceCommand.Build, null)) {
                task.BeforeBuild(Bounce);
                taskScope.TaskSucceeded();
            }
        }

        private void BuildIfNotAlreadyBuilt(ITask task) {
            if (!BuiltTasks.Contains(task)) {
                BuildAndLog(task);
                BuiltTasks.Add(task);
            }
        }

        private void BuildAndLog(ITask task) {
            using (var taskScope = Bounce.TaskScope(task, BounceCommand.Build, null)) {
                task.Build(Bounce);
                taskScope.TaskSucceeded();
            }
        }

        public void Clean(ITask task) {
            Walker.Walk(task, CleanAndLog, null);
        }

        private void CleanAndLog(ITask task) {
            using (var taskScope = Bounce.TaskScope(task, BounceCommand.Clean, null)) {
                task.Clean(Bounce);
                taskScope.TaskSucceeded();
            }
        }
    }
}