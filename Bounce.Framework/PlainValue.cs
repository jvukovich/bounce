using System.Collections.Generic;

namespace Bounce.Framework {
    public class PlainValue<T> : TaskWithValue<T> {
        private T _value;

        public PlainValue (T value) {
            _value = value;
        }

        public override IEnumerable<ITask> Dependencies {
            get { return new ITask[0]; }
        }

        public override T GetValue() {
            return _value;
        }

        public void SetValue(T val) {
            _value = val;
        }
    }
}