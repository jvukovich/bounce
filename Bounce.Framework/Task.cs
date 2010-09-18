using System.Collections.Generic;

namespace Bounce.Framework {
    public class Task : ITask {
        public virtual IEnumerable<ITask> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this);
            }
        }

        public virtual void BeforeBuild() {
        }

        public virtual void Build() {
        }

        public virtual void Clean() {
        }
    }
}