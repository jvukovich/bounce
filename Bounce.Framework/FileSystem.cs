using System;
using System.IO;

namespace Bounce.Framework {
    class FileSystem : IFileSystem {
        public bool FileExists(string filename) {
            return File.Exists(filename);
        }

        public DateTime LastWriteTimeForFile(string filename) {
            return File.GetLastWriteTimeUtc(filename);
        }
    }
}