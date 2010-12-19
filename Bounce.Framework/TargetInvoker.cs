using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class TargetInvoker {
        public TaskWalker Walker;
        public HashSet<ITask> BuiltTasks;
        private readonly ITargetBuilderBounce Bounce;
        private readonly CleanAfterBuildRegister CleanAfterBuildRegister;
        private readonly OnceOnlyTaskInvoker OnceOnlyCleaner;
        private readonly OnceOnlyTaskInvoker OnceOnlyBuilder;

        public TargetInvoker(ITargetBuilderBounce bounce) {
            BuiltTasks = new HashSet<ITask>();
            Bounce = bounce;
            Walker = new TaskWalker();
            CleanAfterBuildRegister = new CleanAfterBuildRegister();
            OnceOnlyCleaner = new OnceOnlyTaskInvoker(task => InvokeAndLog(task, BounceCommand.Clean));
            OnceOnlyBuilder = new OnceOnlyTaskInvoker(task => InvokeAndLog(task, BounceCommand.Build));
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
                Clean(taskToClean);
            }
        }

        private void BuildIfNotAlreadyBuilt(TaskDependency dep) {
            OnceOnlyBuilder.EnsureInvokedAtLeastOnce(dep.Task);
        }

        private void Clean(ITask task) {
            Walker.Walk(new TaskDependency {Task = task}, CleanIfNotAlreadyCleaned, null);
        }

        private void CleanIfNotAlreadyCleaned(TaskDependency dep) {
            OnceOnlyCleaner.EnsureInvokedAtLeastOnce(dep.Task);
        }

        class OnceOnlyTaskInvoker {
            private readonly Action<ITask> Invoke;
            private HashSet<ITask> InvokedTasks;

            public OnceOnlyTaskInvoker(Action<ITask> invoke) {
                Invoke = invoke;
                InvokedTasks = new HashSet<ITask>();
            }

            public void EnsureInvokedAtLeastOnce(ITask task) {
                if (!InvokedTasks.Contains(task)) {
                    Invoke(task);
                    InvokedTasks.Add(task);
                }
            }
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