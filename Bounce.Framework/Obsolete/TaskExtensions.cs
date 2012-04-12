using System;
using System.IO;

namespace Bounce.Framework.Obsolete {
    public static class TaskExtensions {
        public static Task<TP> WhenBuilt<TP>(this IObsoleteTask task, Func<TP> getValue) {
            return new DependentFuture<TP>(task, getValue);
        }

        public static Task<TP> WhenBuilt<T, TP>(this Task<T> task, Func<T, TP> getValue) {
            return task.WhenBuilt(() => getValue(task.Value));
        }

        public static Task<T> WithDependencyOn<T>(this Task<T> task, params IObsoleteTask [] dependencies) {
            return new DependentTask<T>(new All(dependencies), task);
        }

        public static IObsoleteTask WithDependencyOn(this IObsoleteTask task, params IObsoleteTask [] dependencies) {
            return new DependentTask(new All(dependencies), task);
        }

        public static Task<string> SubPath(this Task<string> path, Task<string> subPath) {
            return new All(path, subPath).WhenBuilt(() => Path.Combine(path.Value, subPath.Value));
        }

        public static Switch<T> Switch<T>(this Task<T> condition) {
            return new Switch<T>(condition);
        }

        public static Switch<T> Switch<T>(this Task<T> condition, T case1, IObsoleteTask task1) {
            var s = condition.Switch();
            s[case1] = task1;
            return s;
        }

        public static Switch<T> Switch<T>(this Task<T> condition, T case1, IObsoleteTask task1, T case2, IObsoleteTask task2) {
            var s = condition.Switch(case1, task1);
            s[case2] = task2;
            return s;
        }

        public static Switch<T> Switch<T>(this Task<T> condition, T case1, IObsoleteTask task1, T case2, IObsoleteTask task2, T case3, IObsoleteTask task3) {
            var s = condition.Switch(case1, task1, case2, task2);
            s[case3] = task3;
            return s;
        }

        public static Task<T> LogDebug<T>(this Task<T> value, Task<string> message) {
            return new LogValue<T>(true) {Message = message, Value = value};
        }

        public static Task<T> LogInfo<T>(this Task<T> value, Task<string> message) {
            return new LogValue<T>(false) {Message = message, Value = value};
        }
    }
}