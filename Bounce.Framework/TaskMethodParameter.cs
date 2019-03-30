using System;
using System.Reflection;

namespace Bounce.Framework
{
    internal class TaskMethodParameter : ITaskParameter
    {
        private readonly ParameterInfo _parameter;

        public TaskMethodParameter(ParameterInfo parameter)
        {
            _parameter = parameter;
        }

        public string Name => _parameter.Name;

        public Type Type => IsNullable ? _parameter.ParameterType.GetGenericArguments()[0] : _parameter.ParameterType;

        public string TypeDescription => TypeParsers.Default.TypeParser(Type).Description;

        public bool IsRequired => !IsNullable && _parameter.RawDefaultValue == DBNull.Value;

        private bool IsNullable =>
            _parameter.ParameterType.IsGenericType
            && typeof(Nullable<>).IsAssignableFrom(_parameter.ParameterType.GetGenericTypeDefinition());

        public object DefaultValue =>
            IsNullable
                ? _parameter.DefaultValue == DBNull.Value
                    ? null
                    : _parameter.DefaultValue
                : _parameter.DefaultValue;
    }
}