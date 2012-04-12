using System;
using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface IFileSystemCopier {
        DateTime GetLastModTimeForPath(string path);
        void Copy(string from, string to, IEnumerable<string> excludes, IEnumerable<string> includes, bool deleteToDirectory);
        void Delete(string path);
        bool Exists(string path);
    }
}