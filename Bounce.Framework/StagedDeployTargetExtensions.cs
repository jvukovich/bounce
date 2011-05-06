using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public static class StagedDeployTargetExtensions {
        public static Func<Task<string>, ITask> CopyToAndInvokeOnMachines(this StagedDeployTarget target,
                                                                          Task<IEnumerable<DeployMachine>> machineConfigurations,
                                                                          IRemoteBounceFactory remoteBounceFactory) {
            return package => machineConfigurations.SelectTasks(machConf => {
                var archiveOnRemote = new Copy {
                    FromPath = package,
                    ToPath = machConf.RemotePath,
                };

                var parameters = new List<IParameter>();
                parameters.Add(target.Stage.WithValue(StagedDeployTarget.RemoteDeployStage));
                if (machConf.BounceParameters != null) {
                    parameters.AddRange(machConf.BounceParameters);
                }

                                                                                Task<string> localPath = new All(archiveOnRemote, machConf.LocalPath).WhenBuilt(() => machConf.LocalPath.Value);
                return remoteBounceFactory.CreateRemoteBounce(BounceArguments.ForTarget(target.Name, parameters),
                                                              localPath,
                                                              machConf.Machine);
            });
        }
    }
}