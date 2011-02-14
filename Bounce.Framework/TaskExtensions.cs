using System;

namespace Bounce.Framework {
    public static class TaskExtensions {
        public static Task<TP> WhenBuilt<TP>(this ITask task, Func<TP> getValue) {
            return new DependentFuture<TP>(task, getValue);
        }

        public static Task<TP> WhenBuilt<T, TP>(this Task<T> task, Func<T, TP> getValue) {
            return task.WhenBuilt(() => getValue(task.Value));
        }
    }
}