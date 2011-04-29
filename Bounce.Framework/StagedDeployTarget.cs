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
        private readonly CachedRemoteDeploy CachedRemoteDeploys;

        [Dependency]
        private readonly Switch Switch;

        public StagedDeployTarget(string name, Parameter<string> stage, Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory, CachedRemoteDeploy cachedRemoteDeploys)
        {
            Name = name;
            Stage = stage;
            MachineConfigurations = machineConfigurations;
            RemoteBounceFactory = remoteBounceFactory;
            CachedRemoteDeploys = cachedRemoteDeploys;
            Switch = new Switch(stage);
        }

        private void SetupSwitch() {
            if (Package != null && Deploy != null) {
                var packageWithBounce = CopyBounceDirectoryIntoPackage(Package);

                Switch["package"] = packageWithBounce;
                Switch["remoteDeploy"] = GetRemoteDeploy(".");
                Switch["deploy"] = Deploy(".");
                Switch["packageRemoteDeploy"] = GetRemoteDeploy(packageWithBounce);
                Switch["packageDeploy"] = Deploy(PackageWithRemoteDeploy());
            }
        }

        private Task<string> PackageWithRemoteDeploy()
        {
            if (RemoteDeploy != null)
            {
                return CachedRemoteDeploys.RemoteDeploy(Package, RemoteDeploy).WhenBuilt(() => Package.Value);
            } else
            {
                return Package;
            }
        }

        private ITask GetRemoteDeploy(Task<string> archive)
        {
            ITask remoteDeploy = GetRemoteDeployTask(archive);

            return MachineConfigurations.SelectManyTasks(machConf =>
            {
                if (Deploy != null)
                {
                    var archiveOnRemote = new Copy
                    {
                        FromPath = archive,
                        ToPath = machConf.RemotePath,
                    }.ToPath.WithDependencyOn(remoteDeploy);

                    var parameters = new List<IParameter>();
                    parameters.Add(Stage.WithValue("deploy"));
                    parameters.AddRange(machConf.BounceParameters);

                    var localPath = new All(archiveOnRemote, machConf.LocalPath).WhenBuilt(() => machConf.LocalPath.Value);
                    return new[]
                    {
                        RemoteBounceFactory.CreateRemoteBounce(BounceArguments.ForTarget(Name, parameters), localPath,
                                                               machConf.Machine)
                    };
                }
                else if (RemoteDeploy != null)
                {
                    return new[] {remoteDeploy};
                }
                else
                {
                    return new ITask[0];
                }
            });
        }

        private ITask GetRemoteDeployTask(Task<string> archive)
        {
            return ((Task<bool>) (RemoteDeploy != null)).IfTrue(CachedRemoteDeploys.RemoteDeploy(archive, RemoteDeploy));
        }

        private Func<Task<string>, ITask> _deploy;
        public Func<Task<string>, ITask> Deploy {
            get { return _deploy; }
            set {
                _deploy = value;
                SetupSwitch();
            }
        }

        private Task<string> _package;
        public Task<string> Package {
            get { return _package; }
            set {
                _package = value;
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

        private Task<string> CopyBounceDirectoryIntoPackage(Task<string> package) {
            return new Copy {
                FromPath = Path.GetDirectoryName(BounceRunner.TargetsPath),
                ToPath = package.SubPath("Bounce"),
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

    public class CachedRemoteDeploy {
        private Dictionary<Task<string>, ITask> RemoteDeploys;
        public CachedRemoteDeploy() {
            RemoteDeploys = new Dictionary<Task<string>, ITask>();
        }

        public ITask RemoteDeploy(Task<string> package, Func<Task<string>, ITask> remoteDeploy) {
            ITask r;
            if (!RemoteDeploys.TryGetValue(package, out r)) {
                RemoteDeploys[package] = r = remoteDeploy(package);
            }
            return r;
        }
    }
}