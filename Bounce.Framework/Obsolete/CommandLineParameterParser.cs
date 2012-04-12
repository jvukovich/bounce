using System.Collections.Generic;

namespace Bounce.Framework.Obsolete {
    public class CommandLineParameterParser : ICommandLineParameterParser {
        public ParsedCommandLineParameters ParseCommandLineParameters(string[] args) {
            var result = new ParsedCommandLineParameters();
            var remainingArguments = new List<string>();

            for (int i = 0; i < args.Length; i++) {
                var arg = args[i];

                if (arg.StartsWith("/")) {
                    ParsedCommandLineParameter parameter = ParseParameter(args, ref i);

                    if (parameter != null) {
                        result.Parameters.Add(parameter);
                    }
                } else {
                    remainingArguments.Add(arg);
                }
            }

            result.RemainingArguments = remainingArguments.ToArray();

            return result;
        }

        private static ParsedCommandLineParameter ParseParameter(string [] args, ref int i) {
            var arg = args[i];

            var indexOfColon = arg.IndexOf(":");
            if (indexOfColon >= 0) {
                var name = arg.Substring(1, indexOfColon - 1);
                var indexOfValue = indexOfColon + 1;
                var value = arg.Substring(indexOfValue, arg.Length - indexOfValue);

                return new ParsedCommandLineParameter {Name = name, Value = value};
            } else {
                i++;
                if (i < args.Length) {
                    var value = args[i];

                    var name = arg.Substring(1);
                    return new ParsedCommandLineParameter {Name = name, Value = value};
                } else {
                    return null;
                }
            }
        }
    }
}