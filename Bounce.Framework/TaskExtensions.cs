using System;

namespace Bounce.Framework {
    public static class TaskExtensions {
        public static Val<TP> WhenBuilt<TP>(this ITask task, Func<TP> getValue) {
            return new FutureValue<TP>(task, getValue);
        }
    }
}