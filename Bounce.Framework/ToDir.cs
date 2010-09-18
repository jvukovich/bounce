namespace Bounce.Framework {
    public class ToDir : Task {
        private readonly IDirectoryUtils DirUtils;

        public ToDir() : this(new DirectoryUtils()) {}

        public ToDir(IDirectoryUtils dirUtils) {
            DirUtils = dirUtils;
        }

        [Dependency]
        public Val<string> FromPath { get; set; }
        [Dependency]
        public Val<string> ToPath { get; set; }

        public override void Build() {
            var fromPath = FromPath.Value;
            var toPath = ToPath.Value;

            if (DirUtils.GetLastModTimeForDirectory(fromPath) > DirUtils.GetLastModTimeForDirectory(toPath)) {
                DirUtils.CopyDirectoryContents(fromPath, toPath);
            }
        }

        public override void Clean() {
            DirUtils.DeleteDirectory(ToPath.Value);
        }
    }
}