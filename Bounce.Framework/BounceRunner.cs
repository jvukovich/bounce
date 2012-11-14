using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bounce.Framework {
    public class BounceRunner {
        public int Run(string bounceDirectory, string [] rawArguments) {
            try {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(bounceDirectory));
                var arguments = ParsedArguments(rawArguments);

                Bounce.SetUp(arguments);

                var tasks = Tasks(bounceDirectory);

                if (rawArguments.Length > 0) {
                    RunTask(TaskName(rawArguments), arguments, tasks);
                } else {
                    UsageHelp.WriteUsage(Console.Out, tasks);
                }
                return 0;
            } catch (BounceException e) {
                e.Explain(Console.Error);
                return 1;
            } catch (ReflectionTypeLoadException e) {
                foreach (var loaderException in e.LoaderExceptions) {
                    Console.Error.WriteLine(loaderException);
                }
                return 1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }
        }

        private static string TaskName(string[] arguments) {
            return arguments[0];
        }

        private static Arguments ParsedArguments(string[] arguments) {
            var parsedArguments = new ArgumentsParser().ParseParameters(arguments.Skip(1));
            var parameters = new Arguments(parsedArguments);
            return parameters;
        }

        private void RunTask(string taskName, Arguments arguments, IEnumerable<ITask> tasks) {
            var matchingTasks = tasks.Where(t => t.AllNames.Contains(taskName));

            if (matchingTasks.Count() > 1) {
                throw new AmbiguousTaskNameException(taskName, matchingTasks.OfType<ITask>());
            }
            else if (!matchingTasks.Any())
            {
                throw new NoMatchingTaskException(taskName, tasks);
            } else {
                matchingTasks.Single().Invoke(arguments);
            }
        }

        private IEnumerable<ITask> Tasks(string bounceDirectory) {
            return Directory.GetFiles(bounceDirectory).Where(IsBounceExecutable).SelectMany(file => {
                var assembly = Assembly.LoadFrom(file);
                var allMethods =
                    assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance));
                return allMethods
                    .Where(method => method.GetCustomAttributes(typeof (TaskAttribute), false).Length > 0)
                    .Select(method => (ITask) new TaskMethod(method));
            });
        }

        private bool IsBounceExecutable(string path) {
            string fileName = Path.GetFileName(path);
            return !fileName.Equals("Bounce.Framework.dll", StringComparison.InvariantCultureIgnoreCase)
                && new Regex(@"\bbounce\b.*\.(dll|exe)", RegexOptions.IgnoreCase).IsMatch(fileName);
        }
    }
}