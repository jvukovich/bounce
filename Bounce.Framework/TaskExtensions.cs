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

        public static Task<string> SubPath(this Task<string> path, Task<string> subPath) {
            return new All(path, subPath).WhenBuilt(() => Path.Combine(path.Value, subPath.Value));
        }
    }
}