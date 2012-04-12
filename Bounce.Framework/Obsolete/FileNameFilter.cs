using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bounce.Framework.Obsolete {
    public interface IFileNameFilter {
        bool IncludeFile(string filename);
    }

    public class FileNameFilter : IFileNameFilter {
        private readonly IGlobToRegexConverter globToRegexConverter;
        private readonly IEnumerable<Regex> toExclude;
        private readonly IEnumerable<Regex> toInclude;

        public FileNameFilter(IEnumerable<string> toExclude, IEnumerable<string> toInclude) : this(new GlobToRegexConverter(), toExclude, toInclude) {}

        public FileNameFilter(IGlobToRegexConverter globToRegexConverter, IEnumerable<string> toExclude, IEnumerable<string> toInclude) {
            this.globToRegexConverter = globToRegexConverter;
            this.toExclude = BuildRegexesFromGlobs(toExclude);
            this.toInclude = BuildRegexesFromGlobs(toInclude);
        }

        private IEnumerable<Regex> BuildRegexesFromGlobs(IEnumerable<string> globs) {
            return globs.Select(g => globToRegexConverter.ConvertToRegex(g)).ToArray();
        }

        public bool IncludeFile(string filename) {
            return !toExclude.Any(e => e.IsMatch(filename)) || toInclude.Any(e => e.IsMatch(filename));
        }
    }
}