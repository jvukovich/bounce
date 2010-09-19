using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IDirectoryUtils {
        DateTime GetLastModTimeForDirectory(string dir);
        void CopyDirectoryContents(string from, string to, IEnumerable<string> excludes, IEnumerable<string> includes);
        void DeleteDirectory(string dir);
        bool DirectoryExists(string dir);
    }
}