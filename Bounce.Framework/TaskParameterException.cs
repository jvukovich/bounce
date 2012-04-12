using System;
using System.Reflection;

namespace Bounce.Framework {
    public class TaskParameterException : BounceException {
        public TaskParameterException(Type parameterType, string name, ITask task) : base(string.Format("no parser for parameter `{0}' of type `{1}' for task {2}", name, parameterType, task.FullName)) {
        }
    }
}