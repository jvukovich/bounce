using System;
using System.Collections.Generic;

namespace Bounce.Framework {
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
    }
}