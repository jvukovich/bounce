using System.Collections.Generic;

namespace Bounce.Framework {
    class Parameter<T> : Val<T>, IParameter {
        public bool Required { get; set; }
        public string Name { get; set; }
        public bool HasValue { get; private set; }
        private T _value;
        public IEnumerable<object> AvailableValues { get; set; }

        public override IEnumerable<ITask> Dependencies
        {
            get { return new ITask[0]; }
        }

        public override T Value {
            get { return _value; }
        }

        public void SetValue(T val) {
            HasValue = true;
            _value = val;
        }

        public object DefaultValue {
            get { return _value; }
            set { SetValue((T) value); }
        }

        public void Parse(string parameterValue, ITypeParsers typeParsers) {
            SetValue(typeParsers.Parse<T>(parameterValue));
        }
    }
}