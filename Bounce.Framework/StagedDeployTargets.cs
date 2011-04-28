using System.Collections.Generic;

namespace Bounce.Framework {
    public class StagedDeployTargets
    {
        public IDictionary<string, ITask> Targets { get; private set; }
        private readonly Task<IEnumerable<DeployMachine>> MachineConfigurations;
        private readonly IRemoteBounceFactory RemoteBounceFactory;
        private readonly Parameter<string> Stage;

        public StagedDeployTargets(Parameter<string> stage, Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory)
            : this(new Dictionary<string, ITask>(), stage, machineConfigurations, remoteBounceFactory)
        {
        }

        public StagedDeployTargets(IDictionary<string, ITask> targets, Parameter<string> stage, Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory)
        {
            Targets = targets;
            MachineConfigurations = machineConfigurations;
            RemoteBounceFactory = remoteBounceFactory;
            Stage = stage;
        }

        public StagedDeployTarget CreateTarget(string name)
        {
            var target = new StagedDeployTarget(name, Stage, MachineConfigurations, RemoteBounceFactory);
            Targets[name] = target;
            return target;
        }
    }
}