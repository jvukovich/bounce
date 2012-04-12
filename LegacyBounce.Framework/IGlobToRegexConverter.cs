using System.Text.RegularExpressions;

namespace LegacyBounce.Framework {
    public interface IGlobToRegexConverter {
        Regex ConvertToRegex(string glob);
    }
}