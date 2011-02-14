using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class DependentFuture<TP> : TaskWithValue<TP> {
        [Dependency]
        private readonly ITask DependencyTask;
        private readonly Func<TP> getValue;

        public DependentFuture(ITask dependencyTask, Func<TP> getValue) {
            this.DependencyTask = dependencyTask;
            this.getValue = getValue;
        }

        protected override TP GetValue() {
            return getValue();
        }
    }
}