using System;
using System.IO;

namespace Bounce.Framework.Obsolete {
    public class FileUtils : IFileUtils {
        public bool FileExists(string filename) {
            return File.Exists(filename);
        }

        public DateTime LastWriteTimeForFile(string filename) {
            return File.GetLastWriteTimeUtc(filename);
        }

        public void DeleteFile(string file) {
            File.Delete(file);
        }

        public void CopyFile(string from, string to) {
            File.Copy(from, to, true);
        }
    }
}