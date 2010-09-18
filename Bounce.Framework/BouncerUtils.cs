using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public static class BouncerUtils {
        public static Val<TP> Property<TB, TP>(this TB bouncer, Func<TB, TP> getProperty) where TB : ITask {
            return new Property<TB, TP>(bouncer, getProperty);
        }

        public static Val<TP> WhenBuilt<TP>(this ITask bouncer, Func<TP> getValue) {
            return new FutureValue<TP>(bouncer, getValue);
        }
    }

    public class Property<TB, TP> : Val<TP> where TB : ITask {
        private readonly TB Dependency;
        private readonly Func<TB, TP> GetProperty;

        public Property(TB dependency, Func<TB, TP> getProperty) {
            Dependency = dependency;
            GetProperty = getProperty;
        }

        public override TP Value {
            get { return GetProperty(Dependency); }
        }

        public override IEnumerable<ITask> Dependencies {
            get { return new[] { (ITask)Dependency }; }
        }

        public override void BeforeBuild() {
        }

        public override void Build() {
        }

        public override void Clean() {
        }
    }
 
    public class FutureValue<TP> : Val<TP> {
        private readonly ITask dependency;
        private readonly Func<TP> getValue;

        public FutureValue(ITask dependency, Func<TP> getValue) {
            this.dependency = dependency;
            this.getValue = getValue;
        }

        public override TP Value {
            get { return getValue(); }
        }

        public override IEnumerable<ITask> Dependencies {
            get { return new[] {dependency}; }
        }

        public override void BeforeBuild() {
        }

        public override void Build() {
        }

        public override void Clean() {
        }
    }
}