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

        public string Type {
            get {
                return TypeParsers.Default.TypeParser(_parameter.ParameterType).Description;
            }
        }
    }
}