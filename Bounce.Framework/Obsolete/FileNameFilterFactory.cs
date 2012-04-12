using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    class FileNameFilterFactory : IFileNameFilterFactory {
        public IFileNameFilter CreateFileNameFilter(IEnumerable<string> excludes, IEnumerable<string> includes) {
            return new FileNameFilter(excludes ?? new string [0], includes ?? new string [0]);
        }
    }
}