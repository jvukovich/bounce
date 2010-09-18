using System;

namespace Bounce.Framework {
    public interface IDirectoryUtils {
        DateTime GetLastModTimeForDirectory(string dir);
        void CopyDirectoryContents(string from, string to);
        void DeleteDirectory(string dir);
        bool DirectoryExists(string dir);
    }
}