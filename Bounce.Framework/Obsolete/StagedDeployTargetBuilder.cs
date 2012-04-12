using System.Collections.Generic;

namespace Bounce.Framework.Obsolete
{
    public class StagedDeployTargetBuilder
    {
        public IDictionary<string, ITask> Targets { get; private set; }
        private Parameter<string> Stage;

        public StagedDeployTargetBuilder(Parameter<string> stage, IDictionary<string, ITask> targets) {
            Targets = targets;
            Stage = stage;
        }

        public StagedDeployTargetBuilder(Parameter<string> stage) : this(stage, new Dictionary<string, ITask>()) {
        }

        public StagedDeployTarget CreateTarget(string name) {
            var target = new StagedDeployTarget(name, Stage);
            Targets[name] = target;
            return target;
        }
    }
}