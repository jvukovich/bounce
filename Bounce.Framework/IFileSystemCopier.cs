using System;
using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IFileSystemCopier {
        DateTime GetLastModTimeForPath(string path);
        void Copy(string from, string to, IEnumerable<string> excludes, IEnumerable<string> includes);
        void Delete(string path);
        bool Exists(string path);
    }
}