using System.Collections.Generic;

namespace Bounce.Framework
{
    public static class Props
    {
        public static IDictionary<string, string> items = new Dictionary<string, string>();

        public static IDictionary<string, string> Items
        {
            get { return items; }
        }

        public static void Load(IDictionary<string, string> parsedCommandLineParameters)
        {
            items = parsedCommandLineParameters;
        }

        //public static void Load(string[] rawArguments) {
        //	var argIndex = -1;

        //	foreach (var arg in rawArguments) {
        //		argIndex++;

        //		string argStandardized;

        //		// Task args are always formatted like: /arg value or /arg:value
        //		// Reformat '/arg value' to '/arv:value' by combining with the next arg in the array
        //		if (arg.StartsWith("/") && !arg.Contains(":"))
        //			argStandardized = string.Format("{0}:{1}", arg, rawArguments[argIndex + 1]);
        //		else
        //			argStandardized = arg;

        //		if (!argStandardized.Contains(":"))
        //			continue;

        //		var dict = argStandardized.Split(':');
        //		var key = dict[0].Replace("/", string.Empty);
        //		var value = dict[1];

        //		Set(key, value);
        //	}
        //}

        public static void Set(string key, string value)
        {
            if (items.ContainsKey(key))
            {
                items[key] = value;
                return;
            }

            items.Add(key, value);
        }

        public static string Get(string arg)
        {
            string result;
            return items.TryGetValue(arg, out result) ? result : null;
        }
    }
}