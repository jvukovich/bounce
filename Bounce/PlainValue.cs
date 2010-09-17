using System.Collections.Generic;

namespace Bounce.Framework {
    public class PlainValue<T> : IValue<T> {
        public PlainValue (T value) {
            Value = value;
        }

        public IEnumerable<ITarget> Dependencies {
            get { return new ITarget[0]; }
        }

        public void Build() {
        }

        public void Clean() {
        }

        public T Value { get; private set; }
    }
}