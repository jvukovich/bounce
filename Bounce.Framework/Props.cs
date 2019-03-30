using System.Collections.Generic;

namespace Bounce.Framework
{
    public static class Props
    {
        public static void Load(IDictionary<string, string> parsedCommandLineParameters)
        {
            Items = parsedCommandLineParameters;
        }

        // ReSharper disable once UnusedMember.Global
        public static void Set(string key, string value)
        {
            if (Items.ContainsKey(key))
            {
                Items[key] = value;
                return;
            }

            Items.Add(key, value);
        }

        public static string Get(string arg)
        {
            return Items.TryGetValue(arg, out var result) ? result : null;
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public static IDictionary<string, string> Items { get; set; } = new Dictionary<string, string>();
    }
}