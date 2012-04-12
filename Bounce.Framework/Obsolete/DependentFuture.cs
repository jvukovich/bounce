using System;

namespace Bounce.Framework.Obsolete {
    public class DependentFuture<T> : TaskWithValue<T> {
        [Dependency]
        private readonly ITask DependencyTask;
        private readonly Func<T> getValue;

        public DependentFuture(ITask dependencyTask, Func<T> getValue) {
            this.DependencyTask = dependencyTask;
            this.getValue = getValue;
        }

        protected override T GetValue() {
            return getValue();
        }
    }
}