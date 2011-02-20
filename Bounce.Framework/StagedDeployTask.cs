using System;

namespace Bounce.Framework {
    public class StagedDeployTask : Task {
        [Dependency] public Task<string> Stage;
        public ITask Archive;
        public ITask RemoteDeploy;
        public ITask Deploy;

        public StagedDeployTask(Task<string> stage, ITask archive, ITask deploy) {
            Stage = stage;
            Archive = archive;
            Deploy = deploy;
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