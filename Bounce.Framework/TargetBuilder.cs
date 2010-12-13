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
            Walker.Walk(task, null, BuildIfNotAlreadyBuilt);
        }

        private void BuildIfNotAlreadyBuilt(ITask task) {
            if (!BuiltTasks.Contains(task)) {
                BuildAndLog(task);
                BuiltTasks.Add(task);
            }
        }

        private void BuildAndLog(ITask task) {
            InvokeAndLog(task, BounceCommand.Build);
        }

        public void Clean(ITask task) {
            Walker.Walk(task, CleanAndLog, null);
        }

        private void CleanAndLog(ITask task)
        {
            InvokeAndLog(task, BounceCommand.Clean);
        }

        private void InvokeAndLog(ITask task, BounceCommand command)
        {
            using (var taskScope = Bounce.TaskScope(task, command, null)) {
                task.Describe(Bounce.DescriptionOutput);
                task.Invoke(command, Bounce);
                taskScope.TaskSucceeded();
            }
        }
    }
}