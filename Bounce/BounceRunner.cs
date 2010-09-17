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

                if (args.Length >= 3) {
                    string[] buildArguments = parameters.ParseCommandLineArguments(args);

                    string command = buildArguments[1];
                    Action<TargetBuilder, ITarget> commandAction = GetCommand(command);

                    for (int i = 2; i < buildArguments.Length; i++) {
                        string targetName = buildArguments[i];
                        ITarget target = FindTarget(targets, targetName);

                        if (target != null) {
                            commandAction(builder, target);
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
            } catch (BuildException ce) {
                Console.WriteLine(ce.Message);
                Console.Write(ce.Output);
            } catch (ConfigurationException ce) {
                Console.WriteLine(ce.Message);
            }
        }

        private static Action<TargetBuilder, ITarget> GetCommand(string command) {
            switch (command.ToLower()) {
                case "build": {
                        return (builder, target) => builder.Build(target);
                    }
                case "clean": {
                        return (builder, target) => builder.Clean(target);
                    }
                default: {
                        throw new ConfigurationException(String.Format("command {0} not recognised", command));
                    }
            }
        }

        private static ITarget FindTarget(object targets, string targetName) {
            PropertyInfo propertyInfo = targets.GetType().GetProperty(targetName);

            if (propertyInfo != null) {
                return (ITarget)propertyInfo.GetValue(targets, new object[0]);
            } else {
                return null;
            }
        }

        private static IEnumerable<string> GetTargetNames(object targets) {
            return targets.GetType().GetProperties().Select(p => p.Name);
        }
    }
}