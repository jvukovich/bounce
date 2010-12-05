using System.Collections.Generic;

namespace Bounce.Framework {
    public class RemoteBounce {
        private ITargetsParser TargetsParser;
        private List<object> RemoteTargets;

        public RemoteBounce(ITargetsParser targetsParser) {
            TargetsParser = targetsParser;
            RemoteTargets = new List<object>();
        }

        public RemoteBounce() : this(new TargetsParser()) { }

        public ITask Targets(object targets, IRemoteBounceExecutor remoteBounceExecutor) {
            RemoteTargets.Add(targets);
            return new RemoteBounceTask() {RemoteBounceExecutor = remoteBounceExecutor, Targets = targets};
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