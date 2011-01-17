using System;

namespace Bounce.Framework {
    public static class TaskExtensions {
        public static Future<TP> WhenBuilt<TP>(this ITask task, Func<TP> getValue) {
            return new DependentFuture<TP>(task, getValue);
        }

        public static Future<TP> WhenBuilt<T, TP>(this Future<T> task, Func<T, TP> getValue) {
            return task.WhenBuilt(() => getValue(task.Value));
        }
    }
}