using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Bounce.Console
{
    [Serializable]
    class BounceAssemblyRunner
    {
        private readonly BeforeBounceScriptRunner BeforeBounceScriptRunner;
        private string bounceDirectory;
        private string workingDirectory;
        private string[] arguments;

        public BounceAssemblyRunner()
        {
            BeforeBounceScriptRunner = new BeforeBounceScriptRunner();
        }

        public int Run(string[] args)
        {
            try
            {
                if (args != null && args.Length > 0)
                {
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

                        var returnCode = FindTargetsAssemblyAndRunBounce(newArgs.ToArray());

                        if (returnCode != 0)
                            return 1;
                    }

                    return 0;
                }

                return FindTargetsAssemblyAndRunBounce(args);
            } catch (BounceConsoleException bce)
            {
                bce.Explain(System.Console.Error);
                return 1;
            } catch (Exception e)
            {
                System.Console.Error.WriteLine(e);
                return 1;
            }
        }

        private int FindTargetsAssemblyAndRunBounce(string[] args)
        {
            var optionsAndArguments = GetAssemblyFileName(args);

            BeforeBounceScriptRunner.RunBeforeBounceScript(optionsAndArguments);

            bounceDirectory = Path.GetFullPath(optionsAndArguments.BounceDirectory);
            workingDirectory = optionsAndArguments.WorkingDirectory;
            arguments = optionsAndArguments.RemainingArguments;

            var bounceAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(bounceDirectory, "Bounce.Framework.dll"));
            var bounceRunnerType = bounceAssembly.GetType("Bounce.Framework.BounceRunner");
            var bounceRunnerInstance = Activator.CreateInstance(bounceRunnerType);

            if (bounceRunnerInstance == null)
                throw new NullReferenceException("bounceRunnerInstance is null");

            // todo: dotnetcore
            // RunTask() temporarily replaced to test the framework changes

            try
            {
                bounceRunnerType.GetMethod("Run").Invoke(bounceRunnerInstance, new object[] {bounceDirectory, workingDirectory, arguments});
                return 0;
            } finally
            {
                // todo: dotnetcore
            }
        }

        private void RunTask()
        {
            var bounceAssembly = ReferencedBounceAssembly();

            RunBounce(bounceAssembly);
        }

        private void RunBounce(Assembly bounceAssembly)
        {
            Type runnerType = bounceAssembly.GetType("Bounce.Framework.BounceRunner");
            object runner = runnerType.GetConstructor(new Type[0]).Invoke(new object[0]);
            try
            {
                var exitCode = (int) runnerType.GetMethod("Run").Invoke(runner, new object[] {bounceDirectory, workingDirectory, arguments});
                if (exitCode != 0)
                {
                    throw new BadExitException();
                }
            } catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
            }
        }

        private Assembly ReferencedBounceAssembly()
        {
            var bounceAssembly = FindBounceAssembly();
            if (bounceAssembly != null)
            {
                return bounceAssembly;
            }

            bounceAssembly = FindBounceAssemblyReferencedFromBounceDirectory();
            if (bounceAssembly != null)
            {
                return bounceAssembly;
            }

            throw new TasksNotFoundException();
        }

        private Assembly FindBounceAssemblyReferencedFromBounceDirectory()
        {
            foreach (var file in Directory.GetFiles(bounceDirectory))
            {
                if (IsExecutable(file))
                {
                    var assembly = Assembly.LoadFrom(file);
                    var allMethods =
                        assembly.GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance));

                    foreach (var method in allMethods)
                    {
                        foreach (var attr in method.GetCustomAttributes(false))
                        {
                            var attrType = attr.GetType();
                            if (attrType.FullName == "Bounce.Framework.TaskAttribute")
                            {
                                return attrType.Assembly;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private Assembly FindBounceAssembly()
        {
            var path = Path.Combine(bounceDirectory, "Bounce.Framework.dll");
            if (File.Exists(path))
            {
                return Assembly.LoadFrom(path);
            } else
            {
                return null;
            }
        }

        private static bool IsExecutable(string file)
        {
            return file.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase)
                   || file.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase);
        }

        private OptionsAndArguments GetAssemblyFileName(string[] args)
        {
            return new TargetsAssemblyArgumentsParser().GetTargetsAssembly(args);
        }
    }
}