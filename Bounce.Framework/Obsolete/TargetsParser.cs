using System.Collections.Generic;
using System.Reflection;

namespace Bounce.Framework.Obsolete {
    class TargetsParser : ITargetsParser {
        public IDictionary<string, IObsoleteTask> ParseTargetsFromObject(object targets) {
            if (targets is IDictionary<string, IObsoleteTask>) {
                return (IDictionary<string, IObsoleteTask>) targets;
            } else {
                var targetsDictionary = new Dictionary<string, IObsoleteTask>();

                foreach (PropertyInfo property in targets.GetType().GetProperties()) {
                    targetsDictionary[property.Name] = (IObsoleteTask) property.GetValue(targets, new object[0]);
                }

                return targetsDictionary;
            }
        }
    }
}