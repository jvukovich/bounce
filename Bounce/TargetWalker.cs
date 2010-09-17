using System;

namespace Bounce.Framework {
    public class TargetWalker {
        public void Walk(ITarget target, Action<ITarget> beforeDependencies, Action<ITarget> afterDependencies) {
            if (beforeDependencies != null) {
                beforeDependencies(target);
            }

            foreach (ITarget bouncerDependency in target.Dependencies) {
                Walk(bouncerDependency, beforeDependencies, afterDependencies);
            }

            if (afterDependencies != null) {
                afterDependencies(target);
            }
        }
    }
}