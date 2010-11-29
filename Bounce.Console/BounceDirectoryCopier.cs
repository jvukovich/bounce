using System.IO;

namespace Bounce.Console {
    class BounceDirectoryCopier {
        public string CopyBounceDirectory(OptionsAndArguments optionsAndArguments) {
            var bounceDirectory = Path.GetDirectoryName(optionsAndArguments.TargetsAssembly);
            var tempBounceDir = Path.Combine(Path.GetDirectoryName(bounceDirectory), bounceDirectory + ".tmp");

            if (!optionsAndArguments.Recurse)
            {
                if (Directory.Exists(tempBounceDir))
                {
                    Directory.Delete(tempBounceDir, true);
                }

                Directory.CreateDirectory(tempBounceDir);

                CopyDirectory(bounceDirectory, tempBounceDir);
            }

            return Path.Combine(tempBounceDir, Path.GetFileName(optionsAndArguments.TargetsAssembly));
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