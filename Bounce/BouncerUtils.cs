using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public static class BouncerUtils {
        public static IValue<TP> Property<TB, TP>(this TB bouncer, Func<TB, TP> getProperty) where TB : ITask {
            return new Property<TB, TP>(bouncer, getProperty);
        }

        public static IValue<TP> Future<TP>(this ITask bouncer, Func<TP> getValue) {
            return new FutureValue<TP>(bouncer, getValue);
        }
    }

    public class Property<TB, TP> : IValue<TP> where TB : ITask {
        private readonly TB Dependency;
        private readonly Func<TB, TP> GetProperty;

        public Property(TB dependency, Func<TB, TP> getProperty) {
            Dependency = dependency;
            GetProperty = getProperty;
        }

        public TP Value {
            get { return GetProperty(Dependency); }
        }

        public IEnumerable<ITask> Dependencies {
            get { return new[] { (ITask)Dependency }; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
        }

        public void Clean() {
        }
    }
 
    public class FutureValue<TP> : IValue<TP> {
        private readonly ITask dependency;
        private readonly Func<TP> getValue;

        public FutureValue(ITask dependency, Func<TP> getValue) {
            this.dependency = dependency;
            this.getValue = getValue;
        }

        public TP Value {
            get { return getValue(); }
        }

        public IEnumerable<ITask> Dependencies {
            get { return new[] {dependency}; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
        }

        public void Clean() {
        }
    }
}