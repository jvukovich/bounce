using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public static class BouncerUtils {
        public static IValue<TP> Property<TB, TP>(this TB bouncer, Func<TB, TP> getProperty) where TB : ITarget {
            return new Property<TB, TP>(bouncer, getProperty);
        }

        public static IValue<TP> Future<TP>(this ITarget bouncer, Func<TP> getValue) {
            return new FutureValue<TP>(bouncer, getValue);
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
            get { return new[] { (ITarget)Dependency }; }
        }

        public void Build() {
        }

        public void Clean() {
        }
    }
 
    public class FutureValue<TP> : IValue<TP> {
        private readonly ITarget dependency;
        private readonly Func<TP> getValue;

        public FutureValue(ITarget dependency, Func<TP> getValue) {
            this.dependency = dependency;
            this.getValue = getValue;
        }

        public TP Value {
            get { return getValue(); }
        }

        public IEnumerable<ITarget> Dependencies {
            get { return new[] {dependency}; }
        }

        public void Build() {
        }

        public void Clean() {
        }
    }
}