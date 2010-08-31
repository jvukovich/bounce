using System;

namespace Bounce {
    public class TargetWalker {
        public void Walk(ITarget target, Action<ITarget> stepOnBouncer) {
            foreach (ITarget bouncerDependency in target.Dependencies) {
                Walk(bouncerDependency, stepOnBouncer);
            }
            stepOnBouncer(target);
        }
    }
}