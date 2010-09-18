using System;

namespace Bounce.Framework {
    public interface IFileSystem {
        bool FileExists(string filename);
        DateTime LastWriteTimeForFile(string filename);
    }
}