namespace LegacyBounce.Framework {
    public class CleanDirectory : Task {
        [Dependency] private Task<string> _path;
        private DirectoryUtils DirectoryUtils;

        public CleanDirectory() {
            DirectoryUtils = new DirectoryUtils();
        }

        public override void Build() {
            DirectoryUtils.DeleteDirectory(_path.Value);
            DirectoryUtils.CreateDirectory(_path.Value);
        }

        public override void Clean() {
            DirectoryUtils.DeleteDirectory(_path.Value);
        }

        public DirectoryFiles Files {
            get { return new DirectoryFiles(Path); }
        }

        public Task<string> Path {
            get { return this.WhenBuilt(() => _path.Value); }
            set { _path = value; }
        }
    }
}