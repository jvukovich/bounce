using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class Parameters {
        private readonly IDictionary<string, string> _parameters;

        public Parameters(IDictionary<string, string> parameters) {
            _parameters = parameters;
        }

        public object Parameter(Type type, string name, object defaultValue) {
            return Parameter(new TaskParameter {Name = name, Type = type, DefaultValue = defaultValue});
        }

        public T Parameter<T>(string name, T defaultValue) {
            return (T) Parameter(typeof (T), name, defaultValue);
        }

        public object Parameter(Type type, string name) {
            return Parameter(new TaskParameter {Name = name, Type = type});
        }

        public object Parameter(ITaskParameter parameter) {
            if (_parameters.ContainsKey(parameter.Name)) {
                return ParseParameter(parameter);
            } else {
                if (parameter.IsRequired) {
                    throw new RequiredParameterNotGivenException(parameter.Name);
                } else {
                    return parameter.DefaultValue;
                }
            }
        }

        private object ParseParameter(ITaskParameter parameter) {
            var parser = TypeParsers.Default.TypeParser(parameter.Type);
            if (parser != null) {
                return parser.Parse(_parameters[parameter.Name]);
            } else {
                throw new TypeParserNotFoundException(_parameters[parameter.Name], parameter.Type);
            }
        }

        public T Parameter<T>(string name) {
            return (T) Parameter(typeof(T), name);
        }
    }
}