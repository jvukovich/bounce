using System.IO;

namespace Bounce.Console {
    class BounceDirectoryFinder : IBounceDirectoryFinder {
        public string FindBounceDirectory() {
            return FindBounceDirectoryFrom(Directory.GetCurrentDirectory());
        }

        private string FindBounceDirectoryFrom(string dir) {
            if (string.IsNullOrEmpty(dir)) {
                return null;
            }

            var bounceDir = Path.Combine(dir, "Bounce");

            if (Directory.Exists(bounceDir))
            {
                return bounceDir;
            } else {
                return FindBounceDirectoryFrom(Path.GetDirectoryName(dir));
            }
        }
    }
}