using System.Collections.Generic;

namespace Bounce.Framework.Tests {
    public class FakeTask : ITask {
        public IEnumerable<ITask> Dependencies { get; set; }

        public virtual void Build(IBounce bounce) {
        }

        public virtual void Clean(IBounce bounce) {
        }

        public bool IsLogged { get { return true; } }
    }
}