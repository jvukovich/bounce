using System;
using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public class TargetInvoker {
        public TaskWalker Walker;
        public HashSet<ITask> BuiltTasks;
        private readonly ITargetBuilderBounce Bounce;
        private readonly CleanAfterBuildRegister CleanAfterBuildRegister;
        private readonly OnceOnlyTaskInvoker OnceOnlyCleaner;
        private readonly OnceOnlyTaskInvoker OnceOnlyBuilder;
        private readonly OnceOnlyTaskInvoker OnceOnlyDescriber;

        public TargetInvoker(ITargetBuilderBounce bounce) {
            BuiltTasks = new HashSet<ITask>();
            Bounce = bounce;
            Walker = new TaskWalker();
            CleanAfterBuildRegister = new CleanAfterBuildRegister();
            OnceOnlyCleaner = new OnceOnlyTaskInvoker((task, command) => InvokeAndLog(task, command));
            OnceOnlyBuilder = new OnceOnlyTaskInvoker((task, command) => InvokeAndLog(task, command));
            OnceOnlyDescriber = new OnceOnlyTaskInvoker((task, command) => InvokeAndLog(task, command));
        }

        public void Invoke(IBounceCommand command, ITask task)
        {
            command.InvokeCommand(() => Build(task, command), () => Clean(task, command), () => Describe(task, command));
        }

        private void Describe(ITask task, IBounceCommand command) {
            Walker.Walk(new TaskDependency(task), null, dep => DescribeIfNotDescribed(dep, command));
        }

        private void DescribeIfNotDescribed(TaskDependency dep, IBounceCommand command) {
            OnceOnlyDescriber.EnsureInvokedAtLeastOnce(dep.Task, command);
        }

        private void Build(ITask task, IBounceCommand command) {
            Walker.Walk(new TaskDependency(task), null, dep => BuildIfNotAlreadyBuilt(dep, command));
            RegisterCleanupAfterBuild(task);
        }

        private void RegisterCleanupAfterBuild(ITask task) {
            Walker.Walk(new TaskDependency(task), CleanAfterBuildRegister.RegisterDependency, null);
        }

        public void CleanAfterBuild(IBounceCommand command) {
            IBounceCommand cleanAfterBuildCommand = command.CleanAfterBuildCommand;
            if (cleanAfterBuildCommand != null) {
                foreach (var taskToClean in CleanAfterBuildRegister.TasksToBeCleaned) {
                    Clean(taskToClean, cleanAfterBuildCommand);
                }
            }
        }

        private void BuildIfNotAlreadyBuilt(TaskDependency dep, IBounceCommand command) {
            OnceOnlyBuilder.EnsureInvokedAtLeastOnce(dep.Task, command);
        }

        private void Clean(ITask task, IBounceCommand command) {
            Walker.Walk(new TaskDependency(task), dep => CleanIfNotAlreadyCleaned(dep, command), null);
        }

        private void CleanIfNotAlreadyCleaned(TaskDependency dep, IBounceCommand command) {
            OnceOnlyCleaner.EnsureInvokedAtLeastOnce(dep.Task, command);
        }

        class OnceOnlyTaskInvoker {
            private readonly Action<ITask, IBounceCommand> Invoke;
            private HashSet<ITask> InvokedTasks;

            public OnceOnlyTaskInvoker(Action<ITask, IBounceCommand> invoke) {
                Invoke = invoke;
                InvokedTasks = new HashSet<ITask>();
            }

            public void EnsureInvokedAtLeastOnce(ITask task, IBounceCommand command) {
                if (!InvokedTasks.Contains(task)) {
                    Invoke(task, command);
                    InvokedTasks.Add(task);
                }
            }
        }

        private void InvokeAndLog(ITask task, IBounceCommand command)
        {
            using (var taskScope = Bounce.TaskScope(task, command, null)) {
                try {
                    task.Describe(Bounce.DescriptionOutput);
                    task.Invoke(command, Bounce);
                    taskScope.TaskSucceeded();
                } catch (BounceException) {
                    throw;
                } catch (Exception e) {
                    throw new TaskException(task, e);
                }
            }
        }
    }
}