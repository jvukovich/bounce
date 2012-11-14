using System.Reflection;

namespace Bounce.Framework {
    public class TaskParameterException : BounceException {
        public TaskParameterException(ITaskParameter parameter, ITask task) : base(string.Format("no parser for parameter `{0}' of type `{1}' for task {2}", parameter.Name, parameter.TypeDescription, task.FullName)) {
        }
    }
}