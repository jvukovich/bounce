using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public interface IFileNameFilterFactory {
        IFileNameFilter CreateFileNameFilter(IEnumerable<string> excludes, IEnumerable<string> includes);
    }
}