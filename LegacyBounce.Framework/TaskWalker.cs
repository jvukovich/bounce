using System;
using System.Collections.Generic;
using System.Linq;

namespace LegacyBounce.Framework {
    public class TaskWalker {
        public void Walk(TaskDependency task, Action<TaskDependency> beforeDependencies, Action<TaskDependency> afterDependencies) {
            Walk(task, beforeDependencies, afterDependencies, new HashSet<IObsoleteTask>());
        }

        public void Walk(TaskDependency dep, Action<TaskDependency> beforeDependencies, Action<TaskDependency> afterDependencies, HashSet<IObsoleteTask> tasksAlreadyDone) {
            if (tasksAlreadyDone.Contains(dep.Task)) {
                return;
            }

            tasksAlreadyDone.Add(dep.Task);

            if (beforeDependencies != null) {
                beforeDependencies(dep);
            }

            foreach (TaskDependency dependency in GetNonNullDependencies(dep.Task)) {
                Walk(dependency, beforeDependencies, afterDependencies, tasksAlreadyDone);
            }

            if (afterDependencies != null) {
                afterDependencies(dep);
            }
        }

        private static IEnumerable<TaskDependency> GetNonNullDependencies(IObsoleteTask task) {
            var deps = task.Dependencies;
            if (deps == null) {
                return new TaskDependency[0];
            } else {
                return deps.Where(d => d != null && d.Task != null);
            }
        }
    }

    public class TaskAggregator<T> where T : ITaskAggregate<T> {
        Stack<T> Stack = new Stack<T>();
        private Func<TaskDependency, T> CreateAggregate;
        public T Aggregate { get; private set; }

        public TaskAggregator(Func<TaskDependency, T> createAggregate) {
            CreateAggregate = createAggregate;
        }

        public void Before(TaskDependency dep) {
            Stack.Push(CreateAggregate(dep));
        }

        public void After(TaskDependency dep) {
            T agg = Stack.Pop();

            agg.Finally();

            if (Stack.Count > 0) {
                var taskAggregate = Stack.Peek();
                taskAggregate.Add(agg);
            } else {
                Aggregate = agg;
            }
        }
    }

    public interface ITaskAggregate<T> {
        void Add(T agg);
        void Finally();
    }

    public class TaskPurity : ITaskAggregate<TaskPurity> {
        public bool Purity { get; private set; }

        public TaskPurity(TaskDependency dep)
        {
            Purity = dep.Task.IsPure;
        }

        public void Add(TaskPurity purity) {
            Purity &= purity.Purity;
        }

        public void Finally () {
        }
    }
}