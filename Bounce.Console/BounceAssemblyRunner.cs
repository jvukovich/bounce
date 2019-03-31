using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Bounce.Framework;

namespace Bounce.Console
{
    [Serializable]
    internal class BounceAssemblyRunner
    {
        public static void Run(string[] args)
        {
            if (args == null || args.Length <= 0)
            {
                FindTargetsAssemblyAndRunBounce(args);
                return;
            }

            var tasks = new HashSet<string>();
            var taskArgs = new HashSet<string>();

            var foundTaskArgsSwitch = false;

            foreach (var arg in args)
            {
                if (arg.StartsWith("/"))
                    foundTaskArgsSwitch = true;

                if (foundTaskArgsSwitch)
                    taskArgs.Add(arg);
                else
                    tasks.Add(arg);
            }

            foreach (var task in tasks)
            {
                var newArgs = new HashSet<string> {task};

                foreach (var taskArg in taskArgs)
                    newArgs.Add(taskArg);

                FindTargetsAssemblyAndRunBounce(newArgs.ToArray());
            }
        }

        private static void FindTargetsAssemblyAndRunBounce(string[] args)
        {
            var optionsAndArguments = GetAssemblyFileName(args);

            var bounceDir = Path.GetFullPath(optionsAndArguments.BounceDirectory);
            var workingDir = optionsAndArguments.WorkingDirectory;
            var remainingArgs = optionsAndArguments.RemainingArguments;

            var bounceAssemblyPath = Path.Combine(bounceDir, BounceRunner.BounceFrameworkAssemblyFileName);

            if (!File.Exists(bounceAssemblyPath))
                bounceAssemblyPath = Path.Combine(workingDir, BounceRunner.BounceFrameworkAssemblyFileName);

            if (!File.Exists(bounceAssemblyPath))
            {
                BounceRunner.GetLogger().Error($"Unable to find Bounce Framework assembly. Failed attempt to load: {bounceAssemblyPath}");
                return;
            }

            var bounceAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(bounceAssemblyPath);
            var bounceRunnerType = bounceAssembly.GetType("Bounce.Framework.BounceRunner");
            var bounceRunnerInstance = Activator.CreateInstance(bounceRunnerType);

            if (bounceRunnerInstance == null)
                throw new NullReferenceException("bounceRunnerInstance is null");

            try
            {
                var parameters = new object[] {bounceDir, workingDir, remainingArgs};
                bounceRunnerType.GetMethod("Run").Invoke(bounceRunnerInstance, parameters);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
            }
        }

        private static OptionsAndArguments GetAssemblyFileName(string[] args)
        {
            return TargetsAssemblyArgumentsParser.GetTargetsAssembly(args);
        }
    }
}