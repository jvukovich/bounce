using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    public class BounceRunner {
        private static Bounce _bounce = new Bounce(System.Console.Out, System.Console.Error, new TaskLogFactory());

        public static IBounce Bounce {
            get { return _bounce; }
        }

        public void Run(string[] args, MethodInfo getTargetsMethod) {
            CommandLineParameters parameters = CommandLineParameters.ParametersWithUsualTypeParsers();

            try {
                object targets = GetTargetsFromAssembly(getTargetsMethod, parameters);

                var parsedParameters = ParseCommandLineArguments(args);
                InterpretParameters(parameters, parsedParameters, _bounce);

                string[] buildArguments = parsedParameters.RemainingArguments;

                if (buildArguments.Length >= 2) {
                    string command = buildArguments[0];
                    IEnumerable<string> targetsToBuild = TargetsToBuild(buildArguments);

                    BuildTargets(command, targetsToBuild, targets);
                } else {
                    System.Console.WriteLine("usage: bounce build|clean target-name");
                    PrintAvailableTargets(targets);
                    PrintAvailableParameters(parameters);
                    Environment.Exit(1);
                }
            } catch (BounceException ce) {
                ce.Explain(System.Console.Error);
                Environment.Exit(1);
            }
        }

        private void BuildTargets(string command, IEnumerable<string> targetsToBuild, object targets) {
            var builder = new TargetBuilder(_bounce);
            CommandAction commandAction = GetCommand(builder, command);

            foreach(var targetName in targetsToBuild) {
                BuildTarget(targets, targetName, commandAction);
            }
        }

        private void BuildTarget(object targets, string targetName, CommandAction commandAction) {
            ITask task = FindTarget(targets, targetName);

            if (task != null) {
                using (var targetScope = _bounce.TaskScope(task, commandAction.Command, targetName)) {
                    commandAction.Action(task);
                    targetScope.TaskSucceeded();
                }
            } else {
                throw new NoSuchTargetException(targetName, GetTargetNames(targets));
            }
        }

        private IEnumerable<string> TargetsToBuild(string [] args) {
            string [] targets = new string[args.Length - 1];
            Array.Copy(args, 1, targets, 0, targets.Length);
            return targets;
        }

        private object GetTargetsFromAssembly(MethodInfo getTargetsMethod, CommandLineParameters parameters) {
            ParameterInfo[] methodParameters = getTargetsMethod.GetParameters();
            if (methodParameters.Length == 1) {
                if (methodParameters[0].ParameterType.IsAssignableFrom(typeof(IParameters)))
                {
                    return getTargetsMethod.Invoke(null, new[] { parameters });
                }
            }

            if (methodParameters.Length == 0) {
                return getTargetsMethod.Invoke(null, new object[0]);
            }

            throw new TargetsMethodWrongSignatureException(getTargetsMethod.Name);
        }

        private void InterpretParameters(CommandLineParameters parameters, ParsedCommandLineParameters parsedParameters, Bounce bounce) {
            var loglevel = parsedParameters.TryPopParameter("loglevel");
            if (loglevel != null) {
                bounce.LogOptions.LogLevel = ParseLogLevel(loglevel.Value);
            }

            var commandOutput = parsedParameters.TryPopParameter("command-output");
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

        private class CommandAction {
            public BounceCommand Command;
            public Action<ITask> Action;
        }

        private static CommandAction GetCommand(TargetBuilder builder, string command) {
            switch (command.ToLower()) {
                case "build":
                    return new CommandAction {Action = builder.Build, Command = BounceCommand.Build};
                case "clean":
                    return new CommandAction { Action = builder.Clean, Command = BounceCommand.Clean };
                default:
                    throw new ConfigurationException(String.Format("no such command {0}, try build or clean", command));
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