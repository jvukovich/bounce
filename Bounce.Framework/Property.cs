using System;
using System.Collections.Generic;

namespace Bounce.Framework {
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
}