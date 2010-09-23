using System.Collections.Generic;

namespace Bounce.Framework {
    public class NoSuchTargetException : BounceException {
        private readonly string Target;
        private readonly IEnumerable<string> AvailableTargets;

        public NoSuchTargetException(string target, IEnumerable<string> availableTargets) : base("no such target " + target) {
            Target = target;
            AvailableTargets = availableTargets;
        }

        public override void Explain(System.IO.TextWriter stderr)
        {
            stderr.WriteLine("no such target {0}", Target);
            stderr.WriteLine();
            stderr.WriteLine("try one of the following:");
            foreach (var name in AvailableTargets) {
                stderr.WriteLine("  " + name);
            }
        }
    }
}