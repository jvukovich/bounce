using System.Collections.Generic;

namespace Bounce.Framework {
    public interface IFileNameFilterFactory {
        IFileNameFilter CreateFileNameFilter(IEnumerable<string> excludes, IEnumerable<string> includes);
    }
}