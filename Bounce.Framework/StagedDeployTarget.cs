using System;
using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public class StagedDeployTarget : Task
    {
        public string Name { get; private set; }

        private readonly Parameter<string> Stage;
        [Dependency]
        private readonly Task<IEnumerable<DeployMachine>> MachineConfigurations;

        private readonly IRemoteBounceFactory RemoteBounceFactory;

        [Dependency]
        private readonly Switch Switch;

        public StagedDeployTarget(string name, Parameter<string> stage, Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory)
        {
            Name = name;
            Stage = stage;
            MachineConfigurations = machineConfigurations;
            RemoteBounceFactory = remoteBounceFactory;
            Switch = new Switch(stage);
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
            return MachineConfigurations.SelectManyTasks(machConf => {
                if (Deploy != null)
                {
                    var archiveOnRemote = new Copy
                    {
                        FromPath = archive,
                        ToPath = machConf.RemotePath,
                    }.ToPath;

                    if (RemoteDeploy != null)
                    {
                        archiveOnRemote = archiveOnRemote.WithDependencyOn(RemoteDeploy(archive));
                    }

                    var parameters = new List<IParameter>();
                    parameters.Add(Stage.WithValue("deploy"));
                    parameters.AddRange(machConf.BounceParameters);

                    var localPath = new All(archiveOnRemote, machConf.LocalPath).WhenBuilt(() => machConf.LocalPath.Value);
                    return new[] { RemoteBounceFactory.CreateRemoteBounce(BounceArguments.ForTarget(Name, parameters), localPath, machConf.Machine) };
                } else if (RemoteDeploy != null)
                {
                    return new[] {RemoteDeploy(archive)};
                } else
                {
                    return new ITask[0];
                }
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

        private Func<Task<string>, ITask> _remoteDeploy;
        public Func<Task<string>, ITask> RemoteDeploy {
            get { return _remoteDeploy; }
            set {
                _remoteDeploy = value;
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