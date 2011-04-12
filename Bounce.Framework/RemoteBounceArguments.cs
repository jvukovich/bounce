using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class RemoteBounceArguments : TaskWithValue<string> {
        public IEnumerable<string> Targets { get; set; }
        public IEnumerable<IParameter> Parameters { get; set; }

        private ITargetsParser TargetsParser;
        private ILogOptionCommandLineTranslator LogOptionCommandLineTranslator;
        private readonly ICommandLineTasksParametersGenerator CommandLineTasksParametersGenerator;
        private string GeneratedBounceArguments;

        public RemoteBounceArguments(ITargetsParser targetsParser, ILogOptionCommandLineTranslator logOptionCommandLineTranslator, ICommandLineTasksParametersGenerator commandLineTasksParametersGenerator) {
            TargetsParser = targetsParser;
            LogOptionCommandLineTranslator = logOptionCommandLineTranslator;
            CommandLineTasksParametersGenerator = commandLineTasksParametersGenerator;
            Parameters = new IParameter[0];
        }

        public RemoteBounceArguments() : this(new TargetsParser(), new LogOptionCommandLineTranslator(), new CommandLineTasksParametersGenerator()) {}

        protected override string GetValue()
        {
            return GeneratedBounceArguments;
        }

        public override void InvokeFuture(IBounceCommand command, IBounce bounce) {
            GeneratedBounceArguments = GetBounceArguments(bounce, command);
        }

        private string GetBounceArguments(IBounce bounce, IBounceCommand command) {
            var args = new List<string>();

            args.Add(LogOptionCommandLineTranslator.GenerateCommandLine(bounce));

            args.Add(command.CommandLineCommand);
            args.AddRange(Targets);
            args.Add(CommandLineTasksParametersGenerator.GenerateCommandLineParametersForTasks(bounce.ParametersGiven, Parameters));

            return String.Join(" ", args.ToArray());
        }

        public RemoteBounceArguments WithParameter(IParameter parameter) {
            IEnumerable<IParameter> newParameters = Parameters.Concat(new [] {parameter});
            return new RemoteBounceArguments {
                Targets = Targets,
                Parameters = newParameters
            };
        }
    }
}