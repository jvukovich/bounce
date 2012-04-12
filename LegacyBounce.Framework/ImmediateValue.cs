namespace LegacyBounce.Framework {
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