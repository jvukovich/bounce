using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Bounce.TestHelpers
{
    public static class FileSystemTestHelper {
        public static void RecreateDirectory(string d) {
            if (Directory.Exists(d)) {
                Directory.Delete(d, true);
            }
            Directory.CreateDirectory(d);
        }
    }
}
