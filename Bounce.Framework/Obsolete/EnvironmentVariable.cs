using System;

namespace Bounce.Framework.Obsolete
{
    public class EnvironmentVariable<T> : TaskWithValue<T>
    {
        private string Name;
        private ITypeParsers TypeParsers;
        private T _defaultValue;
        private bool _hasDefaultValue;

        public EnvironmentVariable(string name) : this(name, Obsolete.TypeParsers.Default)
        {
        }

        public EnvironmentVariable(string name, ITypeParsers typeParsers)
        {
            Name = name;
            TypeParsers = typeParsers;
        }

        public T DefaultValue
        {
            get
            {
                if (_hasDefaultValue)
                {
                    return _defaultValue;
                } else
                {
                    throw new NoValueForParameterException(Name);
                }
            }
            set
            {
                _hasDefaultValue = true;
                _defaultValue = value;
            }
        }

        protected override T GetValue()
        {
            var value = Environment.GetEnvironmentVariable(Name);
            if (value != null)
            {
                return TypeParsers.Parse<T>(value);
            } else
            {
                return DefaultValue;
            }
        }
    }
}