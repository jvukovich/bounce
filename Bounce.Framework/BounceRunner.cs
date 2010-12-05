using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework {
    public class BounceRunner {
        private static Bounce _bounce = new Bounce(System.Console.Out, System.Console.Error);
        private readonly ITargetsRetriever TargetsRetriever;
        private ILogOptionCommandLineTranslator LogOptionCommandLineTranslator;
        private readonly IParameterFinder ParameterFinder;

        public static IBounce Bounce {
            get { return _bounce; }
        }

        public BounceRunner() : this(new TargetsRetriever(), new LogOptionCommandLineTranslator(), new ParameterFinder()) {}

        public BounceRunner (ITargetsRetriever targetsRetriever, ILogOptionCommandLineTranslator logOptionCommandLineTranslator, IParameterFinder parameterFinder) {
            TargetsRetriever = targetsRetriever;
            LogOptionCommandLineTranslator = logOptionCommandLineTranslator;
            ParameterFinder = parameterFinder;
        }

        public void Run(string[] args, MethodInfo getTargetsMethod) {
            var parameters = new CommandLineParameters();

            try {
                IDictionary<string, ITask> targets = GetTargetsFromAssembly(getTargetsMethod, parameters);

                ParsedCommandLineParameters parsedParameters = ParseCommandLineArguments(args);

                string[] buildArguments = parsedParameters.RemainingArguments;

                if (buildArguments.Length >= 2) {
                    InterpretParameters(parameters, parsedParameters, _bounce);

                    string command = buildArguments[0];
                    IEnumerable<Target> targetsToBuild = TargetsToBuild(buildArguments, targets);

                    EnsureAllRequiredParametersAreSet(parameters, targetsToBuild);

                    BuildTargets(command, targetsToBuild);
                }
                else
                {
                    System.Console.WriteLine("usage: bounce build|clean target-name");
                    PrintAvailableTargets(targets);
                    Environment.Exit(1);
                }
            } catch (BounceException ce) {
                ce.Explain(System.Console.Error);
                Environment.Exit(1);
            }
        }

        private void EnsureAllRequiredParametersAreSet(CommandLineParameters parameters, IEnumerable<Target> targetsToBuild) {
            var tasks = targetsToBuild.Select(t => t.Task);
            foreach (ITask task in tasks) {
                IEnumerable<IParameter> parametersForTask = ParameterFinder.FindParametersInTask(task);
                parameters.EnsureAllRequiredParametersHaveValues(parametersForTask);
            }
        }

        private void BuildTargets(string command, IEnumerable<Target> targets) {
            var builder = new TargetBuilder(_bounce);
            CommandAction commandAction = GetCommand(builder, command);

            foreach(var target in targets) {
                BuildTarget(target, commandAction);
            }
        }

        private void BuildTarget(Target target, CommandAction commandAction) {
            using (ITaskScope targetScope = _bounce.TaskScope(target.Task, commandAction.Command, target.Name)) {
                commandAction.Action(target.Task);
                targetScope.TaskSucceeded();
            }
        }

        private Target FindTarget(IDictionary<string, ITask> targets, string targetName) {
            try {
                return new Target {Name = targetName, Task = targets[targetName]};
            } catch (KeyNotFoundException) {
                throw new NoSuchTargetException(targetName, targets.Keys);
            }
        }

        private IEnumerable<Target> TargetsToBuild(string [] args, IDictionary<string, ITask> targets) {
            return GetTargetNamesFromArguments(args).Select(name => FindTarget(targets, name));
        }

        private IEnumerable<string> GetTargetNamesFromArguments(string[] args) {
            string [] targets = new string[args.Length - 1];
            Array.Copy(args, 1, targets, 0, targets.Length);
            return targets;
        }

        private IDictionary<string, ITask> GetTargetsFromAssembly(MethodInfo getTargetsMethod, CommandLineParameters parameters) {
            return TargetsRetriever.GetTargetsFromAssembly(getTargetsMethod, parameters);
        }

        private void InterpretParameters(ICommandLineParameters parameters, ParsedCommandLineParameters parsedParameters, Bounce bounce) {
            LogOptionCommandLineTranslator.ParseCommandLine(parsedParameters, bounce);
            parameters.ParseCommandLineArguments(parsedParameters.Parameters);
        }

        public ParsedCommandLineParameters ParseCommandLineArguments(string [] args) {
            var parser = new CommandLineParameterParser();
            return parser.ParseCommandLineParameters(args);
        }

        private void PrintAvailableTargets(IDictionary<string, ITask> targets) {
            System.Console.WriteLine();
            System.Console.WriteLine("targets:");
            foreach (var target in targets) {
                System.Console.WriteLine("  " + target.Key);
                PrintAvailableParameters(ParameterFinder.FindParametersInTask(target.Value));
            }
        }

        private void PrintAvailableParameters(IEnumerable<IParameter> parameters) {
            if (parameters.Count() > 0) {
                foreach (var param in parameters) {
                    System.Console.Write("    /" + param.Name);
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

    internal class Target {
        public string Name;
        public ITask Task;
    }
}