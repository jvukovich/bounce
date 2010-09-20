using System.Collections.Generic;
using Bounce.Framework;

namespace Bounce.Tests {
    public class FakeTask : ITask {
        public IEnumerable<ITask> Dependencies { get; set; }

        public virtual void BeforeBuild(IBounce bounce) {
        }

        public virtual void Build(IBounce bounce) {
        }

        public virtual void Clean(IBounce bounce) {
        }
    }
}