using System;
using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public class StagedDeployTarget : Task
    {
        public string Name { get; private set; }

        public const string RemoteDeployStage = "remoteDeploy";
        public const string PackageStage = "package";
        public const string InvokeRemoteDeployStage = "invokeRemoteDeploy";
        public const string PackageInvokeRemoteDeployStage = "packageInvokeRemoteDeploy";
        public const string PackageDeployStage = "packageDeploy";
        public const string DeployStage = "deploy";

        private readonly Parameter<string> Stage;
        private readonly CachedDeploys CachedDeploys;

        [Dependency]
        private readonly Switch Switch;

        public StagedDeployTarget(string name, Parameter<string> stage, CachedDeploys cachedDeploys)
        {
            Name = name;
            Stage = stage;
            CachedDeploys = cachedDeploys;
            Switch = new Switch(stage);
            Deploy = package => new NullTask();
        }

        private void SetupSwitch() {
            if (Package != null && DeployOnHost != null) {
                var packageWithBounce = CopyBounceDirectoryIntoPackage(Package);

                Switch[PackageStage] = packageWithBounce;
                Switch[InvokeRemoteDeployStage] = GetInvokeRemoteDeploy(".");
                Switch[RemoteDeployStage] = DeployOnHost(".");
                Switch[PackageInvokeRemoteDeployStage] = GetInvokeRemoteDeploy(packageWithBounce);
                Switch[PackageDeployStage] = DeployOnHost(GetDeployedPackage(Package));
                Switch[DeployStage] = DeployOnHost(GetDeployedPackage("."));
            }
        }

        private Task<string> GetDeployedPackage(Task<string> package) {
            return package.WithDependencyOn(CachedDeploys.Deploy(package, Deploy));
        }

        private ITask GetInvokeRemoteDeploy(Task<string> package)
        {
            Task<string> packageAfterDeploy = GetDeployedPackage(package);

            if (DeployOnHost != null) {
                return InvokeRemoteDeploy(packageAfterDeploy);
            } else {
                return packageAfterDeploy;
            }
        }

        public Func<Task<string>, ITask> CopyToAndInvokeOnMachines(Task<IEnumerable<DeployMachine>> machineConfigurations, IRemoteBounceFactory remoteBounceFactory) {
            return package => machineConfigurations.SelectTasks(machConf =>
            {
                var archiveOnRemote = new Copy
                {
                    FromPath = package,
                    ToPath = machConf.RemotePath,
                };

                var parameters = new List<IParameter>();
                parameters.Add(Stage.WithValue(RemoteDeployStage));
                parameters.AddRange(machConf.BounceParameters);

                var localPath = new All(archiveOnRemote, machConf.LocalPath).WhenBuilt(() => machConf.LocalPath.Value);
                return remoteBounceFactory.CreateRemoteBounce(BounceArguments.ForTarget(Name, parameters),
                                                              localPath,
                                                              machConf.Machine);
            });
        }

        private Func<Task<string>, ITask> _deployOnHost;
        public Func<Task<string>, ITask> DeployOnHost {
            get { return _deployOnHost; }
            set {
                _deployOnHost = value;
                SetupSwitch();
            }
        }

        private Func<Task<string>, ITask> _invokeRemoteDeploy;
        public Func<Task<string>, ITask> InvokeRemoteDeploy {
            get { return _invokeRemoteDeploy; }
            set {
                _invokeRemoteDeploy = value;
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

        private Func<Task<string>, ITask> _deploy;

        public Func<Task<string>, ITask> Deploy {
            get { return _deploy; }
            set {
                _deploy = value;
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

    public class CachedDeploys {
        private Dictionary<Task<string>, ITask> RemoteDeploys;
        public CachedDeploys() {
            RemoteDeploys = new Dictionary<Task<string>, ITask>();
        }

        public ITask Deploy(Task<string> package, Func<Task<string>, ITask> remoteDeploy) {
            ITask r;
            if (!RemoteDeploys.TryGetValue(package, out r)) {
                RemoteDeploys[package] = r = remoteDeploy(package);
            }
            return r;
        }
    }
}