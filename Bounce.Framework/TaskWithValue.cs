using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public abstract class TaskWithValue<T> : Future<T> {
        public override IEnumerable<ITask> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this).Concat(RegisterAdditionalDependencies());
            }
        }

        protected virtual IEnumerable<ITask> RegisterAdditionalDependencies() {
            return new ITask[0];
        }
    }
}