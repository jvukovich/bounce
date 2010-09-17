using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class TargetBuilder {
        public TargetWalker Walker;

        public TargetBuilder() : this(new TargetWalker()) {}

        public TargetBuilder(TargetWalker walker) {
            Walker = walker;
        }

        public void Build(ITarget target) {
            Walker.Walk(target, null, t => t.Build());
        }

        public void Clean(ITarget target) {
            Walker.Walk(target, b => b.Clean(), null);
        }
    }
}