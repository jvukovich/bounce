using System.IO;

namespace Bounce.TestHelpers
{
    public static class FileSystemTestHelper
    {
        public static void RecreateDirectory(string d)
        {
            if (Directory.Exists(d))
                Directory.Delete(d, true);

            Directory.CreateDirectory(d);
        }
    }
}