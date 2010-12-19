using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class TaskWalker {
        public void Walk(ITask task, Action<ITask> beforeDependencies, Action<ITask> afterDependencies) {
            if (beforeDependencies != null) {
                beforeDependencies(task);
            }

            foreach (ITask dependency in GetNonNullDependencies(task)) {
                Walk(dependency, beforeDependencies, afterDependencies);
            }

            if (afterDependencies != null) {
                afterDependencies(task);
            }
        }

        private static IEnumerable<ITask> GetNonNullDependencies(ITask task) {
            var deps = task.Dependencies;
            if (deps == null) {
                return new ITask[0];
            } else {
                return deps.Where(d => d != null).Select(d => d.Task);
            }
        }
    }
}