using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bounce.Framework {
    public static class TargetsRetrieverExtensions {
        public static IEnumerable<Target> ToTargets(this IDictionary<string, ITask> targetsDictionary) {
            return targetsDictionary.Select(t => new Target { Name = t.Key, Task = t.Value });
        }
    }
}
