using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Bounce.Framework.Obsolete {
    public class GlobToRegexConverter : IGlobToRegexConverter {
        private IEnumerable<Func<char[], int, ParsedRegex>> Parsers;

        public GlobToRegexConverter() {
            Parsers = new Func<char[], int, ParsedRegex>[] { ParseLeadingStarStarSlash, ParseStar, ParseDot, ParseQuesionMark, ParseTrailingSlash, ParseSlash };
        }

        public Regex ConvertToRegex(string glob) {
            return new Regex(GetRegexStringFromGlob(glob));
        }

        private string GetRegexStringFromGlob(string glob) {
            ParsedRegex parsedRegex = ParseGlob(glob.ToCharArray(), 0);

            if (parsedRegex != null && parsedRegex.Index == glob.Length) {
                return parsedRegex.Regex;
            } else {
                throw new ConfigurationException(string.Format("syntax error in glob `{0}'", glob));
            }
        }

        class ParsedRegex {
            public string Regex;
            public int Index;

            public ParsedRegex(string regex, int index) {
                Regex = regex;
                Index = index;
            }
        }

        private ParsedRegex ParseGlob(char [] glob, int index) {
            var regex = new StringBuilder();

            while (index < glob.Length) {
                ParsedRegex parsedRegex;

                if ((parsedRegex = ParseSpecial(glob, index)) != null) {
                    regex.Append(parsedRegex.Regex);
                    index = parsedRegex.Index;
                } else {
                    regex.Append(glob[index]);
                    index++;
                }
            }

            return new ParsedRegex(@"^" + regex + "$", index);
        }

        private ParsedRegex ParseSpecial(char [] glob, int index) {
            foreach (var parser in Parsers) {
                ParsedRegex parsedRegex = parser(glob, index);
                if (parsedRegex != null) {
                    return parsedRegex;
                }
            }

            return null;
        }

        private ParsedRegex ParseStar(char [] glob, int index) {
            if (glob[index] == '*') {
                index++;
                if (index < glob.Length && glob[index] == '*') {
                    return new ParsedRegex(@".*", index + 1);
                }

                return new ParsedRegex(@"[^\\/]*", index);
            }

            return null;
        }

        private ParsedRegex ParseDot(char[] glob, int index) {
            if (glob[index] == '.') {
                return new ParsedRegex(@"\.", index + 1);
            } else {
                return null;
            }
        }

        private ParsedRegex ParseQuesionMark(char[] glob, int index) {
            if (glob[index] == '?') {
                return new ParsedRegex(@"[^\\/]", index + 1);
            } else {
                return null;
            }
        }

        private ParsedRegex ParseTrailingSlash(char[] glob, int index) {
            if (glob.Length == (index + 1) && glob[index] == '\\') {
                return new ParsedRegex(@"\\.*", index + 1);
            } else {
                return null;
            }
        }

        private ParsedRegex ParseSlash(char[] glob, int index) {
            if (glob[index] == '\\') {
                return new ParsedRegex(@"\\", index + 1);
            } else {
                return null;
            }
        }

        private ParsedRegex ParseLeadingStarStarSlash(char[] glob, int index) {
            if (index == 0 && glob.Length >= 3 && glob[index] == '*' && glob[index + 1] == '*' && glob[index + 2] == '\\') {
                return new ParsedRegex(@"(|.*\\)", index + 3);
            } else {
                return null;
            }
        }
    }
}