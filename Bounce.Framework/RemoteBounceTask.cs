using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public class RemoteBounceTask : Task {
        public IRemoteBounceExecutor RemoteBounceExecutor { get; set; }
        public object Targets { get; set; }

        private ITargetsParser TargetsParser;
        private ILogOptionCommandLineTranslator LogOptionCommandLineTranslator;
        private readonly ICommandLineTasksParametersGenerator CommandLineTasksParametersGenerator;

        public RemoteBounceTask(ITargetsParser targetsParser, ILogOptionCommandLineTranslator logOptionCommandLineTranslator, ICommandLineTasksParametersGenerator commandLineTasksParametersGenerator) {
            TargetsParser = targetsParser;
            LogOptionCommandLineTranslator = logOptionCommandLineTranslator;
            CommandLineTasksParametersGenerator = commandLineTasksParametersGenerator;
        }

        public RemoteBounceTask() : this(new TargetsParser(), new LogOptionCommandLineTranslator(), new CommandLineTasksParametersGenerator()) {}

        public override void Build(IBounce bounce) {
            ExecuteRemoteBounceWithCommand(bounce, "build");
        }

        public override void Clean(IBounce bounce) {
            ExecuteRemoteBounceWithCommand(bounce, "clean");
        }

        private void ExecuteRemoteBounceWithCommand(IBounce bounce, string command) {
            List<string> args = new List<string>();

            args.Add(LogOptionCommandLineTranslator.GenerateCommandLine(bounce));

            IDictionary<string, ITask> targetsFromObject = TargetsParser.ParseTargetsFromObject(Targets);
            args.Add(command);
            args.AddRange(targetsFromObject.Keys);
            args.Add(CommandLineTasksParametersGenerator.GenerateCommandLineParametersForTasks(targetsFromObject.Values));

            string spaceDelimitedArgs = String.Join(" ", args.ToArray());

            RemoteBounceExecutor.ExecuteRemoteBounce(spaceDelimitedArgs);
        }
    }
}