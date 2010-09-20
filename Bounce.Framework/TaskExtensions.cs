using System;

namespace Bounce.Framework {
    public static class TaskExtensions {
        public static Val<TP> Property<TB, TP>(this TB task, Func<TB, TP> getProperty) where TB : ITask {
            return new Property<TB, TP>(task, getProperty);
        }

        public static Val<TP> WhenBuilt<TP>(this ITask task, Func<TP> getValue) {
            return new FutureValue<TP>(task, getValue);
        }
    }
}