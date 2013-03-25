using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bounce.Framework {
    public class BounceRunner {
        private readonly ITaskRunner TaskRunner;

        public BounceRunner() : this(new TaskRunner()) {
        }

        public BounceRunner(ITaskRunner taskRunner) {
            TaskRunner = taskRunner;
        }

        public int Run(string bounceDirectory, string [] rawArguments) {
            try {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(bounceDirectory));
                var parameters = ParsedArguments(rawArguments);
                Parameters.Main = new Parameters(parameters);

                var dependencyResolver = new AttributedDependencyResolvers();
                var tasks = Tasks(bounceDirectory, dependencyResolver);

                if (rawArguments.Length > 0) {
                    TaskRunner.RunTask(TaskName(rawArguments), parameters, tasks);
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

        private static TaskParameters ParsedArguments(string[] arguments) {
            return new TaskParameters(new ArgumentsParser().ParseParameters(arguments.Skip(1)));
        }

        private IEnumerable<ITask> Tasks(string bounceDirectory, AttributedDependencyResolvers dependencyResolvers) {
            IEnumerable<Type> allTypes = Directory.GetFiles(bounceDirectory).Where(IsBounceExecutable).SelectMany(file => {
                var assembly = Assembly.LoadFrom(file);
                return assembly.GetTypes();
            });

            IEnumerable<MethodInfo> resolverMethods = allTypes.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static));
            IEnumerable<MethodInfo> resolvers = resolverMethods.Where(method => method.GetCustomAttributes(typeof(DependencyResolverAttribute), false).Length > 0);

            foreach (var resolver in resolvers) {
                dependencyResolvers.AddDependencyResolver(resolver);
            }

            IEnumerable<MethodInfo> taskMethods = allTypes.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance));
            IEnumerable<ITask> tasks = taskMethods.Where(method => method.GetCustomAttributes(typeof(TaskAttribute), false).Length > 0).Select(method => (ITask)new TaskMethod(method, dependencyResolvers));
            return tasks;
        }

        private bool IsBounceExecutable(string path) {
            string fileName = Path.GetFileName(path);
            return !fileName.Equals("Bounce.Framework.dll", StringComparison.InvariantCultureIgnoreCase)
                && new Regex(@"\bbounce\b.*\.(dll|exe)", RegexOptions.IgnoreCase).IsMatch(fileName);
        }
    }
}