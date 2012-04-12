using System.Text.RegularExpressions;

namespace Bounce.Framework.Obsolete {
    public interface IGlobToRegexConverter {
        Regex ConvertToRegex(string glob);
    }
}