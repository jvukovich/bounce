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
                return deps.Where(d => d != null && d.Task != null);
            }
        }
    }

    public class TaskAggregator {
        Stack<ITaskAggregate> Stack = new Stack<ITaskAggregate>();
        private Func<TaskDependency, ITaskAggregate> CreateAggregate;

        public TaskAggregator(Func<TaskDependency, ITaskAggregate> createAggregate) {
            CreateAggregate = createAggregate;
        }

        public void Before(TaskDependency dep) {
            if (Stack.Count > 0) {
                var taskAggregate = Stack.Peek();
                taskAggregate.Add(dep);
            }
            Stack.Push(CreateAggregate(dep));
        }

        public void After(TaskDependency dep) {
            Stack.Pop().Finally();
        }
    }

    public interface ITaskAggregate {
        void Add(TaskDependency dep);
        void Finally();
    }
}