using System;

namespace LegacyBounce.Framework {
    public class DependentFuture<T> : TaskWithValue<T> {
        [Dependency]
        private readonly IObsoleteTask DependencyTask;
        private readonly Func<T> getValue;

        public DependentFuture(IObsoleteTask dependencyTask, Func<T> getValue) {
            this.DependencyTask = dependencyTask;
            this.getValue = getValue;
        }

        protected override T GetValue() {
            return getValue();
        }
    }
}