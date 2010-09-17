using System.Collections.Generic;

namespace Bounce.Framework {
    public class PlainValue<T> : IValue<T> {
        public PlainValue (T value) {
            Value = value;
        }

        public IEnumerable<ITask> Dependencies {
            get { return new ITask[0]; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
        }

        public void Clean() {
        }

        public T Value { get; private set; }
    }
}