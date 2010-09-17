using System;

namespace Bounce.Framework {
    public class TaskWalker {
        public void Walk(ITask task, Action<ITask> beforeDependencies, Action<ITask> afterDependencies) {
            if (beforeDependencies != null) {
                beforeDependencies(task);
            }

            foreach (ITask bouncerDependency in task.GetNonNullDependencies()) {
                Walk(bouncerDependency, beforeDependencies, afterDependencies);
            }

            if (afterDependencies != null) {
                afterDependencies(task);
            }
        }
    }
}