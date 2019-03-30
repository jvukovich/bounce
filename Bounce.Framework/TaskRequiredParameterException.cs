namespace Bounce.Framework
{
    public class TaskRequiredParameterException : BounceException
    {
        private readonly ITaskParameter _parameter;
        private readonly ITask _task;

        public TaskRequiredParameterException(ITaskParameter parameter, ITask task)
        {
            _parameter = parameter;
            _task = task;
        }

        public override void Explain(System.IO.TextWriter writer)
        {
            writer.WriteLine(string.Format("required parameter `{0}' not given for task {1}", _parameter.Name, _task.FullName));
        }
    }
}