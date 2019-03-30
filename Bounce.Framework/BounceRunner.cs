using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bounce.Framework
{
    // ReSharper disable once UnusedMember.Global
    public class BounceRunner
    {
        // ReSharper disable once UnusedMember.Global
        public void Run(string bounceDir, string workingDir, string[] rawArgs)
        {
            try
            {
                Directory.SetCurrentDirectory(workingDir);

                var parameters = ParsedArguments(rawArgs);

                Parameters.Main = new Parameters(parameters);

                var dependencyResolver = new AttributedDependencyResolvers();
                var tasks = Tasks(bounceDir, dependencyResolver);

                if (rawArgs.Length > 0)
                    new TaskRunner().RunTask(TaskName(rawArgs), parameters, tasks);
                else
                    UsageHelp.WriteUsage(tasks);
            }
            catch (ReflectionTypeLoadException e)
            {
                // todo: dotnetupgrade
                // todo: change logging strategy
                foreach (var loaderException in e.LoaderExceptions)
                    Console.Error.WriteLine(loaderException);
            }
            catch (Exception e)
            {
                // todo: dotnetupgrade
                // todo: change logging strategy
                Console.Error.WriteLine(e);
            }
        }

        private static string TaskName(IReadOnlyList<string> arguments)
        {
            return arguments[0];
        }

        private static TaskParameters ParsedArguments(IEnumerable<string> arguments)
        {
            return new TaskParameters(ArgumentsParser.ParseParameters(arguments.Skip(1)));
        }

        private static IEnumerable<ITask> Tasks(string bounceDirectory, AttributedDependencyResolvers dependencyResolvers)
        {
            var allTypes =
                Directory.GetFiles(bounceDirectory)
                    .Where(IsBounceExecutable)
                    .SelectMany(
                        file =>
                        {
                            var assembly = Assembly.LoadFrom(file);
                            return assembly.GetTypes();
                        })
                    .ToList();

            var resolverMethods = allTypes.SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Static));
            var resolvers = resolverMethods.Where(x => x.GetCustomAttributes(typeof(DependencyResolverAttribute), false).Length > 0);

            foreach (var resolver in resolvers)
                dependencyResolvers.AddDependencyResolver(resolver);

            var taskMethods = allTypes.SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.Instance));

            var tasks =
                taskMethods
                    .Where(x => x.GetCustomAttributes(typeof(TaskAttribute), false).Length > 0)
                    .Select(x => (ITask) new TaskMethod(x, dependencyResolvers));

            return tasks;
        }

        private static bool IsBounceExecutable(string path)
        {
            var fileName = Path.GetFileName(path);

            // todo: dotnetupgrade
            // todo: cleanup
            return !fileName.Equals("Bounce.Framework.dll", StringComparison.InvariantCultureIgnoreCase)
                   && new Regex(@"\bbounce\b.*\.(dll|exe)", RegexOptions.IgnoreCase).IsMatch(fileName);
        }
    }
}