using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    public class BounceRunner {
        private static Bounce _bounce = new Bounce(System.Console.Out, System.Console.Error);

        public static IBounce Bounce {
            get { return _bounce; }
        }

        public void Run(string[] args, MethodInfo getTargetsMethod) {
            CommandLineParameters parameters = CommandLineParameters.ParametersWithUsualTypeParsers();

            var builder = new TargetBuilder(_bounce);

            try {
                var targets = getTargetsMethod.Invoke(null, new[] {parameters});

                var parsedParameters = ParseCommandLineArguments(args);
                InterpretParameters(parameters, parsedParameters, _bounce);

                string[] buildArguments = parsedParameters.RemainingArguments;

                if (buildArguments.Length >= 2) {
                    string command = buildArguments[0];
                    Action<TargetBuilder, ITask> commandAction = GetCommand(command);

                    for (int i = 1; i < buildArguments.Length; i++) {
                        string targetName = buildArguments[i];
                        ITask task = FindTarget(targets, targetName);

                        if (task != null) {
                            commandAction(builder, task);
                        } else {
                            throw new NoSuchTargetException(targetName, GetTargetNames(targets));
                        }
                    }
                } else {
                    System.Console.WriteLine("usage: bounce build|clean target-name");
                    PrintAvailableTargets(targets);
                    PrintAvailableParameters(parameters);
                }
            } catch (BounceException ce) {
                ce.Explain(System.Console.Error);
                Environment.Exit(1);
            }
        }

        private void InterpretParameters(CommandLineParameters parameters, ParsedCommandLineParameters parsedParameters, Bounce bounce) {
            var loglevel = parsedParameters.Parameters.FirstOrDefault(p => p.Name == "loglevel");
            if (loglevel != null) {
                bounce.LogOptions.LogLevel = ParseLogLevel(loglevel.Value);
            }

            var commandOutput = parsedParameters.Parameters.FirstOrDefault(p => p.Name == "command-output");
            if (commandOutput != null) {
                bounce.LogOptions.CommandOutput = commandOutput.Value.ToLower() == "true";
            }

            parameters.ParseCommandLineArguments(parsedParameters.Parameters);
        }

        private LogLevel ParseLogLevel(string loglevel) {
            try {
                return (LogLevel) Enum.Parse(typeof (LogLevel), loglevel, true);
            } catch (Exception e) {
                throw new ConfigurationException(String.Format("log level {0} not recognised, try one of {1}", loglevel, String.Join(", ", Enum.GetNames(typeof(LogLevel)))));
            }
        }

        public ParsedCommandLineParameters ParseCommandLineArguments(string [] args) {
            var parser = new CommandLineParameterParser();
            return parser.ParseCommandLineParameters(args);
        }

        private void PrintAvailableTargets(object targets) {
            System.Console.WriteLine();
            System.Console.WriteLine("targets:");
            foreach (var name in GetTargetNames(targets)) {
                System.Console.WriteLine("  " + name);
            }
        }

        private void PrintAvailableParameters(CommandLineParameters parameters) {
            if (parameters.Parameters.Count() > 0) {
                System.Console.WriteLine();
                System.Console.WriteLine("arguments:");
                foreach (var param in parameters.Parameters) {
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
                        throw new ConfigurationException(String.Format("no such command {0}, try build or clean", command));
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