using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IDirectoryUtils {
        void CopyDirectory(string from, string to, IEnumerable<string> excludes, IEnumerable<string> includes);
        DateTime GetLastModTimeForDirectory(string dir);
        void DeleteDirectory(string dir);
        bool DirectoryExists(string dir);
    }
}