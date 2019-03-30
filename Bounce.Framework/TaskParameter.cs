using System;

namespace Bounce.Framework
{
    public interface ITaskParameter
    {
        string Name { get; }
        string TypeDescription { get; }
        bool IsRequired { get; }
        object DefaultValue { get; }
        Type Type { get; }
    }

    public class TaskParameter : ITaskParameter
    {
        private object _defaultValue;

        public TaskParameter()
        {
            IsRequired = true;
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public string TypeDescription => TypeParsers.Default.TypeParser(Type).Description;

        public bool IsRequired { get; private set; }

        public object DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                IsRequired = false;
            }
        }
    }
}