using System.IO;

namespace Bounce.Framework {
    public class Delete : Task {
        [Dependency]
        public Task<string> Path;

        private IDirectoryUtils DirectoryUtils;
               
        public Delete() {
            DirectoryUtils = new DirectoryUtils();
        }

        public override void Build() {
            foreach (var file in Directory.GetFiles(".", Path.Value))
            {
                File.Delete(file);
            }

            foreach (var directory in Directory.GetDirectories(".", Path.Value))
            {
                DirectoryUtils.DeleteDirectory(directory);
            }
        }
    }
}