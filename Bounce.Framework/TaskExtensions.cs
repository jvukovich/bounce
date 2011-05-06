using System;
using System.IO;

namespace Bounce.Framework {
    public static class TaskExtensions {
        public static Task<TP> WhenBuilt<TP>(this ITask task, Func<TP> getValue) {
            return new DependentFuture<TP>(task, getValue);
        }

        public static Task<TP> WhenBuilt<T, TP>(this Task<T> task, Func<T, TP> getValue) {
            return task.WhenBuilt(() => getValue(task.Value));
        }

        public static Task<T> WithDependencyOn<T>(this Task<T> task, params ITask [] dependencies) {
            return new DependentTask<T>(new All(dependencies), task);
        }

        public static ITask WithDependencyOn(this ITask task, params ITask [] dependencies) {
            return new DependentTask(new All(dependencies), task);
        }

        public static Task<string> SubPath(this Task<string> path, Task<string> subPath) {
            return new All(path, subPath).WhenBuilt(() => Path.Combine(path.Value, subPath.Value));
        }

        public static Switch<T> Switch<T>(this Task<T> condition) {
            return new Switch<T>(condition);
        }

        public static Switch<T> Switch<T>(this Task<T> condition, T case1, ITask task1) {
            var s = condition.Switch();
            s[case1] = task1;
            return s;
        }

        public static Switch<T> Switch<T>(this Task<T> condition, T case1, ITask task1, T case2, ITask task2) {
            var s = condition.Switch(case1, task1);
            s[case2] = task2;
            return s;
        }

        public static Switch<T> Switch<T>(this Task<T> condition, T case1, ITask task1, T case2, ITask task2, T case3, ITask task3) {
            var s = condition.Switch(case1, task1, case2, task2);
            s[case3] = task3;
            return s;
        }
    }
}