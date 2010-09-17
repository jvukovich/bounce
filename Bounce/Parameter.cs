using System.Collections.Generic;

namespace Bounce.Framework {
    class Parameter<T> : IParameter<T> {
        public bool Required { get; set; }
        public string Name { get; set; }
        public bool HasValue { get; private set; }
        private T _value;
        public IEnumerable<object> AvailableValues { get; set; }

        public IEnumerable<ITarget> Dependencies
        {
            get { return new ITarget[0]; }
        }

        public void BeforeBuild() {
        }

        public void Build() {
        }

        public void Clean() {
        }

        public T Value {
            get { return _value; }
            set {
                HasValue = true;
                _value = value;
            }
        }

        public object DefaultValue {
            get { return Value; }
        }

        public void Parse(string parameterValue, ITypeParsers typeParsers) {
            Value = typeParsers.Parse<T>(parameterValue);
        }
    }
}