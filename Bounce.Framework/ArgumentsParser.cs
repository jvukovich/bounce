using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework
{
    public static class ArgumentsParser
    {
        public static IDictionary<string, string> ParseParameters(IEnumerable<string> parameters)
        {
            var args = parameters.ToList();
            var result = new Dictionary<string, string>();

            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("/"))
                {
                    var (key, value) = ParseParameter(args, ref i);
                    result.Add(key, value);
                }
                else
                    throw new Exception($"expected switch argument beginning with '/', found '{arg}'");
            }

            Props.Load(result);

            return result;
        }

        private static KeyValuePair<string, string> ParseParameter(IReadOnlyList<string> args, ref int i)
        {
            var arg = args[i];

            var indexOfSeparator = arg.IndexOf(":");

            if (indexOfSeparator >= 0)
            {
                var name = arg.Substring(1, indexOfSeparator - 1);
                var indexOfValue = indexOfSeparator + 1;
                var value = arg.Substring(indexOfValue, arg.Length - indexOfValue);

                return new KeyValuePair<string, string>(name, value);
            }
            else
            {
                var name = arg.Substring(1);

                if (i + 1 >= args.Count)
                    return new KeyValuePair<string, string>(name, "true");

                if (args[i + 1].StartsWith("/"))
                    return new KeyValuePair<string, string>(name, "true");

                i++;

                return new KeyValuePair<string, string>(name, args[i]);
            }
        }
    }
}