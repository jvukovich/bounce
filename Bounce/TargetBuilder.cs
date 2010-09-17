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
            Walker.Walk(target, null, BuildIfOutOfDate);
        }

        private static void BuildIfOutOfDate(ITarget t) {
            if (IsBouncerOutOfDate(t)) {
                t.Build();
            }
        }

        private static bool IsBouncerOutOfDate(ITarget t) {
            var deps = t.GetNonNullDependencies();

            if (deps.Count() == 0) {
                return !t.LastBuilt.HasValue;
            }

            if (deps.All(d => d.LastBuilt.HasValue)) {
                IEnumerable<DateTime> allDates = deps.Select(d => d.LastBuilt.Value);
                DateTime latestOfAllDeps = allDates.Max();
                if (t.LastBuilt.HasValue && latestOfAllDeps <= t.LastBuilt.Value) {
                    return false;
                }
            }

            return true;
        }

        public void Clean(ITarget target) {
            Walker.Walk(target, b => b.Clean(), null);
        }
    }
}