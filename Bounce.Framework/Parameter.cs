using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    internal class Parameter<T> : Future<T>, IParameter {
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
            get {
                if (!HasValue) {
                    throw new NoValueForParameterException(Name);
                }
                return _value;
            }
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

        public string Generate(ITypeParsers typeParsers) {
            return String.Format("/{0}:{1}", Name, typeParsers.Generate(Value));
        }
    }
}