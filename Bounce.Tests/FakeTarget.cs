using System.Collections.Generic;
using Bounce.Framework;

namespace Bounce.Tests {
    public class FakeTarget : ITarget {
        public IEnumerable<ITarget> Dependencies { get; set; }

        public virtual void Build() {
        }

        public virtual void Clean() {
        }
    }
}