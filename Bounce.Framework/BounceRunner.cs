using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    public class BounceRunner {
        public void Run(string[] args, MethodInfo getTargetsMethod) {
            CommandLineParameters parameters = CommandLineParameters.ParametersWithUsualTypeParsers();

            var builder = new TargetBuilder();

            try {
                var targets = getTargetsMethod.Invoke(null, new[] {parameters});

                if (args.Length >= 2) {
                    string[] buildArguments = parameters.ParseCommandLineArguments(args);

                    string command = buildArguments[0];
                    Action<TargetBuilder, ITask> commandAction = GetCommand(command);

                    for (int i = 1; i < buildArguments.Length; i++) {
                        string targetName = buildArguments[i];
                        ITask task = FindTarget(targets, targetName);

                        if (task != null) {
                            commandAction(builder, task);
                        } else {
                            Console.WriteLine("no target named {0}", targetName);
                            Console.WriteLine("try one of the following:");
                            foreach (var name in GetTargetNames(targets)) {
                                Console.WriteLine("  " + name);
                            }
                        }
                    }
                } else {
                    Console.WriteLine("usage: bounce build|clean target-name");
                    Console.WriteLine();
                    Console.WriteLine("targets:");
                    foreach (var name in GetTargetNames(targets)) {
                        Console.WriteLine("  " + name);
                    }
                    Console.WriteLine();
                    Console.WriteLine("arguments:");
                    foreach (var param in parameters.Parameters) {
                        Console.Write("  /" + param.Name);
                        if (param.Required) {
                            Console.Write(" required");
                        }
                        if (param.HasValue) {
                            Console.Write(" default: " + param.DefaultValue);
                        }
                        Console.WriteLine();
                    }
                }
            } catch (BounceException ce) {
                ce.Explain(Console.Out);
            }
        }

        private static Action<TargetBuilder, ITask> GetCommand(string command) {
            switch (command.ToLower()) {
                case "build": {
                        return (builder, task) => builder.Build(task);
                    }
                case "clean": {
                        return (builder, task) => builder.Clean(task);
                    }
                default: {
                        throw new ConfigurationException(String.Format("command {0} not recognised", command));
                    }
            }
        }

        private static ITask FindTarget(object targets, string targetName) {
            PropertyInfo propertyInfo = targets.GetType().GetProperty(targetName);

            if (propertyInfo != null) {
                return (ITask)propertyInfo.GetValue(targets, new object[0]);
            } else {
                return null;
            }
        }

        private static IEnumerable<string> GetTargetNames(object targets) {
            return targets.GetType().GetProperties().Select(p => p.Name);
        }
    }
}