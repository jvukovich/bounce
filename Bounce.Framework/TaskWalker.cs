using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class TaskWalker {
        public void Walk(TaskDependency task, Action<TaskDependency> beforeDependencies, Action<TaskDependency> afterDependencies) {
            if (beforeDependencies != null) {
                beforeDependencies(task);
            }

            foreach (TaskDependency dependency in GetNonNullDependencies(task.Task)) {
                Walk(dependency, beforeDependencies, afterDependencies);
            }

            if (afterDependencies != null) {
                afterDependencies(task);
            }
        }

        private static IEnumerable<TaskDependency> GetNonNullDependencies(ITask task) {
            var deps = task.Dependencies;
            if (deps == null) {
                return new TaskDependency[0];
            } else {
                return deps.Where(d => d != null);
            }
        }
    }
}