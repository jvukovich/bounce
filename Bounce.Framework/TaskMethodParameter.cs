using System;
using System.Reflection;

namespace Bounce.Framework {
    class TaskMethodParameter : ITaskParameter {
        private readonly ParameterInfo _parameter;

        public TaskMethodParameter(ParameterInfo parameter) {
            _parameter = parameter;
        }

        public string Name {
            get { return _parameter.Name; }
        }

        public Type Type {
            get {
                if (IsNullable) {
                    return _parameter.ParameterType.GetGenericArguments()[0];
                } else {
                    return _parameter.ParameterType;
                }
            }
        }

        public string TypeDescription {
            get {
                return TypeParsers.Default.TypeParser(Type).Description;
            }
        }

        public bool IsRequired {
            get {
                return !IsNullable && _parameter.RawDefaultValue == DBNull.Value;
            }
        }

        private bool IsNullable {
            get {
                return _parameter.ParameterType.IsGenericType
                       && typeof (Nullable<>).IsAssignableFrom(_parameter.ParameterType.GetGenericTypeDefinition());
            }
        }

        public object DefaultValue {
            get {
                if (IsNullable) {
                    return _parameter.DefaultValue == DBNull.Value ? null : _parameter.DefaultValue;
                } else {
                    return _parameter.DefaultValue;
                }
            }
        }
    }
}