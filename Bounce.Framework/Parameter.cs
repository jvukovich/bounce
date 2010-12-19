using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    internal class Parameter<T> : TaskWithValue<T>, IParameter<T> {
        public bool Required { get; set; }

        public string Name { get; set; }
        public bool HasValue { get; private set; }
        private T _value;
        public IEnumerable<object> AvailableValues { get; set; }

        public Parameter() {
        }

        public Parameter(string name, T value) {
            Name = name;
            SetValue(value);
        }

        public override IEnumerable<ITask> Dependencies
        {
            get { return new ITask[0]; }
        }

        public override T GetValue() {
            if (!HasValue) {
                throw new NoValueForParameterException(Name);
            }
            return _value;
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
            var valueAsString = typeParsers.Generate(Value);
            var paramterAsString = String.Format("/{0}:{1}", Name, valueAsString);

            if (valueAsString.Contains(" ")) {
                return "\"" + paramterAsString + "\"";
            } else {
                return paramterAsString;
            }
        }

        public IParameter<T> NewWithValue(T value) {
            return new Parameter<T>(Name, value);
        }
    }
}