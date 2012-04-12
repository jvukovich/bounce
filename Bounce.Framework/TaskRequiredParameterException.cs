namespace Bounce.Framework {
    public class TaskRequiredParameterException : BounceException {
        public TaskRequiredParameterException(string name, ITask task) : base(string.Format("required parameter `{0}' not given for task {1}", name, task.FullName)) {
        }
    }
}