using System.IO;

namespace Bounce.Console
{
    public interface IBounceDirectoryFinder
    {
        string FindBounceDirectory();
    }

    internal class BounceDirectoryFinder : IBounceDirectoryFinder
    {
        public string FindBounceDirectory()
        {
            return FindBounceDirectoryFrom(Directory.GetCurrentDirectory());
        }

        private static string FindBounceDirectoryFrom(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
                return null;

            var bounceDir = Path.Combine(dir, "Bounce");

            return Directory.Exists(bounceDir)
                ? bounceDir
                : FindBounceDirectoryFrom(Path.GetDirectoryName(dir));
        }
    }
}