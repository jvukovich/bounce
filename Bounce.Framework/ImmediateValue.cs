using System.Collections.Generic;

namespace Bounce.Framework {
    public class ImmediateValue<T> : TaskWithValue<T> {
        private T _value;

        public ImmediateValue (T value) {
            _value = value;
        }

        protected override T GetValue() {
            return _value;
        }
    }
}