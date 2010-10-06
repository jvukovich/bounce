using System.IO;

namespace Bounce.Console {
    class BounceDirectoryCopier {
        public string CopyBounceDirectory(string targetsAssembly) {
            var bounceDirectory = Path.GetDirectoryName(targetsAssembly);
            var tempBounceDir = Path.Combine(Path.GetDirectoryName(bounceDirectory), bounceDirectory + ".tmp");

            if (Directory.Exists(tempBounceDir)) {
                Directory.Delete(tempBounceDir, true);
            }

            Directory.CreateDirectory(tempBounceDir);

            CopyDirectory(bounceDirectory, tempBounceDir);

            return Path.Combine(tempBounceDir, Path.GetFileName(targetsAssembly));
        }

        private void CopyDirectory(string fromDirectory, string toDirectory) {
            foreach (var fromFile in Directory.GetFiles(fromDirectory)) {
                var toFile = Path.Combine(toDirectory, Path.GetFileName(fromFile));
//                System.Console.WriteLine("copying {0} => {1}", fromFile, toFile);
                File.Copy(fromFile, toFile);
            }
        }
    }
}