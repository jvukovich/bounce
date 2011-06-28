using System;
namespace Bounce.Framework {
    public interface ITargetsBuilder {
        void BuildTargets(ITargetBuilderBounce bounce, System.Collections.Generic.IEnumerable<Target> targets, IBounceCommand command);
    }
}
