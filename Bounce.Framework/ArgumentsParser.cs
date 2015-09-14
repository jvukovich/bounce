using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework {
    public class ArgumentsParser {
        public IDictionary<string, string> ParseParameters(IEnumerable<string> parameters) {
            return ParseCommandLineParameters(parameters.ToArray());
        }

        public IDictionary<string, string> ParseCommandLineParameters(string[] args)
        {
            var result = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("/"))
                {
                    var parameter = ParseParameter(args, ref i);

                    result.Add(parameter.Key, parameter.Value);
                }
                else
                {
                    throw new NonNamedArgumentException(arg);
                }
            }

			Props.Load(result);

            return result;
        }

        private static KeyValuePair<string, string> ParseParameter(string[] args, ref int i)
        {
            var arg = args[i];

            var indexOfColon = arg.IndexOf(":");
            if (indexOfColon >= 0)
            {
                var name = arg.Substring(1, indexOfColon - 1);
                var indexOfValue = indexOfColon + 1;
                var value = arg.Substring(indexOfValue, arg.Length - indexOfValue);

                return new KeyValuePair<string, string>(name, value);
            }
            else
            {
                var name = arg.Substring(1);
                if (i + 1 < args.Length)
                {
                    if (args[i + 1].StartsWith("/")) {
                        return new KeyValuePair<string, string>(name, "true");
                    } else {
                        i++;
                        return new KeyValuePair<string, string>(name, args[i]);
                    }
                }
                else
                {
                    return new KeyValuePair<string, string>(name, "true");
                }
            }
        }
    }
}