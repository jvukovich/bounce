using System;

namespace Bounce.Framework {
    public static class TaskExtensions {
        public static Future<TP> WhenBuilt<TP>(this ITask task, Func<TP> getValue) {
            return new DependentFuture<TP>(task, getValue);
        }
    }
}