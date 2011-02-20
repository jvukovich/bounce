namespace Bounce.Framework {
    public class BounceArchive : IDeployArchive {
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
}