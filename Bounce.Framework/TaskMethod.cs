using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework
{
    public interface ITask
    {
        string FullName { get; }
        IEnumerable<ITaskParameter> Parameters { get; }
        void Invoke(TaskParameters taskParameters);
    }

    public class TaskMethod : ITask
    {
        private readonly MethodInfo _method;
        private readonly IDependencyResolver _resolver;

        public TaskMethod(MethodInfo method, IDependencyResolver resolver)
        {
            _method = method;
            _resolver = resolver;
        }

        public string FullName => _method.DeclaringType.FullName + "." + _method.Name;

        public void Invoke(TaskParameters taskParameters)
        {
            try
            {
                var taskObject = _resolver.Resolve(_method.DeclaringType);
                var methodArguments = MethodArgumentsFromCommandLineParameters(taskParameters);

                _method.Invoke(taskObject, methodArguments);
            }
            catch (TargetInvocationException e)
            {
                // todo: dotnetupgrade
                // todo: make exceptions generic
                //throw new TaskException(this, e.InnerException);
                throw e;
            }
        }

        private object[] MethodArgumentsFromCommandLineParameters(TaskParameters taskParameters)
        {
            return Parameters.Select(x => ParseParameter(taskParameters, x)).ToArray();
        }

        private static object ParseParameter(TaskParameters taskParameters, ITaskParameter p)
        {
            return taskParameters.Parameter(p);
        }

        public IEnumerable<ITaskParameter> Parameters
        {
            get { return _method.GetParameters().Select(x => (ITaskParameter) new TaskMethodParameter(x)); }
        }
    }
}