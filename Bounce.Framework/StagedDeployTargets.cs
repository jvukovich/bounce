using System.Collections.Generic;

namespace Bounce.Framework {
    public class StagedDeployTargets
    {
        public IDictionary<string, ITask> Targets { get; private set; }
        private readonly Task<IEnumerable<DeployMachine>> MachineConfigurations;
        private readonly IRemoteBounceFactory RemoteBounceFactory;
        private readonly Parameter<string> Stage;

        public StagedDeployTargets(Parameter<string> stage, Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory)
        {
            Targets = new Dictionary<string, ITask>();
            MachineConfigurations = machineConfigurations;
            RemoteBounceFactory = remoteBounceFactory;
            Stage = stage;
        }

        public StagedDeployTarget CreateTarget(string name)
        {
            return new StagedDeployTarget(Targets, name, Stage, MachineConfigurations, RemoteBounceFactory);
        }
    }
}