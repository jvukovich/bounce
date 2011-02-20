namespace Bounce.Framework {
    public class StagedDeployArchive {
        private Task<string> Stage;
        private IDeployArchive Archive;
        public ITask RemoteDeploy { get; set; }

        public StagedDeployArchive(Task<string> stage, Task<string> archivePath) : this(stage, new BounceArchive(archivePath)) {
        }

        public StagedDeployArchive(Task<string> stage, IDeployArchive deployArchive) {
            Stage = stage;
            Archive = deployArchive;
        }

        public StagedDeployTask Add(Task<string> archivePath, Task<string> from, ITask deploy) {
            var archiveTask = Archive.Add(from, archivePath);
            return new StagedDeployTask(Stage, archiveTask, deploy).WithRemoteDeploy(RemoteDeploy);
        }
    }
}