using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class Parameters {
        private readonly IDictionary<string, string> _parameters;

        public Parameters(IDictionary<string, string> parameters) {
            _parameters = parameters;
        }

        public object Parameter(Type type, string name, object defaultValue) {
            if (_parameters.ContainsKey(name)) {
                return TypeParsers.Default.Parse(type, _parameters[name]);
            } else {
                return defaultValue;
            }
        }

        public T Parameter<T>(string name, T defaultValue) {
            return (T) Parameter(typeof (T), name, defaultValue);
        }

        public object Parameter(Type type, string name) {
            if (_parameters.ContainsKey(name)) {
                return TypeParsers.Default.Parse(type, _parameters[name]);
            } else {
                throw new RequiredParameterNotGivenException(name);
            }
        }

        public T Parameter<T>(string name) {
            return (T) Parameter(typeof(T), name);
        }
    }
}