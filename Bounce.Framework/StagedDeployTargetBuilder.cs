using System.Collections.Generic;

namespace Bounce.Framework
{
    public class StagedDeployTargetBuilder
    {
        public IDictionary<string, ITask> Targets { get; private set; }
        public Task<IEnumerable<DeployMachine>> MachineConfigurations { get; private set; }
        public IRemoteBounceFactory RemoteBounceFactory { get; private set; }
        public Parameter<string> Stage { get; private set; }

        public StagedDeployTargetBuilder()
        {
            Targets = new Dictionary<string, ITask>();
        }

        public StagedDeployTargetBuilder ForMachines(Task<IEnumerable<DeployMachine>> machineConfigurations)
        {
            StagedDeployTargetBuilder builder = Clone();
            builder.MachineConfigurations = machineConfigurations;
            return builder;
        }

        public StagedDeployTargetBuilder WithTargets(IDictionary<string, ITask> targets)
        {
            StagedDeployTargetBuilder builder = Clone();
            builder.Targets = targets;
            return builder;
        }

        public StagedDeployTargetBuilder WithRemoteBounceFactory(IRemoteBounceFactory remoteBounceFactory)
        {
            StagedDeployTargetBuilder builder = Clone();
            builder.RemoteBounceFactory = remoteBounceFactory;
            return builder;
        }

        public StagedDeployTargetBuilder WithStage(Parameter<string> stage)
        {
            StagedDeployTargetBuilder builder = Clone();
            builder.Stage = stage;
            return builder;
        }

        private StagedDeployTargetBuilder Clone()
        {
            return new StagedDeployTargetBuilder
                       {
                           Targets = Targets,
                           MachineConfigurations = MachineConfigurations,
                           RemoteBounceFactory = RemoteBounceFactory,
                           Stage = Stage
                       };
        }

        public StagedDeployTarget CreateTarget(string name)
        {
            var target = new StagedDeployTarget(name, Stage, MachineConfigurations, RemoteBounceFactory);
            Targets[name] = target;
            return target;
        }
    }
}