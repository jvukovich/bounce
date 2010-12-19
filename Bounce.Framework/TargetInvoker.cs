using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class TargetInvoker {
        public TaskWalker Walker;
        public HashSet<ITask> BuiltTasks;
        private readonly ITargetBuilderBounce Bounce;
        private readonly CleanAfterBuildRegister CleanAfterBuildRegister;

        public TargetInvoker(ITargetBuilderBounce bounce) {
            BuiltTasks = new HashSet<ITask>();
            Bounce = bounce;
            Walker = new TaskWalker();
            CleanAfterBuildRegister = new CleanAfterBuildRegister();
        }

        public void Invoke(BounceCommand command, ITask task)
        {
            command.InvokeCommand(() => Build(task), () => Clean(task));
        }

        private void Build(ITask task) {
            Walker.Walk(new TaskDependency {Task = task}, null, BuildIfNotAlreadyBuilt);
            RegisterCleanupAfterBuild(task);
        }

        private void RegisterCleanupAfterBuild(ITask task) {
            Walker.Walk(new TaskDependency {Task = task}, CleanAfterBuildRegister.RegisterDependency, null);
        }

        public void CleanAfterBuild() {
            foreach (var taskToClean in CleanAfterBuildRegister.TasksToBeCleaned) {
                InvokeAndLog(taskToClean, BounceCommand.Clean);
            }
        }

        private void BuildIfNotAlreadyBuilt(TaskDependency dep) {
            if (!BuiltTasks.Contains(dep.Task)) {
                BuildAndLog(dep.Task);
                BuiltTasks.Add(dep.Task);
            }
        }

        private void BuildAndLog(ITask task) {
            InvokeAndLog(task, BounceCommand.Build);
        }

        private void Clean(ITask task) {
            Walker.Walk(new TaskDependency {Task = task}, CleanAndLog, null);
        }

        private void CleanAndLog(TaskDependency dep)
        {
            InvokeAndLog(dep.Task, BounceCommand.Clean);
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