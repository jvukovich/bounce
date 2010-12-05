using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class RemoteBounceTask : Task {
        public IRemoteProcessExecutor RemoteProcessExecutor { get; set; }
        public object Targets { get; set; }

        [Dependency]
        public Future<string> PathToBounceOnRemoteMachine { get; set; }

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
            List<string> args = new List<string>();

            args.Add(LogOptionCommandLineTranslator.GenerateCommandLine(bounce));

            IDictionary<string, ITask> targetsFromObject = TargetsParser.ParseTargetsFromObject(Targets);
            args.AddRange(targetsFromObject.Keys);
            args.Add(CommandLineTasksParametersGenerator.GenerateCommandLineParametersForTasks(targetsFromObject.Values));

            string spaceDelimitedArgs = String.Join(" ", args.ToArray());

            RemoteProcessExecutor.ExecuteRemoteProcess(PathToBounceOnRemoteMachine.Value, spaceDelimitedArgs);
        }
    }

    public class RemoteBounce {
        public IRemoteProcessExecutor RemoteProcessExecutor { get; set; }
        public Future<string> PathToBounceOnRemoteMachine { get; set; }

        private ITargetsParser TargetsParser;
        private List<object> RemoteTargets;

        public RemoteBounce(ITargetsParser targetsParser) {
            TargetsParser = targetsParser;
            RemoteTargets = new List<object>();
        }

        public RemoteBounce() : this(new TargetsParser()) { }

        public ITask Targets(object targets) {
            RemoteTargets.Add(targets);
            return new RemoteBounceTask() {PathToBounceOnRemoteMachine = PathToBounceOnRemoteMachine, RemoteProcessExecutor = RemoteProcessExecutor, Targets = targets};
        }

        public object WithRemoteTargets(object targetsObject) {
            var targets = TargetsParser.ParseTargetsFromObject(targetsObject);

            foreach (var remoteTarget in RemoteTargets) {
                foreach (KeyValuePair<string, ITask> target in TargetsParser.ParseTargetsFromObject(remoteTarget)) {
                    targets.Add(target.Key, target.Value);
                }
            }

            return targets;
        }
    }
}