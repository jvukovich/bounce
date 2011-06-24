using System;
using System.Collections.Generic;
using System.IO;
using Bounce.Framework;

namespace MultiStageTargets
{
    public class BuildTargets
    {
        [Targets]
        public static object Targets(IParameters parameters) {
            var stage = parameters.Required<string>("stage");
            var machine = parameters.Required<string>("machine");

            var deployService = new Copy {
                FromPath = "service",
                ToPath = machine.WhenBuilt(m => String.Format(@"c:\deployments\install\{0}\service", m)),
            };
            var deployWeb = new Copy {
                FromPath = "web",
                ToPath = machine.WhenBuilt(m => String.Format(@"c:\deployments\install\{0}\web", m)),
            };

            var remoteMachineOne = GetRemoteMachine(stage, machine, "one");
            var remoteMachineTwo = GetRemoteMachine(stage, machine, "two");

            Task<IEnumerable<RemoteMachine>> serviceMachines = new[] {remoteMachineOne, remoteMachineTwo};
            Task<IEnumerable<RemoteMachine>> webMachines = new[] {remoteMachineTwo};

            var deployArchive = new StagedDeployArchive(stage, "archive");
            var service = deployArchive.Add("service", "service", deployService).WithRemoteDeploy(serviceMachines.SelectTasks(m => m.DeployTargets("Service")));
            var web = deployArchive.Add("web", "web", deployWeb).WithRemoteDeploy(webMachines.SelectTasks(m => m.DeployTargets("Web")));

            return new {
                Service = service,
                Web = web,
            };
        }

        public class RemoteMachine {
            private readonly Task<string> LocalDeployDirectory;
            private readonly Parameter<string> Stage;
            private readonly Parameter<string> MachineParameter;
            private readonly string MachineName;
            private ITask ArchiveCopiedToRemote;

            public RemoteMachine(Task<string> remoteDeployDirectory, Task<string> localDeployDirectory, Parameter<string> stage, Parameter<string> machineParameter, string machineName) {
                LocalDeployDirectory = localDeployDirectory;
                Stage = stage;
                MachineParameter = machineParameter;
                MachineName = machineName;
                ArchiveCopiedToRemote = new Copy {
                    FromPath = ".",
                    ToPath = remoteDeployDirectory,
                };
            }

            public SubBounce DeployTargets(params string [] targets) {
                return new SubBounce {
                    BounceArguments = new RemoteBounceArguments {Targets = targets}.WithParameter(Stage.WithValue("deploy")).WithParameter(MachineParameter.WithValue(MachineName)),
                    DependsOn = new [] {ArchiveCopiedToRemote},
                    WorkingDirectory = LocalDeployDirectory,
                };
            }
        }

        private static RemoteMachine GetRemoteMachine(Parameter<string> stage, Parameter<string> machine, string machineName) {
            return new RemoteMachine(Path.Combine(@"\\sonomorph\deployments\bigsolution", machineName),
                                    Path.Combine(@"c:\deployments\bigsolution", machineName), stage, machine,
                                    machineName);
        }
    }

    public class SubBounce : Task {
        [Dependency] public Task<string> BounceArguments;
        [Dependency] public Task<string> WorkingDirectory;

        public override void Build(IBounce bounce) {
            var cwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(WorkingDirectory.Value);
            try {
                bounce.ShellCommand.ExecuteAndExpectSuccess(@"bounce.exe", BounceArguments.Value);
            } finally {
                Directory.SetCurrentDirectory(cwd);
            }
        }
    }
}
