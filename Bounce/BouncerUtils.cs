using System;
using System.Collections.Generic;

namespace Bounce {
    public static class BouncerUtils {
        public static IValue<TP> Property<TB, TP>(this TB bouncer, Func<TB, TP> getProperty) where TB : ITarget {
            return new Property<TB, TP>(bouncer, getProperty);
        }
    }

    public class Property<TB, TP> : IValue<TP> where TB : ITarget {
        private readonly TB Dependency;
        private readonly Func<TB, TP> GetProperty;

        public Property(TB dependency, Func<TB, TP> getProperty) {
            Dependency = dependency;
            GetProperty = getProperty;
        }

        public TP Value {
            get { return GetProperty(Dependency); }
        }

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {(ITarget) Dependency}; }
        }

        public DateTime? LastBuilt {
            get { return Dependency.LastBuilt; }
        }

        public void Build() {
        }

        public void Clean() {
        }
    }
}