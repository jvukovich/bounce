using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Serilog;

namespace Bounce.Framework
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class BounceRunner
    {
        public const string BounceFrameworkAssemblyFileName = "Bounce.Framework.dll";

        // ReSharper disable once UnusedMember.Global
        public void Run(string bounceDir, string workingDir, string[] rawArgs)
        {
            GetLogger();

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
                foreach (var loaderException in e.LoaderExceptions)
                    Log.Error(loaderException.ToString());
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public static ILogger GetLogger()
        {
            if (_loggerConfigured)
                return Log.Logger;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();

            _loggerConfigured = true;

            return Log.Logger;
        }

        private static bool _loggerConfigured;

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

            return fileName.ToLowerInvariant() != BounceFrameworkAssemblyFileName &&
                !fileName.ToLowerInvariant().EndsWith(".config") &&
                new Regex(@"\bbounce\b.*\.(dll|exe)", RegexOptions.IgnoreCase).IsMatch(fileName);
        }
    }
}