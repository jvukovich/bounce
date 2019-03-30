using System;

namespace Bounce.Framework
{
    public class TaskParameter : ITaskParameter
    {
        public TaskParameter()
        {
            IsRequired = true;
        }

        private object _defaultValue;

        public string Name { get; set; }

        public string TypeDescription
        {
            get { return TypeParsers.Default.TypeParser(Type).Description; }
        }

        public bool IsRequired { get; private set; }

        public object DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                _defaultValue = value;
                IsRequired = false;
            }
        }

        public Type Type { get; set; }
    }
}