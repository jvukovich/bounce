using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework
{
    public interface ITaskRunner
    {
        void RunTask(string taskName, TaskParameters taskParameters, IEnumerable<ITask> tasks);
    }

    public class TaskRunner : ITaskRunner
    {
        public void RunTask(string taskName, TaskParameters taskParameters, IEnumerable<ITask> tasks)
        {
            var taskList = tasks.ToList();

            var matchingTasks = taskList.Where(x => AllTaskNames(x).Contains(taskName.ToLower())).ToList();

            if (matchingTasks.Count > 1)
                throw new Exception($"Multiple tasks with the name '{taskName}'. Qualify the task with a class name or namespace.");

            if (!matchingTasks.Any())
            {
                var availableTasks = UsageHelp.GetAvailableTasks(taskList);
                throw new Exception($"Task '{taskName}' not found. Try one of the following: {availableTasks}");
            }

            matchingTasks.Single().Invoke(taskParameters);
        }

        private static IEnumerable<string> AllTaskNames(ITask task)
        {
            var fullName = task.FullName.ToLower();

            yield return fullName;

            var index = fullName.IndexOf(".");

            while (index > 0)
            {
                fullName = fullName.Substring(index + 1);

                yield return fullName;

                index = fullName.IndexOf(".");
            }
        }
    }
}