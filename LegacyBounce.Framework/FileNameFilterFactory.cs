using System.Collections.Generic;

namespace LegacyBounce.Framework {
    class FileNameFilterFactory : IFileNameFilterFactory {
        public IFileNameFilter CreateFileNameFilter(IEnumerable<string> excludes, IEnumerable<string> includes) {
            return new FileNameFilter(excludes ?? new string [0], includes ?? new string [0]);
        }
    }
}