namespace Bounce.Framework {
    public class CleanDirectory : Task {
        [Dependency] public Future<string> Path;
        private DirectoryUtils DirectoryUtils;

        public CleanDirectory() {
            DirectoryUtils = new DirectoryUtils();
        }

        public override void Build() {
            DirectoryUtils.DeleteDirectory(Path.Value);
            DirectoryUtils.CreateDirectory(Path.Value);
        }

        public override void Clean() {
            DirectoryUtils.DeleteDirectory(Path.Value);
        }

        public DirectoryFiles Files {
            get { return new DirectoryFiles(this, () => Path.Value); }
        }
    }
}