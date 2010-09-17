using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class TargetWalker {
        public void Walk(ITarget target, Action<ITarget> beforeDependencies, Action<ITarget> afterDependencies) {
            if (beforeDependencies != null) {
                beforeDependencies(target);
            }

            foreach (ITarget bouncerDependency in target.GetNonNullDependencies()) {
                Walk(bouncerDependency, beforeDependencies, afterDependencies);
            }

            if (afterDependencies != null) {
                afterDependencies(target);
            }
        }
    }

    public static class TargetExtensions
    {
        public static IEnumerable<ITarget> GetNonNullDependencies(this ITarget target) {
            var deps = target.Dependencies;
            if (deps == null) {
                return new ITarget[0];
            } else {
                return deps.Where(d => d != null);
            }
        }
    }
}