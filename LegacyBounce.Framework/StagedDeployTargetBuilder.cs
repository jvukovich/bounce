using System.Collections.Generic;

namespace LegacyBounce.Framework
{
    public class StagedDeployTargetBuilder
    {
        public IDictionary<string, IObsoleteTask> Targets { get; private set; }
        private Parameter<string> Stage;

        public StagedDeployTargetBuilder(Parameter<string> stage, IDictionary<string, IObsoleteTask> targets) {
            Targets = targets;
            Stage = stage;
        }

        public StagedDeployTargetBuilder(Parameter<string> stage) : this(stage, new Dictionary<string, IObsoleteTask>()) {
        }

        public StagedDeployTarget CreateTarget(string name) {
            var target = new StagedDeployTarget(name, Stage);
            Targets[name] = target;
            return target;
        }
    }
}