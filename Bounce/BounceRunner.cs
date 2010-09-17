using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    public class BounceRunner {
        public BounceRunner () {
            Parameters = CommandLineParameters.ParametersWithUsualTypeParsers();
        }

        public CommandLineParameters Parameters { get; set; }

        public void Run(string[] args, object targets) {
            var builder = new TargetBuilder();

            try {
                if (args.Length >= 3) {
                    string[] buildArguments = Parameters.ParseCommandLineArguments(args);

                    string command = buildArguments[1];
                    Action<TargetBuilder, ITarget> commandAction = GetCommand(command);

                    for (int i = 2; i < buildArguments.Length; i++) {
                        string targetName = buildArguments[i];
                        ITarget target = FindTarget(targets, targetName);

                        if (target != null) {
                            commandAction(builder, target);
                        } else {
                            System.Console.WriteLine("no target named {0}", targetName);
                            System.Console.WriteLine("try one of the following:");
                            foreach (var name in GetTargetNames(targets)) {
                                System.Console.WriteLine("  " + name);
                            }
                        }
                    }
                } else {
                    System.Console.WriteLine("usage: bounce build|clean target-name");
                    System.Console.WriteLine();
                    System.Console.WriteLine("targets:");
                    foreach (var name in GetTargetNames(targets)) {
                        System.Console.WriteLine("  " + name);
                    }
                    System.Console.WriteLine();
                    System.Console.WriteLine("arguments:");
                    foreach (var param in Parameters.Parameters) {
                        System.Console.Write("  /" + param.Name);
                        if (param.Required) {
                            System.Console.Write(" required");
                        }
                        if (param.HasValue) {
                            System.Console.Write(" default: " + param.DefaultValue);
                        }
                        System.Console.WriteLine();
                    }
                }
            } catch (BuildException ce) {
                System.Console.WriteLine(ce.Message);
                System.Console.Write(ce.Output);
            } catch (ConfigurationException ce) {
                System.Console.WriteLine(ce.Message);
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