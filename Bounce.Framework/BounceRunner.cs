using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    public class BounceRunner {
        private static Bounce _bounce = new Bounce(System.Console.Out, System.Console.Error);
        private LogFactoryRegistry LogFactoryRegistry;

        public static IBounce Bounce {
            get { return _bounce; }
        }

        public BounceRunner () {
            LogFactoryRegistry = LogFactoryRegistry.Default;
        }

        public void Run(string[] args, MethodInfo getTargetsMethod) {
            var parameters = new CommandLineParameters();

            try {
                Dictionary<string, ITask> targets = GetTargetsFromAssembly(getTargetsMethod, parameters);

                var parsedParameters = ParseCommandLineArguments(args);

                string[] buildArguments = parsedParameters.RemainingArguments;

                if (buildArguments.Length >= 2) {
                    InterpretParameters(parameters, parsedParameters, _bounce);

                    string command = buildArguments[0];
                    IEnumerable<string> targetsToBuild = TargetsToBuild(buildArguments);

                    BuildTargets(command, targetsToBuild, targets);
                }
                else
                {
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

        private void BuildTargets(string command, IEnumerable<string> targetsToBuild, Dictionary<string, ITask> targets) {
            var builder = new TargetBuilder(_bounce);
            CommandAction commandAction = GetCommand(builder, command);

            foreach(var targetName in targetsToBuild) {
                BuildTarget(targets, targetName, commandAction);
            }
        }

        private void BuildTarget(Dictionary<string, ITask> targets, string targetName, CommandAction commandAction) {
            ITask task = FindTarget(targets, targetName);

            using (ITaskScope targetScope = _bounce.TaskScope(task, commandAction.Command, targetName)) {
                commandAction.Action(task);
                targetScope.TaskSucceeded();
            }
        }

        private ITask FindTarget(Dictionary<string, ITask> targets, string targetName) {
            ITask task;
            try {
                task = targets[targetName];
            } catch (KeyNotFoundException) {
                throw new NoSuchTargetException(targetName, targets.Keys);
            }
            return task;
        }

        private IEnumerable<string> TargetsToBuild(string [] args) {
            string [] targets = new string[args.Length - 1];
            Array.Copy(args, 1, targets, 0, targets.Length);
            return targets;
        }

        private Dictionary<string, ITask> GetTargetsFromAssembly(MethodInfo getTargetsMethod, CommandLineParameters parameters) {
            object targetsObject = GetTargetsObjectFromAssembly(getTargetsMethod, parameters);

            Dictionary<string, ITask> targets = new Dictionary<string, ITask>();

            foreach (PropertyInfo property in targetsObject.GetType().GetProperties()) {
                targets[property.Name] = (ITask) property.GetValue(targetsObject, new object[0]);
            }

            return targets;
        }

        private object GetTargetsObjectFromAssembly(MethodInfo getTargetsMethod, CommandLineParameters parameters) {
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

        private void InterpretParameters(ICommandLineParameters parameters, ParsedCommandLineParameters parsedParameters, Bounce bounce) {
            parsedParameters.IfParameterDo("loglevel", loglevel => bounce.LogOptions.LogLevel = ParseLogLevel(loglevel));
            parsedParameters.IfParameterDo("command-output", commandOutput => bounce.LogOptions.CommandOutput = ParseBoolOption(commandOutput));
            parsedParameters.IfParameterDo("logformat", logformat => bounce.LogFactory = GetLogFactoryByName(logformat));
            parsedParameters.IfParameterDo("describe-tasks", descTasks => bounce.LogOptions.DescribeTasks = ParseBoolOption(descTasks));

            parameters.ParseCommandLineArguments(parsedParameters.Parameters);
        }

        private bool ParseBoolOption(string option) {
            return option.ToLower() == "true";
        }

        private ITaskLogFactory GetLogFactoryByName(string name) {
            return LogFactoryRegistry.GetLogFactoryByName(name);
        }

        private LogLevel ParseLogLevel(string loglevel) {
            try {
                return (LogLevel) Enum.Parse(typeof (LogLevel), loglevel, true);
            } catch (Exception) {
                throw new ConfigurationException(String.Format("log level {0} not recognised, try one of {1}", loglevel, String.Join(", ", Enum.GetNames(typeof(LogLevel)))));
            }
        }

        public ParsedCommandLineParameters ParseCommandLineArguments(string [] args) {
            var parser = new CommandLineParameterParser();
            return parser.ParseCommandLineParameters(args);
        }

        private void PrintAvailableTargets(Dictionary<string, ITask> targets) {
            System.Console.WriteLine();
            System.Console.WriteLine("targets:");
            foreach (var name in targets.Keys) {
                System.Console.WriteLine("  " + name);
            }
        }

        private void PrintAvailableParameters(ICommandLineParameters parameters) {
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

        private static IEnumerable<string> GetTargetNames(object targets) {
            return targets.GetType().GetProperties().Select(p => p.Name);
        }
    }
}