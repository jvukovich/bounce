using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    public class TaskInvoker {
        private readonly Parameters _commandLineParameters;

        public TaskInvoker(Parameters commandLineParameters) {
            _commandLineParameters = commandLineParameters;
        }

        public void InvokeTask(Type taskType, string taskName) {
            var taskObject = taskType.GetConstructor(new Type[0]).Invoke(new object[0]);
            var method = taskType.GetMethod(taskName);

            var parameters = method.GetParameters();
            var arguments = ArgumentsFromCommandLineParameters(parameters, taskType, taskName);

            method.Invoke(taskObject, arguments);
        }

        private object[] ArgumentsFromCommandLineParameters(IEnumerable<ParameterInfo> parameters, Type taskType, string taskName) {
            return parameters.Select(p => (object) ParseParameter(p, taskType, taskName)).ToArray();
        }

        private object ParseParameter(ParameterInfo p, Type taskType, string taskName) {
            try {
                return _commandLineParameters.Parameter(p.ParameterType, p.Name);
            } catch (TypeParserNotFoundException e) {
                throw new TaskParameterException(p.ParameterType, p.Name, taskType, taskName);
            }
        }
    }

    public class TaskParameterException : Exception {
        public TaskParameterException(Type parameterType, string name, Type taskType, string taskName) : base(string.Format("no parser for parameter `{0}' of type `{1}' for task {2}.{3}", name, parameterType, taskType, taskName)) {
        }
    }
}