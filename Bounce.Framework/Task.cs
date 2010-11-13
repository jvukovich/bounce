using System.Collections.Generic;

namespace Bounce.Framework {
    public abstract class Task : ITask {
        public virtual IEnumerable<ITask> Dependencies {
            get {
                return TaskDependencyFinder.Instance.GetDependenciesFor(this);
            }
        }

        public virtual void Build(IBounce bounce) {
            Build();
        }

        public virtual void Build() {}

        public virtual void Clean(IBounce bounce) {
            Clean();
        }

        public virtual void Clean() {}

        public bool IsLogged { get { return true; } }
    }
}