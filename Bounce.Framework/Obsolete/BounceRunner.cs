using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bounce.Framework.Obsolete {
    public class BounceRunner {
        private readonly ITargetsRetriever TargetsRetriever;
        private ILogOptionCommandLineTranslator LogOptionCommandLineTranslator;
        private readonly IParameterFinder ParameterFinder;
        private ITargetsBuilder TargetsBuilder;
        private CommandAndTargetParser CommandAndTargetParser;

        public static string TargetsPath { get; private set; }

        public BounceRunner() : this(new TargetsRetriever(), new LogOptionCommandLineTranslator(), new ParameterFinder(), new TargetsBuilder()) {}

        public BounceRunner (ITargetsRetriever targetsRetriever, ILogOptionCommandLineTranslator logOptionCommandLineTranslator, IParameterFinder parameterFinder, ITargetsBuilder targetsBuilder) {
            TargetsRetriever = targetsRetriever;
            LogOptionCommandLineTranslator = logOptionCommandLineTranslator;
            ParameterFinder = parameterFinder;
            TargetsBuilder = targetsBuilder;
            CommandAndTargetParser = new CommandAndTargetParser(new BounceCommandParser());
        }

        public void Run(string[] args, MethodInfo getTargetsMethod) {
            TargetsPath = new Uri(getTargetsMethod.Module.Assembly.CodeBase).LocalPath;

            var parameters = new CommandLineParameters();

            try {
                IDictionary<string, ITask> targets = GetTargetsFromAssembly(getTargetsMethod, parameters);

                ParsedCommandLineParameters parsedParameters = ParseCommandLineArguments(args);

                string[] buildArguments = parsedParameters.RemainingArguments;

                CommandAndTargets commandAndTargets = CommandAndTargetParser.ParseCommandAndTargetNames(buildArguments, targets);

                if (commandAndTargets.Targets.Count() >= 1) {
                    var bounce = new Bounce();

                    InterpretParameters(parameters, parsedParameters, bounce);

                    try
                    {
                        EnsureAllRequiredParametersAreSet(parameters, commandAndTargets.Targets);

                        BuildTargets(bounce, commandAndTargets);
                    }
                    catch (BounceException ce)
                    {
                        ce.Explain(bounce.LogOptions.StdErr);
                        Environment.Exit(1);
                    }
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

        private void BuildTargets(Bounce bounce, CommandAndTargets commandAndTargets) {
            TargetsBuilder.BuildTargets(bounce, commandAndTargets.Targets, commandAndTargets.Command);
            bounce.CleanAfterBuild(commandAndTargets.Command);
        }

        private IDictionary<string, ITask> GetTargetsFromAssembly(MethodInfo getTargetsMethod, CommandLineParameters parameters) {
            return TargetsRetriever.GetTargetsFromAssembly(getTargetsMethod, parameters);
        }

        private void InterpretParameters(ICommandLineParameters parameters, ParsedCommandLineParameters parsedParameters, Bounce bounce) {
            LogOptionCommandLineTranslator.ParseCommandLine(parsedParameters, bounce);
            bounce.ParametersGiven = parameters.ParseCommandLineArguments(parsedParameters.Parameters);
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

        private static void PrintAvailableParameters(IEnumerable<IParameter> parameters) {
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
    }

    internal class CommandAndTargets {
        public IBounceCommand Command;
        public IEnumerable<Target> Targets;
    }
}