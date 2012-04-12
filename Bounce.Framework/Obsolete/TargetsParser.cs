using System.Collections.Generic;
using System.Reflection;

namespace Bounce.Framework.Obsolete {
    class TargetsParser : ITargetsParser {
        public IDictionary<string, ITask> ParseTargetsFromObject(object targets) {
            if (targets is IDictionary<string, ITask>) {
                return (IDictionary<string, ITask>) targets;
            } else {
                var targetsDictionary = new Dictionary<string, ITask>();

                foreach (PropertyInfo property in targets.GetType().GetProperties()) {
                    targetsDictionary[property.Name] = (ITask) property.GetValue(targets, new object[0]);
                }

                return targetsDictionary;
            }
        }
    }
}