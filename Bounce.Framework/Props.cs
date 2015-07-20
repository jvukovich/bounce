using System.Collections.Generic;

namespace Bounce.Framework {
	public static class Props {
		public static IDictionary<string, string> items = new Dictionary<string, string>();

		public static IDictionary<string, string> Items
		{
			get { return items; }
		}

		public static void Load(string[] rawArguments) {
			foreach (var arg in rawArguments) {
				if (!arg.StartsWith("/"))
					continue;

				if (!arg.Contains(":"))
					continue;

				var dict = arg.Split(':');

				var key = dict[0].Replace("/", string.Empty);
				var value = dict[1];

				Set(key, value);
			}
		}

		public static void Set(string key, string value) {
			if (items.ContainsKey(key))
			{
				items[key] = value;
				return;
			}

			items.Add(key, value);
		}

		public static string Get(string arg) {
			string result;
			return items.TryGetValue(arg, out result) ? result : null;
		}
	}
}