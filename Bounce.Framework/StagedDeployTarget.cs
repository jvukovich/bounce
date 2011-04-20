using System;
using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public class StagedDeployTarget : Task
    {
        private readonly string TargetName;

        private readonly Parameter<string> Stage;
        [Dependency]
        private readonly Task<IEnumerable<DeployMachine>> MachineConfigurations;

        private readonly IRemoteBounceFactory RemoteBounceFactory;

        [Dependency]
        private readonly Switch Switch;

        public StagedDeployTarget(IDictionary<string, ITask> targets, string targetName, Parameter<string> stage, Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory)
        {
            TargetName = targetName;
            Stage = stage;
            MachineConfigurations = machineConfigurations;
            RemoteBounceFactory = remoteBounceFactory;
            Switch = new Switch(stage);
            targets[targetName] = this;
        }

        private void SetupSwitch() {
            if (Build != null && Deploy != null) {
                var buildWithBounce = CopyBounceDirectoryIntoArchive(Build);

                Switch["build"] = buildWithBounce;
                Switch["remoteDeploy"] = GetRemoteDeploy(".");
                Switch["deploy"] = Deploy(".");
                Switch["buildRemoteDeploy"] = GetRemoteDeploy(buildWithBounce);
                Switch["buildDeploy"] = Deploy(Build);
            }
        }

        private ITask GetRemoteDeploy(Task<string> archive) {
            return MachineConfigurations.SelectTasks(machConf => {
                var archiveOnRemote = new Copy {
                    FromPath = archive,
                    ToPath = machConf.RemotePath,
                };

                var parameters = new List<IParameter>();
                parameters.Add(Stage.WithValue("deploy"));
                parameters.AddRange(machConf.BounceParameters);

                var localPath = new All(archiveOnRemote, machConf.LocalPath).WhenBuilt(() => machConf.LocalPath.Value);
                return RemoteBounceFactory.CreateRemoteBounce(BounceArguments.ForTarget(TargetName, parameters), localPath, machConf.Machine);
            });
        }

        private Func<Task<string>, ITask> _deploy;
        public Func<Task<string>, ITask> Deploy {
            get { return _deploy; }
            set {
                _deploy = value;
                SetupSwitch();
            }
        }

        private Task<string> _build;
        public new Task<string> Build {
            get { return _build; }
            set {
                _build = value;
                SetupSwitch();
            }
        }

        private Task<string> CopyBounceDirectoryIntoArchive(Task<string> archive) {
            return new Copy {
                FromPath = Path.GetDirectoryName(BounceRunner.TargetsPath),
                ToPath = archive.SubPath("Bounce"),
            }.ToPath.SubPath("..");
        }

        public override bool IsLogged
        {
            get
            {
                return false;
            }
        }
    }
}