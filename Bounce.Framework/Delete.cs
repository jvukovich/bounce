using System.IO;

namespace Bounce.Framework {
    public class Delete : Task {
        [Dependency]
        public Task<string> Path;

        private IDirectoryUtils DirectoryUtils;
               
        public Delete() {
            DirectoryUtils = new DirectoryUtils();
        }

        public override void Build(IBounce bounce) {
            foreach (var file in Directory.GetFiles(".", Path.Value))
            {
                bounce.Log.Debug("deleting file: `{0}'", file);
                File.Delete(file);
            }

            foreach (var directory in Directory.GetDirectories(".", Path.Value))
            {
                bounce.Log.Debug("deleting directory: `{0}'", directory);
                DirectoryUtils.DeleteDirectory(directory);
            }
        }
    }
}