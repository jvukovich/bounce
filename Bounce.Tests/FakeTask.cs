using System.Collections.Generic;
using Bounce.Framework;

namespace Bounce.Tests {
    public class FakeTask : ITask {
        public IEnumerable<ITask> Dependencies { get; set; }

        public virtual void BeforeBuild() {
        }

        public virtual void Build() {
        }

        public virtual void Clean() {
        }
    }
}