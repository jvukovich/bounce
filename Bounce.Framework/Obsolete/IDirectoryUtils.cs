using System;
using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface IDirectoryUtils {
        void CopyDirectory(string from, string to, IEnumerable<string> excludes, IEnumerable<string> includes);
        DateTime GetLastModTimeForDirectory(string dir);
        void DeleteDirectory(string dir);
        void DeleteDirectoryContents(string dir);
        bool DirectoryExists(string dir);
        void CreateDirectory(string dir);
    }
}