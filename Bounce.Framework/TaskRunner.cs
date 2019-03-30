using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework
{
    public class TaskRunner : ITaskRunner
    {
        public void RunTask(string taskName, TaskParameters taskParameters, IEnumerable<ITask> tasks)
        {
            var matchingTasks = tasks.Where(t => AllTaskNames(t).Contains(taskName.ToLower())).ToArray();

            if (matchingTasks.Count() > 1)
            {
                throw new AmbiguousTaskNameException(taskName, matchingTasks);
            } else if (!matchingTasks.Any())
            {
                throw new NoMatchingTaskException(taskName, tasks);
            } else
            {
                matchingTasks.Single().Invoke(taskParameters);
            }
        }

        public IEnumerable<string> AllTaskNames(ITask task)
        {
            var fullName = task.FullName.ToLower();

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
}