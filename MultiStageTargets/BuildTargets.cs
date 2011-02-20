using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            var bounceArchive = new RemoteDeployArchive(stage, "archive");
            var service = bounceArchive.Add("service", "service").WithDeploy(deployService).WithRemoteDeploy(new All(remoteMachineOne.DeployTargets("Service"), remoteMachineTwo.DeployTargets("Service")));
            var web = bounceArchive.Add("web", "web").WithDeploy(deployWeb).WithRemoteDeploy(remoteMachineTwo.DeployTargets("Web"));

            return new {
                Service = service,
                Web = web,
            };
        }

        public class RemoteMachine {
            private readonly Task<string> LocalDeployDirectory;
            private readonly Task<string> Stage;
            private readonly Task<string> MachineParameter;
            private readonly string MachineName;
            private ITask ArchiveCopiedToRemote;

            public RemoteMachine(Task<string> remoteDeployDirectory, Task<string> localDeployDirectory, Task<string> stage, Task<string> machineParameter, string machineName) {
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
                    Arguments = new RemoteBounceArguments {Targets = targets}.WithRemoteParameter(Stage, "deploy").WithRemoteParameter(MachineParameter, MachineName),
                    DependsOn = new [] {new TaskDependency {Task = ArchiveCopiedToRemote}},
                    WorkingDirectory = LocalDeployDirectory,
                };
            }
        }

        private static RemoteMachine GetRemoteMachine(Task<string> stage, Task<string> machine, string machineName) {
            return new RemoteMachine(Path.Combine(@"\\sonomorph\deployments\bigsolution", machineName),
                                    Path.Combine(@"c:\deployments\bigsolution", machineName), stage, machine,
                                    machineName);
        }
    }

    public class SubBounce : Task {
        [Dependency] public Task<string> Arguments;
        [Dependency] public Task<string> WorkingDirectory;

        public override void Build(IBounce bounce) {
            var cwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(WorkingDirectory.Value);
            try {
                bounce.ShellCommand.ExecuteAndExpectSuccess(@"bounce.exe", Arguments.Value);
            } finally {
                Directory.SetCurrentDirectory(cwd);
            }
        }
    }

    public class BounceArchive {
        private Task<string> ArchivedBounce;
        private CleanDirectory ArchiveRoot;

        public BounceArchive(Task<string> path) {
            ArchiveRoot = new CleanDirectory {Path = path};

            ArchivedBounce = new Copy {
                FromPath = @"bounce",
                ToPath = ArchiveRoot.Files["bounce"],
            }.ToPath;
        }

        public Task<string> Add(Task<string> from, Task<string> archivePath) {
            return new Copy {
                FromPath = from,
                ToPath = ArchiveRoot.Files[archivePath],
                DependsOn = new [] {new TaskDependency {Task = ArchivedBounce}},
            }.ToPath;
        }
    }

    public class RemoteDeployArchive {
        private Task<string> Stage;
        private BounceArchive Archive;
        public ITask RemoteDeploy { get; set; }

        public RemoteDeployArchive(Task<string> stage, Task<string> path) {
            Stage = stage;
            Archive = new BounceArchive(path);
        }

        public StagedDeployTask Add(Task<string> archivePath, Task<string> from) {
            var archiveTask = Archive.Add(from, archivePath);
            return new StagedDeployTask(Stage, archiveTask).WithRemoteDeploy(RemoteDeploy);
        }
    }

    public class StagedDeployTask : Task {
        [Dependency] public Task<string> Stage;
        public ITask Archive;
        public ITask RemoteDeploy;
        public ITask Deploy;

        public StagedDeployTask(Task<string> stage, ITask archive) {
            Stage = stage;
            Archive = archive;
        }

        public StagedDeployTask WithDeploy(ITask deploy) {
            Deploy = deploy;
            return this;
        }

        public StagedDeployTask WithRemoteDeploy(ITask remoteDeploy) {
            RemoteDeploy = remoteDeploy;
            return this;
        }

        public override void Invoke(IBounceCommand command, IBounce bounce) {
            bounce.Invoke(command, GetStageTask());
        }

        private ITask GetStageTask() {
            switch (Stage.Value) {
                case "archive":
                    return Archive;
                case "remoteDeploy":
                    return RemoteDeploy;
                case "deploy":
                    return Deploy;
                default:
                    throw new ApplicationException("no such stage: " + Stage.Value);
            }
        }
    }
}
