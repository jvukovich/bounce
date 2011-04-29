using System.Collections.Generic;

namespace Bounce.Framework
{
    public class StagedDeployTargetBuilder
    {
        public IDictionary<string, ITask> Targets { get; private set; }
        private Parameter<string> Stage;
        private CachedDeploys CachedDeploys;

        public StagedDeployTargetBuilder(Parameter<string> stage, IDictionary<string, ITask> targets) {
            Targets = targets;
            Stage = stage;
            CachedDeploys = new CachedDeploys();
        }

        public StagedDeployTargetBuilder(Parameter<string> stage) : this(stage, new Dictionary<string, ITask>()) {
        }

        public StagedDeployTarget CreateTarget(string name) {
            var target = new StagedDeployTarget(name, Stage, CachedDeploys);
            Targets[name] = target;
            return target;
        }
    }
}