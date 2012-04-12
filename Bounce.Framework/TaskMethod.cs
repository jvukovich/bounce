using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bounce.Framework {
    public class TaskMethod : ITask {
        private readonly MethodInfo _method;

        public TaskMethod(MethodInfo method) {
            _method = method;
        }

        public string Name {
            get { return _method.Name; }
        }

        public string FullName {
            get { return _method.DeclaringType.FullName + "." + _method.Name; }
        }

        public void Invoke(Arguments arguments) {
            var taskObject = _method.DeclaringType.GetConstructor(new Type[0]).Invoke(new object[0]);
            var methodArguments = MethodArgumentsFromCommandLineParameters(arguments);
            _method.Invoke(taskObject, methodArguments);
        }

        private object[] MethodArgumentsFromCommandLineParameters(Arguments arguments)
        {
            return Parameters.Select(p => (object)ParseParameter(arguments, p)).ToArray();
        }

        private object ParseParameter(Arguments arguments, ITaskParameter p)
        {
            try {
                return arguments.Parameter(p);
            } catch(RequiredParameterNotGivenException e) {
                throw new TaskRequiredParameterException(p, this);
            } catch (TypeParserNotFoundException e) {
                throw new TaskParameterException(p, this);
            }
        }

        public IEnumerable<string> AllNames {
            get {
                var fullName = FullName;

                yield return fullName;
                int index = fullName.IndexOf(".");
                while (index > 0)
                {
                    fullName = fullName.Substring(index + 1);
                    yield return fullName;
                    index = fullName.IndexOf(".");
                }
            }
        }

        public IEnumerable<ITaskParameter> Parameters {
            get { return _method.GetParameters().Select(p => (ITaskParameter) new TaskMethodParameter(p)); }
        }
    }
}