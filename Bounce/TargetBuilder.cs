using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class TargetBuilder {
        public TargetWalker Walker;
        public HashSet<ITarget> BuiltTargets;

        public TargetBuilder() : this(new TargetWalker()) {
            BuiltTargets = new HashSet<ITarget>();
        }

        public TargetBuilder(TargetWalker walker) {
            Walker = walker;
        }

        public void Build(ITarget target) {
            Walker.Walk(target, t => t.BeforeBuild(), BuildIfNotAlreadyBuilt);
        }

        public void BuildIfNotAlreadyBuilt(ITarget target) {
            if (!BuiltTargets.Contains(target)) {
                target.Build();
                BuiltTargets.Add(target);
            }
        }

        public void Clean(ITarget target) {
            Walker.Walk(target, b => b.Clean(), null);
        }
    }
}