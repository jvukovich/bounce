using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public abstract class TaskWithValue<T> : Future<T> {
        public override T Value {
            get {
                if (Invoked) {
                    return GetValue();
                } else {
                    throw new DependencyBuildFailureException(this, "no value for future");
                }
            }
        }

        private bool Invoked = true;

        public abstract T GetValue();

        public override void Invoke(IBounceCommand command, IBounce bounce) {
            InvokeFuture(command, bounce);
            Invoked = true;
        }

        public virtual void InvokeFuture(IBounceCommand command, IBounce bounce) {}

        public override IEnumerable<TaskDependency> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this).Concat(RegisterAdditionalDependencies());
            }
        }

        protected virtual IEnumerable<TaskDependency> RegisterAdditionalDependencies() {
            return new TaskDependency[0];
        }
    }
}