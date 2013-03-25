using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bounce.Framework {
    public class TaskMethod : ITask {
        private readonly MethodInfo Method;
        private readonly IDependencyResolver Resolver;

        public TaskMethod(MethodInfo method, IDependencyResolver resolver) {
            Method = method;
            Resolver = resolver;
        }

        public string Name {
            get { return Method.Name; }
        }

        public string FullName {
            get { return Method.DeclaringType.FullName + "." + Method.Name; }
        }

        public void Invoke(TaskParameters taskParameters) {
            try {
                var taskObject = Resolver.Resolve(Method.DeclaringType);
                var methodArguments = MethodArgumentsFromCommandLineParameters(taskParameters);
                Method.Invoke(taskObject, methodArguments);
            } catch (TargetInvocationException e) {
                throw new TaskException(this, e.InnerException);
            }
        }

        private object[] MethodArgumentsFromCommandLineParameters(TaskParameters taskParameters)
        {
            return Parameters.Select(p => (object)ParseParameter(taskParameters, p)).ToArray();
        }

        private object ParseParameter(TaskParameters taskParameters, ITaskParameter p)
        {
            try {
                return taskParameters.Parameter(p);
            } catch(RequiredParameterNotGivenException e) {
                throw new TaskRequiredParameterException(p, this);
            } catch (TypeParserNotFoundException e) {
                throw new TaskParameterException(p, this);
            }
        }

        public IEnumerable<ITaskParameter> Parameters {
            get { return Method.GetParameters().Select(p => (ITaskParameter) new TaskMethodParameter(p)); }
        }
    }
}