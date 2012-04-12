using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bounce.Framework.Obsolete;

namespace Bounce.Framework {
    public class BounceRunner {
        public void Run(string bounceDirectory, string [] arguments) {
            try {
                var tasks = Tasks(bounceDirectory);
                if (arguments.Length > 0) {
                    RunTask(tasks, arguments);
                } else {
                    PrintUsage(tasks);
                }
            } catch (BounceException e) {
                e.Explain(Console.Error);
            } catch (Exception e) {
                Console.Error.WriteLine(e);
            }
        }

        private static void PrintUsage(IEnumerable<ITask> tasks) {
            foreach (var task in tasks) {

                Console.WriteLine("    " + task.FullName);

                foreach (var parameter in task.Parameters) {
                    Console.WriteLine("        /{0}:{1}", parameter.Name, parameter.Type);
                }
            }
        }

        private void RunTask(IEnumerable<ITask> tasks, string[] arguments) {
            string taskName = arguments[0];
            var parsedArguments = new ParameterParser().ParseParameters(arguments.Skip(1));
            var parameters = new Parameters(parsedArguments);

            var matchingTasks = tasks.Where(t => t.AllNames.Contains(taskName));

            if (matchingTasks.Count() > 1) {
                throw new AmbiguousTaskNameException(taskName, matchingTasks.OfType<IObsoleteTask>());
            }
            else if (!matchingTasks.Any())
            {
                throw new NoTasksException();
            } else {
                matchingTasks.Single().Invoke(parameters);
            }
        }

        private IEnumerable<ITask> Tasks(string bounceDirectory) {
            return Directory.GetFiles(bounceDirectory).Where(IsExecutable).SelectMany(file => {
                var assembly = Assembly.LoadFrom(file);
                var allMethods =
                    assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance));
                return allMethods
                    .Where(method => method.GetCustomAttributes(typeof (TaskAttribute), false).Length > 0)
                    .Select(method => (ITask) new TaskMethod(method));
            });
        }

        private bool IsExecutable(string s) {
            return s.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)
                   || s.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}