using System.Collections.Generic;

namespace Bounce.Framework
{
    public class StagedDeployTargetBuilder
    {
        public IDictionary<string, ITask> Targets { get; private set; }
        private Task<IEnumerable<DeployMachine>> MachineConfigurations;
        private IRemoteBounceFactory RemoteBounceFactory;
        private Parameter<string> Stage;
        private CachedRemoteDeploy CachedRemoteDeploys;

        public StagedDeployTargetBuilder(Parameter<string> stage) {
            Stage = stage;
            Targets = new Dictionary<string, ITask>();
            CachedRemoteDeploys = new CachedRemoteDeploy();
        }

        public StagedDeployTargetBuilder() : this(null) {
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
            return new StagedDeployTargetBuilder {
                           Targets = Targets,
                           MachineConfigurations = MachineConfigurations,
                           RemoteBounceFactory = RemoteBounceFactory,
                           Stage = Stage,
                           CachedRemoteDeploys = CachedRemoteDeploys
                       };
        }

        public StagedDeployTarget CreateTarget(string name, Task<IEnumerable<DeployMachine>> machines) {
            var target = new StagedDeployTarget(name, Stage, machines, RemoteBounceFactory, CachedRemoteDeploys);
            Targets[name] = target;
            return target;
        }

        public StagedDeployTarget CreateTarget(string name) {
            return CreateTarget(name, MachineConfigurations);
        }
    }
}