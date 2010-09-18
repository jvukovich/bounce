using System.Collections.Generic;

namespace Bounce.Framework {
    public class PlainValue<T> : Val<T> {
        private T _value;

        public PlainValue (T value) {
            _value = value;
        }

        public override IEnumerable<ITask> Dependencies {
            get { return new ITask[0]; }
        }

        public override void BeforeBuild() {
        }

        public override void Build() {
        }

        public override void Clean() {
        }

        public override T Value {
            get { return _value; }
        }

        public void SetValue(T val) {
            _value = val;
        }
    }
}