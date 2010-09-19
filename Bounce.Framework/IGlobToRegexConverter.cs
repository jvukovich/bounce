using System.Text.RegularExpressions;

namespace Bounce.Framework {
    public interface IGlobToRegexConverter {
        Regex ConvertToRegex(string glob);
    }
}