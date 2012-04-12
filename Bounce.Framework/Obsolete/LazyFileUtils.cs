using System;

namespace Bounce.Framework.Obsolete {
    public class LazyFileUtils : IFileUtils {
        private readonly IFileUtils FileUtils;

        public LazyFileUtils() : this(new FileUtils()) {}

        public LazyFileUtils(IFileUtils fileUtils) {
            FileUtils = fileUtils;
        }

        public bool FileExists(string filename) {
            return FileUtils.FileExists(filename);
        }

        public DateTime LastWriteTimeForFile(string filename) {
            return FileUtils.LastWriteTimeForFile(filename);
        }

        public void DeleteFile(string file) {
            FileUtils.DeleteFile(file);
        }

        public void CopyFile(string from, string to) {
            if (!FileUtils.FileExists(to) || FileUtils.LastWriteTimeForFile(from) > FileUtils.LastWriteTimeForFile(to)) {
                FileUtils.CopyFile(from, to);
            }
        }
    }
}