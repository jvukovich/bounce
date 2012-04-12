using System.Collections.Generic;

namespace LegacyBounce.Framework {
    public interface IFileNameFilterFactory {
        IFileNameFilter CreateFileNameFilter(IEnumerable<string> excludes, IEnumerable<string> includes);
    }
}